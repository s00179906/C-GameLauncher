using GameLauncher.Models;
using Microsoft.Win32;
using System;
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
        public readonly string EpicRegistry = "SOFTWARE\\WOW6432Node\\EpicGames\\Unreal Engine";
        public readonly string UplayRegistry = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher\\Installs";

        public ObservableCollection<string> LibraryDirectories { get; set; }

        public GameScanner()
        {
            LibraryDirectories = new ObservableCollection<string>();
        }

        public void Scan()
        {
            GetSteamDirs();
            GetEpicDirs();
            GetUplayDirs();
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
                            .Where(l => !string.IsNullOrEmpty(l) && l.Contains(":\\"));
                        foreach (var line in configLines)
                        {
                            LibraryDirectories.Add($"{line.Substring(line.IndexOf(":") - 1, line.Length - line.IndexOf(":"))}\\steamapps\\common");
                        }
                        LibraryDirectories.Add($"{steamPath}\\steamapps\\common");
                    }
                }
            }
        }

        public void GetEpicDirs()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(EpicRegistry))
            {
                foreach (string k in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(k))
                    {
                        string epicGamesPath = subkey.GetValue("InstalledDirectory").ToString();
                        epicGamesPath = epicGamesPath.Substring(0, epicGamesPath.Length - 4);
                        LibraryDirectories.Add(epicGamesPath);
                    }
                }
            }
        }

        public void GetUplayDirs()
        {
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(UplayRegistry))
            {
                foreach (string k in key.GetSubKeyNames())
                {
                    using (RegistryKey subKey = key.OpenSubKey(k))
                    {
                        string uplayPath = subKey.GetValue("InstallDir").ToString();
                        string uplayPathTrimmed = uplayPath.Substring(0, 58);
                        //string[] splitTitle = uplayPath.Split('/');
                        //int largest = splitTitle.Length;
                        //largest = largest - 2;
                        //string title = splitTitle[largest];
                        LibraryDirectories.Add(uplayPathTrimmed);
                    }
                }
            }
        }

        public dynamic DeterminePlatform()
        {
            Platforms platform = Platforms.Steam;
            foreach (var dir in LibraryDirectories)
            {
                if (dir.Contains("Steam"))
                    platform = Platforms.Steam;

                else if (dir.Contains("Epic"))
                    platform = Platforms.Epic;

                else if (dir.Contains("Bethesda"))
                    platform = Platforms.Bethesda;

                else
                    platform = Platforms.Origin;
            }
            return platform;
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
                        Platform = DeterminePlatform(),
                        Executables = new List<string>(exes)
                    };
                    games.Add(game);
                }
            }

            return games;
        }
    }
}
