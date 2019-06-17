using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Utils
{
    public class APIController
    {
        public RestClient Client { get; set; }
        public string APIKey { get; set; }
        public string Params { get; set; }
        public string Query { get; set; }
        public string GameName { get; set; }
        public APIController()
        {
            Client = new RestClient();
            string endPoint = "https://www.giantbomb.com/api/search/?";
            GameName = "Tekken 7";
            Query = $"&query={GameName}";
            APIKey = "api_key=13e2d0e2206f7def69b1e3d3fb83a505c59c262e";
            Params = "&resources=game&format=json&limit=1";

            string finalEndPoint = "https://www.giantbomb.com/api/games";
            string json = Client.Get(finalEndPoint);
        }
    }
}
