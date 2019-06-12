using GameLauncher.Models;
using GameLauncher.Utils;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace GameLauncher.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Game> Games { get; set; }
        public CommandRunner AddFolderPathCommand { get; set; }
        public GameScanner Scanner { get; set; }
        public CommandRunner LaunchGameCommand { get; private set; }
        public Game SelectedGame { get; set; }

        public MainViewModel()
        {
            LaunchGameCommand = new CommandRunner(LaunchGame);
            Games = new ObservableCollection<Game>();

            AddFolderPathCommand = new CommandRunner(AddFolder);

            Scanner = new GameScanner();

            Scanner.Scan();

            Games = Scanner.GetExecutables();
        }

        private void LaunchGame(object obj)
        {
            if (SelectedGame != null)
            {
                if (SelectedGame.Executables.Count == 1)
                {
                    Process.Start(SelectedGame.Executables[0]);
                }
                else
                {
                    MessageBox.Show("Game has more than one exe", "Whoops");
                }
            }
        }

        private void AddFolder(object obj)
        {
            Scanner.LibraryDirectories.Add(Helper.SelectDir());
        }
    }
}
