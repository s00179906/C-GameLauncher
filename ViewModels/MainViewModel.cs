using System;
using System.Linq;

namespace GameLauncher.ViewModels
{
    public class MainViewModel
    {

        public MainViewModel()
        {
            Console.WriteLine("View Model Initalized...");

            CheckFolderPathsExist();
        }

        private void CheckFolderPathsExist()
        {
            var folderPaths = Properties.Settings.Default.FolderPaths.Cast<string>().ToArray();
            Console.WriteLine(folderPaths.Length);

            if (folderPaths.Length < 2)
            {
                Console.WriteLine("No game folders found...");
            }
        }
    }
}
