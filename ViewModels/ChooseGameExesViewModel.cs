using GameLauncher.Models;
using GameLauncher.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using GameLauncher.Views;

namespace GameLauncher.ViewModels
{
    public class ChooseGameExesViewModel
    {
        public Game Game { get; set; }
        public ObservableCollection<string> GameExecutables { get; set; }
        public string SelectedExecutable { get; set; }
        public ChooseGameExesViewModel(Game game)
        {
            Game = game;
            GameExecutables = new ObservableCollection<string>();
            foreach (var exe in Game.Executables)
            {
                GameExecutables.Add(exe);
            }
            
        }
    }

   
}
