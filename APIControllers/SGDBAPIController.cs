using GameLauncher.Models;
using GameLauncher.Utils;
using GameLauncher.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.APIControllers
{
    public class SGDBAPIController
    {
        public RestClient Client { get; set; }
        public string APIKey { get; set; }
        public ObservableCollection<Game> GamesOC { get; set; }

        public string GameName { get; set; }
        public int GameID { get; set; }
        public SGDBAPIController(ObservableCollection<Game> _gamesOC)
        {
            GamesOC = _gamesOC;
            APIKey = "cd13d67850ce95d37e2074699647e6cf";
            Client = new RestClient(APIKey);
            GetGameIDFromEndpoint();
        }

        private void GetGameIDFromEndpoint()
        {
            string jsonResponse = string.Empty;

            foreach (Game game in GamesOC)
            {
                GameName = game.Name;
                string searchByGameNameEndpoint = $"https://www.steamgriddb.com/api/v2/search/autocomplete/{GameName}";

                Client.EndPoint = searchByGameNameEndpoint;

                jsonResponse = Client.MakeRequest();

                var jsonDeserialized = JsonConvert.DeserializeObject<SGDBGameInfo>(jsonResponse);

                //foreach (var gameDeserialzied in jsonDeserialized.Data)
                //{
                //    string x = gameDeserialzied.Name.Replace(":", "");
                //    if (x.Equals(game.Name))
                //    {
                //        GameID = gameDeserialzied.Id;
                //    }
                //}
                if (jsonDeserialized.Data.Length != 0)
                {

                    GameID = jsonDeserialized.Data[0].Id;
                }
                GetGameCovers(game);
            }
        }

        private void GetGameCovers(Game _game)
        {
            string jsonResponse = string.Empty;
            string gameGridImageEndpoint = $"https://www.steamgriddb.com/api/v2/grids/game/{GameID}";

            Client.EndPoint = gameGridImageEndpoint;

            jsonResponse = Client.MakeRequest();

            var jsonDeserialized = JsonConvert.DeserializeObject<SGDBGameImageInfo>(jsonResponse);

            foreach (var g in jsonDeserialized.Data)
            {
                if (jsonDeserialized.Data.Length != 0)
                {
                    _game.WideGameCovers.Add(g.Thumb.AbsoluteUri);
                }
            }
        }
    }
}
