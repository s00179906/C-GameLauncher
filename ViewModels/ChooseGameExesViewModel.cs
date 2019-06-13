using GameLauncher.Models;
using GameLauncher.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using GameLauncher.Views;
using System.Linq;

namespace GameLauncher.ViewModels
{
    public class ChooseGameExesViewModel
    {
        public Game Game { get; set; }
        public ObservableCollection<string> GameExecutables { get; set; }
        public string SelectedExecutable { get; set; }
        public CommandRunner SetEXECommand { get; set; }
        public ChooseGameExesView exeWindow { get; set; }

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
                MainViewModel.UserSelectedExe = SelectedExecutable;
                exeWindow = Application.Current.Windows.OfType<ChooseGameExesView>().SingleOrDefault(w => w.IsActive);
                exeWindow.Close();
            }
        }


    }
}
