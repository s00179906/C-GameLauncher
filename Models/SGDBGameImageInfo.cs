using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher.Models
{
    public class SGDBGameImageInfo
    {
        [JsonProperty("data")]
        public Datum[] Data { get; set; }
    }
    public partial class Datum
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("thumb")]
        public Uri Thumb { get; set; }
    }

}
