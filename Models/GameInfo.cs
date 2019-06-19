namespace GameLauncher.Models
{
    public class Rootobject
    {
        public GameInfo[] Property1 { get; set; }
    }

    public class GameInfo
    {
        public int id { get; set; }
        public Cover cover { get; set; }
        public string name { get; set; }
        public Screenshot[] screenshots { get; set; }
    }

    public class Cover
    {
        public int id { get; set; }
        public int game { get; set; }
        public int height { get; set; }
        public string image_id { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Screenshot
    {
        public int id { get; set; }
        public int game { get; set; }
        public int height { get; set; }
        public string image_id { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

}
