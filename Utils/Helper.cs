using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using System.Windows;

namespace GameLauncher.Utils
{
    public class Helper
    {
        public static void CheckGameFoldersExist()
        {
            string[] folderPaths = Properties.Settings.Default.FolderPaths.Cast<string>().ToArray();
            
            // Used for testing
            // Properties.Settings.Default.Reset();

            if (folderPaths.Length < 2)
            {
                if (MessageBox.Show("Please select a game folder", "No game folders found") == MessageBoxResult.OK)
                {
                    Properties.Settings.Default.FolderPaths.Add(SelectDir());
                    Properties.Settings.Default.Save();
                }
            }
        }

        public static string SelectDir()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                Properties.Settings.Default.FolderPaths.Add(dialog.FileName);
                Properties.Settings.Default.Save();
                return dialog.FileName;
            }

            return string.Empty;
        }
    }
}
