using GameLauncher.Models;
using GameLauncher.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace GameLauncher.Utils
{
    public class IGDBAPIController
    {
        public RestClient Client { get; set; }
        public string Params { get; set; }
        public string Query { get; set; }
        public string GameName { get; set; }
        public List<Game> GameAPIData { get; set; }
        public ObservableCollection<Game> Games { get; set; }
        public List<GameInfo> JsonDeserialized { get; set; }
        public string APIKey { get; set; }
        public IGDBAPIController(ObservableCollection<Game> games)
        {
            APIKey = "d890bcccf131c0c13138621d6908fe1e";
            Client = new RestClient(APIKey);
            GameAPIData = new List<Game>();
            Games = games;
        }

        public string GetGameCovers()
        {
            List<string> gamesList = AddEachGameToList();
            string jsonResponse = "";

            foreach (var gameName in gamesList)
            {
                string coverImageID = "", name = "";

                // replaces any '-' with ':'
                // doing this so the game name can be found by the api to search its covers etc...
                // any '-' in the game name, the api will not find that game...
                gameName.Replace('-', ':');

                // construct the endpoint with the data required for the covers
                string endPointWithData = $"?search={gameName}&limit=1&fields=cover.*, name, screenshots.*";

                // set the endpoint
                Client.EndPoint = $"https://api-v3.igdb.com/games/{endPointWithData}";

                // make the request to the api
                jsonResponse = Client.MakeRequest();

                JsonDeserialized = JsonConvert.DeserializeObject<List<GameInfo>>(jsonResponse);

                if (JsonDeserialized.Count != 0)
                {
                    #region Get Name and Cover Image ID for the game
                    if (JsonDeserialized[0].cover != null)
                        coverImageID = JsonDeserialized[0].cover.image_id;

                    if (JsonDeserialized[0].name != null)
                        name = JsonDeserialized[0].name;
                    #endregion
                }

                string gameImageCover = $"https://images.igdb.com/igdb/image/upload/t_1080p/{coverImageID}.jpg";


                // setting the games cover and screenshots from here
                foreach (var game in Games)
                {
                    if (game.Name == gameName)
                    {
                        var charsToRemove = new string[] { ":", "-", "'", " " };
                        string gameNameFromGames = game.Name;

                        foreach (var c in charsToRemove)
                        {
                            if (!gameNameFromGames.Equals(name))
                            {
                                gameNameFromGames = gameNameFromGames.Replace(c, string.Empty);
                                name = name.Replace(c, string.Empty);
                            }
                        }

                        if (gameNameFromGames.ToUpper().Equals(name.ToUpper()))
                        {
                            var retrievedGameScreenshots = GetGameScreenshots();

                            if (retrievedGameScreenshots != null)
                            {
                                game.GameScreenshots = retrievedGameScreenshots;
                            }

                            if (gameImageCover != null || String.IsNullOrEmpty(gameImageCover))
                                game.GameCover = gameImageCover;
                            else
                                game.GameCover = "https://sisterhoodofstyle.com/wp-content/uploads/2018/02/no-image-1.jpg";
                        }
                    }
                }

            }
            return jsonResponse;
        }

        private List<string> AddEachGameToList()
        {
            List<string> gamesList = new List<string>();

            // add the game name to the above list
            foreach (var game in Games)
            {
                gamesList.Add(game.Name);
            }

            return gamesList;
        }

        private List<string> GetGameScreenshots()
        {
            List<string> screenshots = new List<string>();
            string screenshotURL = "";

            // all the screenshot retrieved from the api for the game
            if (JsonDeserialized.Count != 0)
            {
                if (JsonDeserialized[0].screenshots != null)
                {
                    var allGameScreenshots = JsonDeserialized[0].screenshots;

                    foreach (var screenshot in allGameScreenshots)
                    {
                        screenshotURL = $"https://images.igdb.com/igdb/image/upload/t_1080p/{screenshot.image_id}.jpg";
                        screenshots.Add(screenshotURL);
                    }
                }
            }
            return screenshots;
        }
    }
}
