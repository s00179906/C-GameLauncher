using GameLauncher.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace GameLauncher.Utils
{
    public class GameScanner
    {
        private readonly string Steam32 = "SOFTWARE\\VALVE\\";
        private readonly string Steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";
        private readonly string EpicRegistry = "SOFTWARE\\WOW6432Node\\EpicGames\\Unreal Engine";
        private readonly string UplayRegistry = "SOFTWARE\\WOW6432Node\\Ubisoft\\Launcher";
        private readonly string OriginsRegistry = "SOFTWARE\\WOW6432Node\\Origin";
        public bool DirExists { get; set; }

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
            GetOriginsDir();
            GetEpicDirs();
            GetUplayDirs();
            ReadUserAddedDirectories();
        }
        private void ReadUserAddedDirectories()
        {
            var dirs = Properties.Settings.Default.FolderPaths;
            if (dirs != null || dirs.Count != 0)
            {
                foreach (var dir in dirs)
                {
                    LibraryDirectories.Add(new Platform
                    {
                        PlatformType = Platforms.NONE,
                        Name = nameof(Platforms.NONE),
                        InstallationPath = dir
                    });
                }
            }
        }
        public void GetSteamDirs()
        {
            try
            {
                RegistryKey key = string.IsNullOrEmpty(Registry.LocalMachine.OpenSubKey(Steam64).ToString())
                    ? Registry.LocalMachine.OpenSubKey(Steam32)
                    : Registry.LocalMachine.OpenSubKey(Steam64);

                DirExists = CheckIfRegistryDirExists(Steam64 + "\\Steam", "InstallPath");

                if (DirExists)
                {
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
                                        PlatformType = Platforms.STEAM,
                                        Name = nameof(Platforms.STEAM),
                                        InstallationPath = $"{line.Substring(line.IndexOf(":") - 1, line.Length - line.IndexOf(":"))}\\steamapps\\common"
                                    });
                                }

                                LibraryDirectories.Add(new Platform
                                {
                                    PlatformType = Platforms.STEAM,
                                    Name = nameof(Platforms.STEAM),
                                    InstallationPath = $"{steamPath}\\steamapps\\common"
                                });
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Could not find Steam directories...");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void GetOriginsDir()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(OriginsRegistry);

            foreach (string ksubKey in key.GetSubKeyNames())
            {
                using (RegistryKey subKey = key.OpenSubKey(ksubKey))
                {
                    foreach (string subkeyname in subKey.GetValueNames())
                    {
                    }
                }
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
                                PlatformType = Platforms.EPIC,
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
                DirExists = CheckIfRegistryDirExists(UplayRegistry, "InstallDir");

                if (DirExists)
                {

                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(UplayRegistry))
                    {
                        string uplayPath = key.GetValue("InstallDir").ToString();
                        string uplayPathTrimmed = uplayPath + "games";
                        LibraryDirectories.Add(new Platform
                        {
                            PlatformType = Platforms.UPLAY,
                            Name = nameof(Platforms.UPLAY),
                            InstallationPath = uplayPathTrimmed
                        });
                    }
                }
                else
                {
                    MessageBox.Show("Could not find Uplay directories...");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool CheckIfRegistryDirExists(string key, string value)
        {
            RegistryKey r;
            bool keyExists = false;
            try
            {
                r = Registry.LocalMachine.OpenSubKey(key);
                string k = r.GetValue(value).ToString();

                if (r != null && (k != null && Directory.Exists(k)))
                    keyExists = true;
                else
                    keyExists = false;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return keyExists;
        }

        public ObservableCollection<Game> GetExecutables()
        {
            ObservableCollection<Game> games = new ObservableCollection<Game>();

            foreach (Platform libDir in LibraryDirectories)
            {
                string[] gameDirs = Directory.GetDirectories(libDir.InstallationPath);

                foreach (var gameDir in gameDirs)
                {
                    string[] exes = Directory.GetFiles(gameDir, "*.exe", SearchOption.AllDirectories);
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
