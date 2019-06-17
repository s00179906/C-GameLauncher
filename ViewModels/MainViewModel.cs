using GameLauncher.Models;
using GameLauncher.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Data;
using GameLauncher.Views;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

/*
    TODOS:
        1. Add spinner for games launching

*/

namespace GameLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Public Props 
        private ObservableCollection<Game> _games;
        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set { _games = value; OnPropertyChanged(nameof(Games)); }
        }
        public ICollectionView FilteredGames { get; set; }
        public CommandRunner AddFolderPathCommand { get; private set; }
        public CommandRunner DeleteFolderPathCommand { get; private set; }
        public CommandRunner FilterGamesCommand { get; private set; }
        public CommandRunner SetPreferedEXECommand { get; private set; }
        public CommandRunner ResetAllSettingsCommand { get; private set; }
        public CommandRunner TileCommand { get; private set; }
        public CommandRunner PlayGameCommand { get; set; }
        public GameScanner Scanner { get; set; }
        public static Game SelectedGame { get; set; }
        public Platform SelectedFolder { get; set; }
        public ChooseGameExesView Window { get; set; }
        public static string UserSelectedExe { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }
        public GameScanner GameScanner { get; set; }
        public ReadACF ReadACF { get; set; }
        public CommandRunner ScanGamesCommand { get; set; }
        public bool AllowGameToBePlayed { get; set; }
        public static int GameID { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public APIController APIController { get; set; }
        #endregion

        #region Constructor
        public MainViewModel(IDialogCoordinator instance)
        {
            Games = new ObservableCollection<Game>();
            DialogCoordinator = instance;
            Scanner = new GameScanner();
            ScanGamesCommand = new CommandRunner(ScanGames);
            SetPreferedEXECommand = new CommandRunner(SetPreferedEXE);
            AddFolderPathCommand = new CommandRunner(AddDir);
            DeleteFolderPathCommand = new CommandRunner(DeleteDir);
            FilterGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);
            ResetAllSettingsCommand = new CommandRunner(ResetAllSettings);
            TileCommand = new CommandRunner(TileClick);
            PlayGameCommand = new CommandRunner(PlayGame);
            Scanner.Scan();
            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
            APIController = new APIController();
        }

        /// <summary>
        /// Quick fix, we need to rework this process.. 
        /// </summary>
        /// <param name="obj"></param>
        private void TileClick(object obj)
        {
            SelectedGame = obj as Game;
            SetPreferedEXE(obj);
            PlayGame(obj);
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
                AllowGameToBePlayed = true;

                if (gameFound == null)
                {
                    Window = new ChooseGameExesView(SelectedGame);
                    Window.ShowDialog();
                    AllowGameToBePlayed = false;
                }
            }
        }

        //Used to scan games when settings have been reset.
        private void ScanGames(object obj)
        {
            Scanner.Scan();
            RefreshGames();
        }

        private void PlayGame(object obj)
        {
            switch (SelectedGame.Platform)
            {
                case Platforms.STEAM:
                    SteamGame steam = new SteamGame(GameID, AllowGameToBePlayed);
                    steam.Launch();
                    break;

                case Platforms.NONE:
                    NONEGame none = new NONEGame(AllowGameToBePlayed);
                    none.Launch();
                    break;

                default:
                    MessageBox.Show("Game Platform not playable...");
                    break;
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

        private void ResetAllSettings(object obj)
        {
            ResetAllSettingsWarning();
            File.WriteAllText(@"game.json", string.Empty);
            File.WriteAllText(@"game.json", "[]");
            Scanner.LibraryDirectories.Clear();
            Properties.Settings.Default.FolderPaths.Clear();
            Properties.Settings.Default.Save();
            RefreshGames();
        }

        private async void ResetAllSettingsWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Reset All Settings", "Warning you are about to reset all settings. Continue?");
        }
        #endregion
    }
}
