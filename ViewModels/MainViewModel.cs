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
using Microsoft.Win32;
using System.Linq;

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

        public static int GameID { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
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
            ResetAllSettingsCommand = new CommandRunner(ResetAllSettings);
            TileCommand = new CommandRunner(TileClick);
            PlayGameCommand = new CommandRunner(PlayGame);

            

            Scanner.Scan();
            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
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
            LaunchSteamGame();
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
                            ReadACF = new ReadACF(SelectedGame.Name);
                            Process.Start($"steam://rungameid/{GameID}");
                        }
                        if (game.Name == SelectedGame.Name && !game.Platform.Equals(Platforms.STEAM))
                        {
                            Process.Start(game.UserPreferedEXE);
                        }
                        else if (SelectedGame.Executables.Count == 1)
                            Process.Start(SelectedGame.Executables[0]);
                    }
                    catch (Win32Exception){}
                }
            }
        }

        private void LaunchSteamGame()
        {
            string Steam32 = "SOFTWARE\\VALVE\\STEAM\\APPS";
            string Steam64 = "SOFTWARE\\Wow6432Node\\Valve\\STEAM\\APPS";
            try
            {
                RegistryKey key = string.IsNullOrEmpty(Registry.LocalMachine.OpenSubKey(Steam64).ToString())
                    ? Registry.LocalMachine.OpenSubKey(Steam32)
                    : Registry.LocalMachine.OpenSubKey(Steam64);

                foreach (string k in key.GetSubKeyNames())
                {
                    using (RegistryKey subKey = key.OpenSubKey(k))
                    {
                        string steamPath = subKey.GetValue("InstallPath").ToString();
                        //string configPath = steamPath + "/steamapps/libraryfolders.vdf";
                        //if (File.Exists(configPath))
                        //{
                        //    IEnumerable<string> configLines = File.ReadAllLines(configPath)
                        //        .Where(l => !string.IsNullOrEmpty(l) && l.Contains(":\\"));
                        //    foreach (var line in configLines)
                        //    {
                        //        GameScanner.LibraryDirectories.Add(new Platform
                        //        {
                        //            PlatformType = Platforms.STEAM,
                        //            Name = nameof(Platforms.STEAM),
                        //            InstallationPath = $"{line.Substring(line.IndexOf(":") - 1, line.Length - line.IndexOf(":"))}\\steamapps\\common"
                        //        });
                        //    }

                        //    GameScanner.LibraryDirectories.Add(new Platform
                        //    {
                        //        PlatformType = Platforms.STEAM,
                        //        Name = nameof(Platforms.STEAM),
                        //        InstallationPath = $"{steamPath}\\steamapps\\common"
                        //    });
                        //}
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            RefreshGames();
        }

        private async void ResetAllSettingsWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Reset All Settings", "Warning you are about to reset all settings. Continue?");
        }

        private async void MultilpleEXEWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Multiple Exes", $"{SelectedGame.Name} has multiple exes. \nPlease choose the correct one to launch.");
        }

        private async void NoEXESetWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, $"{SelectedGame.Name}", $"No exe set, Please set a exe before playing.");
        }

        private async void UserPreferedEXEAlreadySETWarning()
        {
            await DialogCoordinator.ShowMessageAsync(this, "Warning", $"The prefered exe for the {SelectedGame.Name} game has already been set.");
        }
        #endregion
    }
}
