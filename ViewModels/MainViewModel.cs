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
            Games = new ObservableCollection<Game>();
            Scanner = new GameScanner();

            LaunchGameCommand = new CommandRunner(LaunchGame);
            AddFolderPathCommand = new CommandRunner(AddFolder);

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
            Scanner.LibraryDirectories.Add(new Platform
            {
                Name = "Agnostic",
                InstallationPath = Helper.SelectDir(),
                PlatformType = Platforms.None
            });

            Games.Clear();

            foreach (var exe in Scanner.GetExecutables())
            {
                Games.Add(exe);
            }
        }
    }
}
