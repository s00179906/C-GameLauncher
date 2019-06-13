using GameLauncher.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace GameLauncher.Utils
{
    public class GameScanner
    {
        private readonly string Steam32 = "SOFTWARE\\VALVE\\";
        private readonly string Steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";
        private readonly string EpicRegistry = "SOFTWARE\\WOW6432Node\\EpicGames\\Unreal Engine";
        private readonly string UplayRegistry = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher\\Installs";

        public ObservableCollection<Platform> LibraryDirectories { get; set; }

        public GameScanner()
        {
            //Properties.Settings.Default.Reset();
            LibraryDirectories = new ObservableCollection<Platform>();
            //LibraryDirectories.CollectionChanged += new NotifyCollectionChangedEventHandler(UpdateSettings);
        }

        //private void UpdateSettings(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    foreach (Platform item in e.NewItems)
        //    {
        //        if (!Properties.Settings.Default.FolderPaths.Contains(item.InstallationPath))
        //        {
        //            Properties.Settings.Default.FolderPaths.Add(item.InstallationPath);
        //        }
        //    }

        //    Properties.Settings.Default.Save();
        //}

        public void Scan()
        {
            GetSteamDirs();
            GetEpicDirs();
            GetUplayDirs();
        }

        public void GetSteamDirs()
        {
            try
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
                                LibraryDirectories.Add(new Platform
                                {
                                    PlatformType = Platforms.Steam,
                                    Name = nameof(Platforms.Steam),
                                    InstallationPath = $"{line.Substring(line.IndexOf(":") - 1, line.Length - line.IndexOf(":"))}\\steamapps\\common"
                                });
                            }

                            LibraryDirectories.Add(new Platform
                            {
                                PlatformType = Platforms.Steam,
                                Name = nameof(Platforms.Steam),
                                InstallationPath = $"{steamPath}\\steamapps\\common"
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetEpicDirs()
        {
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(EpicRegistry))
                {
                    foreach (string k in key.GetSubKeyNames())
                    {
                        using (RegistryKey subkey = key.OpenSubKey(k))
                        {
                            string epicGamesPath = subkey.GetValue("InstalledDirectory").ToString();
                            epicGamesPath = epicGamesPath.Substring(0, epicGamesPath.Length - 4);
                            LibraryDirectories.Add(new Platform
                            {
                                PlatformType = Platforms.Epic,
                                Name = "Epic",
                                InstallationPath = epicGamesPath
                            });
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetUplayDirs()
        {
            try
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
                            LibraryDirectories.Add(new Platform
                            {
                                PlatformType = Platforms.UPlay,
                                Name = nameof(Platforms.UPlay),
                                InstallationPath = uplayPathTrimmed
                            });

                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public ObservableCollection<Game> GetExecutables()
        {
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            foreach (Platform libDir in LibraryDirectories)
            {
                string[] gameDirs = Directory.GetDirectories(libDir.InstallationPath);

                foreach (var gameDir in gameDirs)
                {
                    string[] exes = Directory.GetFiles(gameDir, "*.exe");
                    Game game = new Game
                    {
                        Name = new DirectoryInfo(gameDir).Name,
                        Platform = libDir.PlatformType,
                        Executables = new List<string>(exes)
                    };

                    games.Add(game);
                }
            }

            return games;
        }
    }
}
