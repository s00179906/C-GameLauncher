using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GameLauncher.ViewModels;
using System.Collections.Generic;

namespace GameLauncher.Utils
{
    public class ReadACF
    {
        public string GameName { get; set; }
        public string GameID { get; set; }
        public string ACFLocationPath { get; set; }
        public string ExternalSteamLibACFPath { get; set; }
        public string[] ExternalSteamLibACFPathArr { get; set; }
        public List<string> externalLibDirectories = new List<string>();

        private readonly string Steam32 = "SOFTWARE\\VALVE\\STEAM";
        private readonly string Steam64 = "SOFTWARE\\Wow6432Node\\Valve\\STEAM";

        public ReadACF(string gameName)
        {
            GetSteamACFDirectory();
            GetACFFilesFromAllSteamLibraries(gameName);
        }

        public void GetSteamACFDirectory()
        {
            try
            {
                RegistryKey key = string.IsNullOrEmpty(Registry.LocalMachine.OpenSubKey(Steam64).ToString())
                    ? Registry.LocalMachine.OpenSubKey(Steam32)
                    : Registry.LocalMachine.OpenSubKey(Steam64);

                string path = key.GetValue("InstallPath").ToString();
                ACFLocationPath = path + "\\steamapps";
                CheckExternalSteamLibs();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void CheckExternalSteamLibs()
        {
            string externalLibPath = ACFLocationPath;
            string[] libraryFolders = Directory.GetFiles(externalLibPath, "*.vdf");

            var x = File.ReadAllLines(libraryFolders[0]);

           

            foreach (var line in x)
            {
                if (line.Contains(":\\"))
                {
                    //ExternalSteamLibACFPathArr = (string[])File.ReadLines(libraryFolders[0]).Skip(4);
                    var numerical = line;
                    var test = numerical.Split('"');
                    numerical = test[test.Length - 2];
                    externalLibDirectories.Add(numerical);
                }
            }




            //ExternalSteamLibACFPath = File.ReadLines(libraryFolders[0]).Skip(4).Take(1).First();
            ////ExternalSteamLibACFPathArr = (string[])File.ReadLines(libraryFolders[0]).Skip(4);
            //var numerical = ExternalSteamLibACFPath;
            //var test = ExternalSteamLibACFPath.Split('"');
            //ExternalSteamLibACFPath = test[test.Length - 2];

            //numerical = Regex.Match(numerical, @"\d+").Value;
            //var charsToRemove = new string[] { "\"", "\t", $"{numerical}" };

            //foreach (var c in charsToRemove)
            //    ExternalSteamLibACFPath = ExternalSteamLibACFPath.Replace(c, string.Empty);
        }

        public void GetACFFilesFromAllSteamLibraries(string gameName)
        {
            string[] acfFiles = Directory.GetFiles(ACFLocationPath, "*.acf");

            string[] externalACFFiles = new string[] { };
            foreach (var exLib in externalLibDirectories)
            {
                externalACFFiles = externalACFFiles.Concat((Directory.GetFiles(exLib + "\\steamapps", "*.acf"))).ToArray();
            }

            string[] combinedACF = acfFiles.Concat(externalACFFiles).ToArray();

            foreach (var acfFile in combinedACF)
            {
                ReadACFFile(acfFile, gameName);
            }
        }

        private void ReadACFFile(string acfFile, string _gameName)
        {
            try
            {
                var gameID = File.ReadLines(acfFile).Skip(2).Take(1).First();
                var gameName = File.ReadLines(acfFile).Skip(6).Take(1).First();

                var charsToRemove = new string[] { "\"", "\t", "installdir" };

                foreach (var c in charsToRemove)
                    gameName = gameName.Replace(c, string.Empty);

                gameID = Regex.Match(gameID, @"\d+").Value;

                GameID = gameID;
                GameName = gameName;

                if (_gameName == GameName)
                {
                    MainViewModel.GameID = int.Parse(GameID);
                    GameDetailedViewModel.GameID = int.Parse(GameID);
                }
            }
            catch
            {
                //
            }
        }
    }
}
