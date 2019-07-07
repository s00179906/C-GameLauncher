using System;
using System.Collections.Generic;
using GameLauncher.Models;
using GameLauncher.Utils;
using GameLauncher.Views;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace GameLauncher.ViewModels
{
    public class GameDetailedViewModel : INotifyPropertyChanged
    {
        private string _randomSelectedGameScreenshot;
        public string RandomSelectedGameScreenshot
        {
            get { return _randomSelectedGameScreenshot; }
            set { _randomSelectedGameScreenshot = value; OnPropertyChanged(nameof(RandomSelectedGameScreenshot)); }
        }
        public ChooseGameExesView Window { get; set; }
        public ReadACF ReadACF { get; set; }
        public static int GameID { get; set; }
        public bool AllowGameToBePlayed { get; set; }
        public Game SelectedGame { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public CommandRunner BackToMainViewCommand { get; set; }
        public CommandRunner TileCommand { get; private set; }
        public MainView MainView { get; set; }
        public List<string> SelectedGameScreenshots { get; set; }
        public Random Random = new Random();
        public GameDetailedView GameDetailView { get; set; } 

        public GameDetailedViewModel()
        {
            MainView = new MainView();
            //SelectedGame = _selectedGame;
            SelectedGame = MainViewModel.SelectedGame;
            BackToMainViewCommand = new CommandRunner(BackToView);
            TileCommand = new CommandRunner(TileClick);

            //var startTimeSpan = TimeSpan.Zero;
            //var periodTimeSpan = TimeSpan.FromSeconds(3);

            //var timer = new System.Threading.Timer((e) =>
            //{
            //    PickRandomGameScreenshot();
            //}, null, startTimeSpan, periodTimeSpan);
            var dueTime = TimeSpan.FromSeconds(0);
            var interval = TimeSpan.FromSeconds(3);

            // TODO: Add a CancellationTokenSource and supply the token here instead of None.
            _ = RunPeriodicAsync(PickRandomGameScreenshot, dueTime, interval, CancellationToken.None);

        }

        // The `onTick` method will be called periodically unless cancelled.
        private static async Task RunPeriodicAsync(Action onTick,
                                                   TimeSpan dueTime,
                                                   TimeSpan interval,
                                                   CancellationToken token)
        {
            // Initial wait time before we begin the periodic loop.
            if (dueTime > TimeSpan.Zero)
                await Task.Delay(dueTime, token);

            // Repeat this loop until cancelled.
            while (!token.IsCancellationRequested)
            {
                // Call our onTick function.
                onTick?.Invoke();

                // Wait to repeat again.
                if (interval > TimeSpan.Zero)
                    await Task.Delay(interval, token);
            }
        }
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
        private void PickRandomGameScreenshot()
        {
            SelectedGameScreenshots = SelectedGame.GameScreenshots;
            RandomSelectedGameScreenshot = SelectedGameScreenshots[Random.Next(0, SelectedGameScreenshots.Count)];
        }

        private void BackToView(object obj)
        {
            GameLauncherViewModel.MainFrame.Content = MainView;
        }
        private void TileClick(object obj)
        {
            //GameDetailView = new GameDetailedView(SelectedGame);
            //GameLauncherViewModel.MainFrame.Content = GameDetailView;
            SetPreferedEXE(obj);
            PlayGame(obj);
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
                        if (SelectedGame.Executables.Count != 1)
                        {
                            Window = new ChooseGameExesView(SelectedGame);
                            Window.ShowDialog();
                            AllowGameToBePlayed = false;
                        }
                        else
                        {
                            if (SelectedGame.Executables.Count == 1)
                            {
                                MainViewModel.SelectedGame.UserPreferedEXE = SelectedGame.Executables[0];
                            }
                        }
                    }
                }
            }
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
                            if (game.Name == SelectedGame.Name && SelectedGame.Platform.Equals(Platforms.STEAM))
                            {
                                ReadACF = new ReadACF(MainViewModel.SelectedGame.Name);
                                Process.Start($"steam://rungameid/{GameID}");
                            }

                            if (game.Name == SelectedGame.Name && SelectedGame.Platform.Equals(Platforms.NONE))
                            {
                                Process.Start(game.UserPreferedEXE);
                            }

                            if (game.Name == SelectedGame.Name && SelectedGame.Platform.Equals(Platforms.UPLAY))
                            {
                                Process.Start(game.UserPreferedEXE);
                            }

                            if (game.Name == SelectedGame.Name && SelectedGame.Platform.Equals(Platforms.EPIC))
                            {
                                Process.Start(game.UserPreferedEXE);
                            }
                        }
                        catch (Win32Exception) { }
                    }
                }
            }
        }
    }
}
