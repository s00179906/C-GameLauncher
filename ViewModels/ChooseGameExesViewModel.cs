using GameLauncher.Models;
using GameLauncher.Utils;
using System.Collections.ObjectModel;
using System.Windows;
using GameLauncher.Views;
using System.Linq;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace GameLauncher.ViewModels
{
    public class ChooseGameExesViewModel
    {
        public Game Game { get; set; }
        public ObservableCollection<string> GameExecutables { get; set; }
        public string SelectedExecutable { get; set; }
        public CommandRunner SetEXECommand { get; set; }
        public ChooseGameExesView ExeWindow { get; set; }

        public ChooseGameExesViewModel(Game game)
        {
            Game = game;
            GameExecutables = new ObservableCollection<string>();
            foreach (var exe in Game.Executables)
            {
                GameExecutables.Add(exe);
            }
            SetEXECommand = new CommandRunner(SetEXE);
        }

        private void SetEXE(object obj)
        {
            if (SelectedExecutable != null)
            {
                  MainViewModel.SelectedGame.UserPreferedEXE = SelectedExecutable;

                    if (MainViewModel.SelectedGame.UserPreferedEXE != null)
                    {
                        var initialJson = File.ReadAllText(@"game.json");

                        var list = JsonConvert.DeserializeObject<List<Game>>(initialJson);
                        Game game = new Game()
                        {
                            Executables = MainViewModel.SelectedGame.Executables,
                            ID = MainViewModel.SelectedGame.ID,
                            Platform = MainViewModel.SelectedGame.Platform,
                            UserPreferedEXE = MainViewModel.SelectedGame.UserPreferedEXE,
                            Name = MainViewModel.SelectedGame.Name
                        };

                        //create steam game

                        list.Add(game);
                        var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
                        File.WriteAllText(@"game.json", string.Empty);
                        File.AppendAllText(@"game.json", convertedJson);
                    }
                
                ExeWindow = Application.Current.Windows.OfType<ChooseGameExesView>().SingleOrDefault(w => w.IsActive);
                ExeWindow.Close();
            }
        }
    }
}
