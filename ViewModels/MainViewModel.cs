using GameLauncher.Models;
using GameLauncher.Utils;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace GameLauncher.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Game> Games { get; set; }
        public ObservableCollection<string> FolderPaths { get; set; }
        public CommandRunner AddFolderPathCommand { get; set; }


        public MainViewModel()
        {
            Console.WriteLine("View Model Initalized...");
            Games = new ObservableCollection<Game>();
            FolderPaths = new ObservableCollection<string>(Properties.Settings.Default.FolderPaths.Cast<string>().ToArray().Skip(1));
            AddFolderPathCommand = new CommandRunner(AddFolder);
        }

        private void AddFolder(object obj)
        {
            FolderPaths.Add(Helper.SelectDir());
        }
    }
}
