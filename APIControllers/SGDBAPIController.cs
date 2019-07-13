using GameLauncher.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.APIControllers
{
    public class SGDBAPIController
    {
        public RestClient Client { get; set; }
        public string APIKey { get; set; }
        public SGDBAPIController()
        {
            APIKey = "cd13d67850ce95d37e2074699647e6cf";
            Client = new RestClient(APIKey);
        }

        // Need to search for a game by name first
        // Then get the id of the searched game
        // And use another endpoint to get the searched games images

        private void MakeRequest()
        {
            Client.EndPoint = $"";
        }
    }
}
