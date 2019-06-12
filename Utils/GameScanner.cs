using GameLauncher.Models;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace GameLauncher.Utils
{
    public class GameScanner
    {
        public readonly string Steam32 = "SOFTWARE\\VALVE\\";
        public readonly string Steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";

        public ObservableCollection<string> LibraryDirectories { get; set; }

        public GameScanner()
        {
            LibraryDirectories = new ObservableCollection<string>();
        }

        public void Scan()
        {
            GetSteamDirs();
        }

        public void GetSteamDirs()
        {
            RegistryKey key = string.IsNullOrEmpty(Registry.LocalMachine.OpenSubKey(Steam64).ToString())
                ? Registry.LocalMachine.OpenSubKey(Steam32)
                : Registry.LocalMachine.OpenSubKey(Steam64);

            foreach (string k in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(k))
                {
                    string steamPath = subKey.GetValue("InstallPath").ToString();
                    string configPath = steamPath + "/steamapps/libraryfolders.vdf";
                    if (File.Exists(configPath))
                    {
                        IEnumerable<string> configLines = File.ReadAllLines(configPath)
                            .Where(l => !string.IsNullOrEmpty(l) && l.Contains(":/"));
                        foreach (var line in configLines)
                        {
                            LibraryDirectories.Add($"{line}\\steamapps\\common");
                        }
                        LibraryDirectories.Add($"{steamPath}\\steamapps\\common");
                    }
                }
            }
        }

        public ObservableCollection<Game> GetExecutables()
        {
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            foreach (string dir in LibraryDirectories)
            {
                string[] gameDirs = Directory.GetDirectories(dir);


                foreach (var gameDir in gameDirs)
                {
                    string[] exes = Directory.GetFiles(gameDir, "*.exe");
                    Game game = new Game
                    {
                        Name = new DirectoryInfo(gameDir).Name,
                        Platform = Platforms.Steam,
                        Executables = new List<string>(exes)
                    };
                    games.Add(game);
                }
            }

            return games;
        }
    }
}
