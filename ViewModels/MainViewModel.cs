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
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Game> _games;

        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set { _games = value; OnPropertyChanged(nameof(Games)); }
        }

        public ICollectionView FilteredGames { get; set; }

        public CommandRunner AddFolderPathCommand { get; set; }
        public CommandRunner DeleteFolderPathCommand { get; private set; }
        public CommandRunner FilterGamesCommand { get; private set; }
        public CommandRunner LaunchGameCommand { get; private set; }
        public GameScanner Scanner { get; set; }
        public Game SelectedGame { get; set; }
        public Platform SelectedFolder { get; set; }
        public ChooseGameExesView window { get; set; }
        public static string UserSelectedExe { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            Games = new ObservableCollection<Game>();
           
            Scanner = new GameScanner();

            LaunchGameCommand = new CommandRunner(LaunchGame);
            AddFolderPathCommand = new CommandRunner(AddDir);
            DeleteFolderPathCommand = new CommandRunner(DeleteDir);
            FilterGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);

            Scanner.Scan();

            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
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
                    window = new ChooseGameExesView(SelectedGame);
                    window.ShowDialog();
                    Process.Start(UserSelectedExe);
                }
            }
        }

        private void AddDir(object obj)
        {
            string dirToAdd = Helper.SelectDir();

            if (!string.IsNullOrEmpty(dirToAdd))
            {
                Scanner.LibraryDirectories.Add(new Platform
                {
                    Name = "Agnostic",
                    InstallationPath = dirToAdd,
                    PlatformType = Platforms.NONE
                });

                RefreshGames();
            }
        }

        private void DeleteDir(object obj)
        {
            if (SelectedFolder != null)
            {
                Scanner.LibraryDirectories.Remove(SelectedFolder);
                RefreshGames();
            }
        }

        private void RefreshGames()
        {
            Games.Clear();
            foreach (var exe in Scanner.GetExecutables())
            {
                Games.Add(exe);
            }
        }

        private void FilterGamesByPlatformSteam(object obj)
        {
            if (obj as string == "ALL")
            {
                FilteredGames.Filter = game => game.Equals(game);
            }
            else
            {
                Enum.TryParse(obj as string, out Platforms selectedPlatform);
                FilteredGames.Filter = game => ((Game)game).Platform.Equals(selectedPlatform);
            }
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

    }
}
