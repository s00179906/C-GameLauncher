using GameLauncher.Utils;
using GameLauncher.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Models
{
    public class NONEGame : LaunchGame
    {
        public bool AllowGameToBePlayed { get; set; }
        public ReadACF ReadACF { get; set; }

        public NONEGame(bool allowGameToBePlayed)
        {
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
                                Process.Start(game.UserPreferedEXE);
                        }
                        catch (Win32Exception) { }
                    }
                }
            }

        }
    }
}
