﻿using GameLauncher.Models;
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
        public IDialogCoordinator DialogCoordinator { get; set; }

        public ChooseGameExesViewModel(Game game, IDialogCoordinator instance)
        {
            Game = game;
            DialogCoordinator = instance;
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
                //MultilpleEXEWarning(); // crashes the app for some reason.
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

                    list.Add(game);
                    var convertedJson = JsonConvert.SerializeObject(list, Formatting.Indented);
                    File.WriteAllText(@"game.json", string.Empty);
                    File.AppendAllText(@"game.json", convertedJson);
                }

                ExeWindow = Application.Current.Windows.OfType<ChooseGameExesView>().SingleOrDefault(w => w.IsActive);
                ExeWindow.Close();
            }
        }
        public string AddObjectsToJson<JObject>(string json, JObject objects)
        {
            List<JObject> list = JsonConvert.DeserializeObject<List<JObject>>(json);
            list.Add(objects);
            return JsonConvert.SerializeObject(list);
        }
        private async void MultilpleEXEWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, $"{MainViewModel.SelectedGame.Name}", $"You are about to add the selected exe as prefered exe. Continue?");
        }
    }
}
