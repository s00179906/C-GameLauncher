using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

        public static void DeleteDir(string dir)
        {
            if (!String.IsNullOrEmpty(dir))
            {
                Properties.Settings.Default.FolderPaths.Remove(dir);
                Properties.Settings.Default.Save();
            }
        }

        public static void OpenGameLocation(string gameLocation)
        {
            string loc = RefactorGameLocation(gameLocation);
            if (Directory.Exists(loc))
            {
                Process.Start("explorer.exe", loc);
            }
        }

        private static string RefactorGameLocation(string gameLocation)
        {
            string[] x = gameLocation.Split('\\');
            x = x.Where(w => w != "").ToArray();
            string loc = "";
            foreach (var s in x)
            {
                loc += $"{s}\\";
            }

            return loc;
        }
    }
}
