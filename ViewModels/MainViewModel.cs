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
using System.Windows.Controls;

namespace GameLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Private Members
        private ObservableCollection<Game> _games;
        private bool firstTimeConfiguration;
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
        public CommandRunner ResetAllSettingsCommand { get; private set; }
        public CommandRunner PassSelectedGameToViewCommand { get; private set; }
        public CommandRunner ScanGamesCommand { get; set; }
        public GameScanner Scanner { get; set; }
        public GameScanner GameScanner { get; set; }
        public static Game SelectedGame { get; set; }
        public Platform SelectedFolder { get; set; }
        public ChooseGameExesView Window { get; set; }
        public static int GameID { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public APIController APIController { get; set; }
        public string JSONResponse { get; set; }
        public string Cover { get; set; }
        public List<Game> GameCovers { get; set; }
        public CommandRunner CloseSettingsCommand { get; set; }

        public string[] ComboBoxItems = { "GENRES", "----------------------------------------", "FPS", "RPG", "ACTION", "ADVENTURE", "HORROR", "RACING" };
        public MainView MainView { get; set; }
        public GameDetailedView GameDetailedView { get; set; }
        public Platform SelectedFolderValue { get; set; }
        #endregion

        #region Constructor
        public MainViewModel()
        {
            //Properties.Settings.Default.Reset();
            CloseSettingsCommand = new CommandRunner(CloseSettings);
            FirstTimeConfiguration();
            ScanGamesCommand = new CommandRunner(ScanGames);
            AddFolderPathCommand = new CommandRunner(AddDir);
            DeleteFolderPathCommand = new CommandRunner(DeleteDir);
            FilterGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);
            ResetAllSettingsCommand = new CommandRunner(ResetAllSettings);
            PassSelectedGameToViewCommand = new CommandRunner(PassSelectedGameToGameDetailedView);
            Scanner = new GameScanner();
            Scanner.Scan();
            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
            APIController = new APIController(Games);
            APIController.GetGameCovers();
            GameCovers = APIController.GameCovers;
            SetGameCover();
        }

        /// <summary>
        /// Quick fix, we need to rework this process.. 
        /// </summary>
        /// <param name="obj"></param>
        private void PassSelectedGameToGameDetailedView(object obj)
        {
            SelectedGame = obj as Game;
            GameDetailedView = new GameDetailedView();
            GameLauncherViewModel.MainFrame.Content = GameDetailedView;
        }
        #endregion

        #region VM Methods
        private void CloseSettings(object obj)
        {
            GameLauncherViewModel.MainFrame.Content = new MainView();
        }
        private void FirstTimeConfiguration()
        {
            firstTimeConfiguration = Properties.Settings.Default.FirstTimeConfig;
            if (firstTimeConfiguration)
            {
                File.WriteAllText(@"game.json", string.Empty);
                File.WriteAllText(@"game.json", "[]");
                Properties.Settings.Default.FolderPaths.Clear();
                Properties.Settings.Default.FirstTimeConfig = false;
                Properties.Settings.Default.Save();
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
                        if (cover.GameCover != null || String.IsNullOrEmpty(cover.GameCover))
                        {
                            game.GameCover = cover.GameCover;
                        }
                        else
                        {
                            game.GameCover = "https://sisterhoodofstyle.com/wp-content/uploads/2018/02/no-image-1.jpg";
                        }
                        game.GameScreenshots = cover.GameScreenshots;
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
            if (!String.IsNullOrEmpty(SelectedFolder.InstallationPath))
            {
                //remove from c# settings
                Helper.DeleteDir(SelectedFolder.InstallationPath);

                // remove from library directories 
                // because when vm gets instanciated the library from settings gets added to the oc
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
            File.WriteAllText(@"game.json", string.Empty);
            File.WriteAllText(@"game.json", "[]");
            Scanner.LibraryDirectories.Clear();
            Properties.Settings.Default.FolderPaths.Clear();
            Properties.Settings.Default.Save();
            RefreshGames();
        }
        #endregion
    }
}
