using GameLauncher.Models;
using GameLauncher.Utils;
using System.Collections.ObjectModel;

namespace GameLauncher.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<Game> Games { get; set; }
        public CommandRunner AddFolderPathCommand { get; set; }
        public GameScanner Scanner { get; set; }

        public MainViewModel()
        {
            Games = new ObservableCollection<Game>();

            AddFolderPathCommand = new CommandRunner(AddFolder);

            Scanner = new GameScanner();

            Scanner.Scan();
        }

        private void AddFolder(object obj)
        {
            Scanner.GameDirectories.Add(Helper.SelectDir());
        }
    }
}
