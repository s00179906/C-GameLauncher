namespace GameLauncher.Models
{
    public enum Platforms { STEAM, EPIC, ORIGIN, BETHESDA, UPLAY, BLIZZARD, NONE }

    public class Platform
    {
        public string Name { get; set; }
        public Platforms PlatformType { get; set; }
        public string InstallationPath { get; set; }
    }
}
