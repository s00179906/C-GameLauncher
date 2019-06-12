using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GameLauncher.Utils
{
    public class GameScanner
    {
        public readonly string Steam32 = "SOFTWARE\\VALVE\\";
        public readonly string Steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";

        public List<string> GameDirectories { get; set; }

        public GameScanner()
        {
            GameDirectories = new List<string>();
        }

        public void Scan()
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
                            GameDirectories.Add($"{line}\\steamapps\\common");
                        }
                        GameDirectories.Add($"{steamPath}\\steamapps\\common");
                    }
                }
            }
            //string[] folders = Directory.GetDirectories(GameDirectories[0]);
            //List<string> folderNames = new List<string>();

            //foreach (var item in folders)
            //{
            //    int lastSlashIndex = item.LastIndexOf("\\");
            //    folderNames.Add(item.Substring(++lastSlashIndex, item.Length - lastSlashIndex));
            //}

            //string[] files = Directory.GetFiles(GameDirectories[0], "*.exe", SearchOption.AllDirectories);
            //ObservableCollection<Game> games = new ObservableCollection<Game>();

            //List<string> fileNames = new List<string>();

            //foreach (var file in files)
            //{
            //    int lastSlashIndex = file.LastIndexOf("\\");
            //    fileNames.Add(file.Substring(++lastSlashIndex, file.Length - lastSlashIndex));
            //}

            //foreach (var file in fileNames)
            //{
            //    foreach (var folder in folderNames)
            //    {
            //        if (file.Trim().ToLower().Contains(folder.Trim().ToLower()))
            //        {
            //            Console.WriteLine(file);
            //        }
            //    }
            //}
        }
    }
}
