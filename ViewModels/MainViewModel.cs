using GameLauncher.Models;
using GameLauncher.Utils;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using GameLauncher.Views;
using MahApps.Metro.Controls.Dialogs;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;
using System.Diagnostics;

/*
    TODOS:
        1. Add spinner for app launching > DONE

*/

namespace GameLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {

        #region Private Members
        private ObservableCollection<Game> _games;
        private bool firstTimeConfiguration = true;
        #endregion

        #region Public Properties 
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
        public CommandRunner ScanGamesCommand { get; set; }
        public GameScanner Scanner { get; set; }
        public GameScanner GameScanner { get; set; }
        public static Game SelectedGame { get; set; }
        public Platform SelectedFolder { get; set; }
        public ChooseGameExesView Window { get; set; }
        public IDialogCoordinator DialogCoordinator { get; set; }
        public ReadACF ReadACF { get; set; }
        public bool AllowGameToBePlayed { get; set; }
        public static int GameID { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public APIController APIController { get; set; }
        public string JSONResponse { get; set; }
        public string Cover { get; set; }
        public List<Game> GameCovers { get; set; }

        #endregion

        #region Constructor
        public MainViewModel(IDialogCoordinator instance)
        {
            DialogCoordinator = instance;
            
            ScanGamesCommand = new CommandRunner(ScanGames);
            SetPreferedEXECommand = new CommandRunner(SetPreferedEXE);
            AddFolderPathCommand = new CommandRunner(AddDir);
            DeleteFolderPathCommand = new CommandRunner(DeleteDir);
            FilterGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);
            ResetAllSettingsCommand = new CommandRunner(ResetAllSettings);
            TileCommand = new CommandRunner(TileClick);
            
            Scanner = new GameScanner();
            Scanner.Scan();
            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
            FirstTimeConfiguration();
            APIController = new APIController(Games);
            APIController.GetGameCovers();
            GameCovers = APIController.GameCovers;
            SetGameCover();
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

        private void FirstTimeConfiguration()
        {
            if (firstTimeConfiguration)
            {
                File.WriteAllText(@"game.json", string.Empty);
                File.WriteAllText(@"game.json", "[]");
                Properties.Settings.Default.FolderPaths.Clear();
                Properties.Settings.Default.Save();
                firstTimeConfiguration = false;
            }
        }

        private void SetGameCover()
        {
            foreach (var cover in GameCovers)
            {
                foreach (var game in Games)
                {
                    string gameNameFromOC = game.Name;
                    string gameNameFromCovers = cover.Name;
                    var charsToRemove = new string[] { ":", "-", "'", " " };

                    foreach (var c in charsToRemove)
                    {
                        gameNameFromCovers = gameNameFromCovers.Replace(c, string.Empty);
                        gameNameFromOC = gameNameFromOC.Replace(c, string.Empty);
                    }

                    if (gameNameFromOC.ToUpper() == gameNameFromCovers.ToUpper())
                    {
                        game.GameCover = cover.GameCover;
                    }
                }
            }
        }

        private void SetPreferedEXE(object obj)
        {
            if (SelectedGame != null)
            {
                //should put this into ctor
                string json = "game.json";
                if (!File.Exists(json))
                {
                    using (StreamWriter file = File.CreateText(@"game.json"))
                    {
                        file.WriteLine("[]");
                    }
                }
                else
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
        }

        //Used to scan games when settings have been reset.
        private void ScanGames(object obj)
        {
            Scanner.Scan();
            RefreshGames();
        }

        private void PlayGame(object obj)
        {
            if (AllowGameToBePlayed)
            {
                var initialJson = File.ReadAllText(@"game.json");
                var gameList = JsonConvert.DeserializeObject<List<Game>>(initialJson);

                foreach (var game in gameList)
                {
                    if (SelectedGame != null)
                    {
                        try
                        {
                            if (game.Name == SelectedGame.Name && game.Platform.Equals(Platforms.STEAM))
                            {
                                ReadACF = new ReadACF(MainViewModel.SelectedGame.Name);
                                Process.Start($"steam://rungameid/{GameID}");
                            }

                            if (game.Name == SelectedGame.Name && game.Platform.Equals(Platforms.NONE))
                            {
                                ReadACF = new ReadACF(MainViewModel.SelectedGame.Name);
                                Process.Start(SelectedGame.UserPreferedEXE);
                            }
                        }
                        catch (Win32Exception) { }
                    }
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
            SetGameCover();
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
