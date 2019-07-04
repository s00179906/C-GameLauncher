using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameLauncher.Models;
using GameLauncher.Utils;
using System.Windows.Navigation;
using GameLauncher.Views;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using System.ComponentModel;

namespace GameLauncher.ViewModels
{
    public class GameDetailedViewModel : INotifyPropertyChanged
    {
        public ChooseGameExesView Window { get; set; }
        public ReadACF ReadACF { get; set; }
        public static int GameID { get; set; }
        public bool AllowGameToBePlayed { get; set; }
        public Game SelectedGame { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private string _randomSelectedGameScreenshot;
        public string RandomSelectedGameScreenshot
        {
            get { return _randomSelectedGameScreenshot; }
            set { _randomSelectedGameScreenshot = value; OnPropertyChanged(nameof(RandomSelectedGameScreenshot)); }
        }

        public CommandRunner BackToMainViewCommand { get; set; }
        public CommandRunner TileCommand { get; private set; }
        public GameDetailedViewModel(Game _selectedGame)
        {
            SelectedGame = _selectedGame;
            BackToMainViewCommand = new CommandRunner(BackToView);
            TileCommand = new CommandRunner(TileClick);
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(3);

            var timer = new System.Threading.Timer((e) =>
            {
                PickRandomGameScreenshot();
            }, null, startTimeSpan, periodTimeSpan);
            //GameScreenshotSlideshow();
        }

        private void GameScreenshotSlideshow()
        {
        }

        private void PickRandomGameScreenshot()
        {

            Random random = new Random();  // Only do this once
            List<string> SelectedGameScreenshots = SelectedGame.GameScreenshots; //might slow down
            RandomSelectedGameScreenshot = SelectedGameScreenshots[random.Next(0, SelectedGameScreenshots.Count)];
        }

        private void BackToView(object obj)
        {
            MainView view = new MainView();
            GameLauncherViewModel.MainFrame.Content = view;
        }

        /// <summary>
        /// Quick fix, we need to rework this process.. 
        /// </summary>
        /// <param name="obj"></param>
        private void TileClick(object obj)
        {
            GameDetailedView view = new GameDetailedView(SelectedGame);
            GameLauncherViewModel.MainFrame.Content = view;
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
                        Window = new ChooseGameExesView(SelectedGame);
                        Window.ShowDialog();
                        AllowGameToBePlayed = false;
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
