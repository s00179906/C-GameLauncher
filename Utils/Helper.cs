using Microsoft.WindowsAPICodePack.Dialogs;
using System.Linq;
using System.Windows;

namespace GameLauncher.Utils
{
    public class Helper
    {
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
