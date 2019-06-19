using GameLauncher.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace GameLauncher.Utils
{
    public class APIController
    {
        public RestClient Client { get; set; }
        public string APIKey { get; set; }
        public string Params { get; set; }
        public string Query { get; set; }
        public string GameName { get; set; }
        public List<Game> GameCovers { get; set; }
        public ObservableCollection<Game> Games { get; set; }
        public APIController(ObservableCollection<Game> games)
        {
            APIKey = "41cca02193617b8b009006af5f560649";
            Client = new RestClient();
            GameCovers = new List<Game>();
            Games = games;
        }

        public string GetGameCovers()
        {
            List<string> list = new List<string>();

            foreach (var game in Games)
            {
                list.Add(game.Name);
            }
            string json = "";
            foreach (var game in list)
            {
                string cover = "", name = "";
                string game2 = game.Replace('-', ':');
                string stringData = $"?search={game2}&limit=1&fields=cover.*, name";
                Client.EndPoint = $"https://api-v3.igdb.com/games/{stringData}";
                json = Client.MakeRequest();

                var g = JsonConvert.DeserializeObject<List<GameInfo>>(json);
                if (g.Count != 0)
                {
                    if (g[0].cover != null)
                    {
                        cover = g[0].cover.image_id;
                       
                    }
                    if (g[0].name != null)
                    {
                        name = g[0].name;
                    }

                }
                string finalCover = $"https://images.igdb.com/igdb/image/upload/t_cover_big/{cover}.jpg";

                Game test = new Game()
                {
                    Name = name,
                    GameCover = finalCover
                };
                GameCovers.Add(test);
            }

            return json;
        }
    }
}
