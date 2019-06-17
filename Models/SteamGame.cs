using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GameLauncher.ViewModels;
using GameLauncher.Utils;

namespace GameLauncher.Models
{
    public class SteamGame : LaunchGame
    {
        public bool AllowGameToBePlayed { get; set; }
        public ReadACF ReadACF { get; set; }
        public int GameID { get; set; }

        public SteamGame(int gameID, bool allowGameToBePlayed)
        {
            GameID = gameID;
            AllowGameToBePlayed = allowGameToBePlayed;
        }

        public override void Launch()
        {
            if (AllowGameToBePlayed)
            {
                var initialJson = File.ReadAllText(@"game.json");
                var gameList = JsonConvert.DeserializeObject<List<Game>>(initialJson);

                foreach (var game in gameList)
                {
                    if (MainViewModel.SelectedGame != null)
                    {
                        try
                        {
                            if (game.Name == MainViewModel.SelectedGame.Name)
                            {
                                ReadACF = new ReadACF(MainViewModel.SelectedGame.Name);
                                Process.Start($"steam://rungameid/{GameID}");
                            }
                        }
                        catch (Win32Exception) { }
                    }
                }
            }
        }
    }
}
