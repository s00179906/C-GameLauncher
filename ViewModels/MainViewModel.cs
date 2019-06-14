using GameLauncher.Models;
using GameLauncher.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using GameLauncher.Views;
using MahApps.Metro.Controls.Dialogs;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;

namespace GameLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Public & Private Members 
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
        public CommandRunner SetPreferedEXECommand { get; private set; }
        public CommandRunner ResetDirPathsCommand { get; set; }
        public GameScanner Scanner { get; set; }
        public static Game SelectedGame { get; set; }
        public Platform SelectedFolder { get; set; }
        public ChooseGameExesView Window { get; set; }
        public static string UserSelectedExe { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;


        public CommandRunner PlayGameCommand { get; set; }


        #endregion

        #region Constructor
        public MainViewModel(IDialogCoordinator instance)
        {
            Games = new ObservableCollection<Game>();
            DialogCoordinator = instance;
            Scanner = new GameScanner();
            SetPreferedEXECommand = new CommandRunner(SetPreferedEXE);
            AddFolderPathCommand = new CommandRunner(AddDir);
            DeleteFolderPathCommand = new CommandRunner(DeleteDir);
            FilterGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);
            ResetDirPathsCommand = new CommandRunner(ResetDirPaths);


            PlayGameCommand = new CommandRunner(PlayGame);


            Scanner.Scan();
            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
        }
        #endregion

        #region VM Methods

        private void SetPreferedEXE(object obj)
        {
            if (SelectedGame != null)
            {
                var initialJson = File.ReadAllText(@"game.json");
                var gameDirList = JsonConvert.DeserializeObject<List<Game>>(initialJson);
                var gameFound = gameDirList.Find(game => game.Name == SelectedGame.Name);

                if (gameFound != null)
                {
                    UserPreferedEXEAlreadySETWarning();
                }
                else
                {
                    MultilpleEXEWarning();
                    Window = new ChooseGameExesView(SelectedGame);
                    Window.ShowDialog();
                }
            }
        }

        private void PlayGame(object obj)
        {
            var initialJson = File.ReadAllText(@"game.json");

            var gameList = JsonConvert.DeserializeObject<List<Game>>(initialJson);

            foreach (var game in gameList)
            {
                if (game.Name == SelectedGame.Name)
                {
                    Process.Start(game.UserPreferedEXE);
                }
                else if (SelectedGame.Executables.Count == 1)
                    Process.Start(SelectedGame.Executables[0]);
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

        private void ResetDirPaths(object obj)
        {
            ResetAllSettingsWarning();
            Properties.Settings.Default.Reset();
        }

        private async void ResetAllSettingsWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Reset All Settings", "Warning you are about to reset all settings. Continue?");
        }

        private async void MultilpleEXEWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Multiple Exes", $"{SelectedGame.Name} has multiple exes. \nPlease choose the correct one to launch.");
        }

        private async void UserPreferedEXEAlreadySETWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Warning", $"The prefered exe for the {SelectedGame.Name} game has already been set.");
        }
        #endregion
    }
}
