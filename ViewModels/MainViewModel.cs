using GameLauncher.Models;
using GameLauncher.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Data;

namespace GameLauncher.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Game> _games;

        public ObservableCollection<Game> Games
        {
            get { return _games; }
            set { _games = value; OnPropertyChanged("Games"); }
        }

        public ICollectionView FilteredGames { get; set; }

        public CommandRunner AddFolderPathCommand { get; set; }
        public CommandRunner DeleteFolderPathCommand { get; private set; }
        public CommandRunner FilterSteamGamesCommand { get; private set; }
        public CommandRunner FilterUplayGamesCommand { get; private set; }
        public CommandRunner FilterBethesdaGamesCommand { get; private set; }
        public CommandRunner FilterBlizzardGamesCommand { get; private set; }
        public CommandRunner FilterOriginsGamesCommand { get; private set; }
        public CommandRunner FilterEpicGamesCommand { get; private set; }
        public GameScanner Scanner { get; set; }
        public CommandRunner LaunchGameCommand { get; private set; }
        public Game SelectedGame { get; set; }
        public Platform SelectedFolder { get; set; }


        public MainViewModel()
        {
            Games = new ObservableCollection<Game>();
           
            Scanner = new GameScanner();

            LaunchGameCommand = new CommandRunner(LaunchGame);
            AddFolderPathCommand = new CommandRunner(AddFolder);
            DeleteFolderPathCommand = new CommandRunner(DeleteFolder);
            FilterSteamGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);
            FilterEpicGamesCommand = new CommandRunner(FilterGamesByPlatformSteam);

            Scanner.Scan();

            Games = Scanner.GetExecutables();
            FilteredGames = CollectionViewSource.GetDefaultView(Games);
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }


        private void LaunchGame(object obj)
        {
            if (SelectedGame != null)
            {
                if (SelectedGame.Executables.Count == 1)
                {
                    Process.Start(SelectedGame.Executables[0]);
                }
                else
                {
                    MessageBox.Show("Game has more than one exe", "Whoops");
                }
            }
        }

        private void AddFolder(object obj)
        {
            string dirToAdd = Helper.SelectDir();

            if (!string.IsNullOrEmpty(dirToAdd))
            {
                Scanner.LibraryDirectories.Add(new Platform
                {
                    Name = "Agnostic",
                    InstallationPath = dirToAdd,
                    PlatformType = Platforms.Cracked
                });

                Games.Clear();

                foreach (var exe in Scanner.GetExecutables())
                {
                    Games.Add(exe);
                }
            }
        }

        private void DeleteFolder(object obj)
        {
            if (SelectedFolder != null)
            {
                Scanner.LibraryDirectories.Remove(SelectedFolder);
                Games.Clear();
                foreach (var exe in Scanner.GetExecutables())
                {
                    Games.Add(exe);
                }
            }
        }

        private void FilterGamesByPlatformSteam(object obj)
        {
            string Platform = obj as string;
            switch (Platform)
            {
                case "Steam":
                    FilteredGames.Filter = game => ((Game)game).Platform.Equals(Platforms.Steam);
                    break;
                case "Epic":
                    FilteredGames.Filter = game => ((Game)game).Platform.Equals(Platforms.Epic);
                    break;
                case "Uplay":
                    FilteredGames.Filter = game => ((Game)game).Platform.Equals(Platforms.UPlay);
                    break;
                default:
                    MessageBox.Show("Platform not found . . .");
                    break;
            }
        }
    }
}
