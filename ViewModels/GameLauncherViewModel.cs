using GameLauncher.Utils;
using GameLauncher.Views;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Reflection;
using System.Text;
using System.Windows;

namespace GameLauncher.ViewModels
{
    public class GameLauncherViewModel
    {
        public CommandRunner ExitCommand { get; set; }
        public CommandRunner ShowAboutCommand { get; set; }
        public CommandRunner ChangeThemeCommand { get; set; }
        public CommandRunner ChangeAccentCommand { get; set; }
        public MainView MainView { get; set; }


        private readonly IDialogCoordinator dialogCoordinator;

        public GameLauncherViewModel(IDialogCoordinator instance)
        {
            ExitCommand = new CommandRunner(Close);
            ShowAboutCommand = new CommandRunner(ShowAboutDialog);
            ChangeThemeCommand = new CommandRunner(ChangeTheme);
            ChangeAccentCommand = new CommandRunner(ChangeAccent);
            MainView = new MainView();

            dialogCoordinator = instance;
            ChangeTheme(Properties.Settings.Default["Theme"]);
        }

        /// <summary>
        /// Changes application theme while preserving current accent, updates theme in Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeTheme(object obj)
        {
            string accent = Properties.Settings.Default["Accent"].ToString();

            ThemeManager.ChangeAppStyle(Application.Current,
                       ThemeManager.GetAccent(accent),
                       ThemeManager.GetAppTheme($"Base{obj}"));

            Properties.Settings.Default["Theme"] = obj;
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Changes application accent while preserving current theme, updates accent in Properties.Settings
        /// </summary>
        /// <param name="obj"></param>
        private void ChangeAccent(object obj)
        {
            string theme = $"Base{Properties.Settings.Default["Theme"]}";

            ThemeManager.ChangeAppStyle(Application.Current,
                       ThemeManager.GetAccent(obj as string),
                       ThemeManager.GetAppTheme(theme));

            Properties.Settings.Default["Accent"] = obj;
            Properties.Settings.Default.Save();
        }

        private void Close(object obj)
        {
            Application.Current.MainWindow.Close();
        }

        /// <summary>
        /// Shows dialogue with Application Information.
        /// </summary>
        /// <param name="obj"></param>
        private async void ShowAboutDialog(object obj)
        {
            string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            DateTime now = DateTime.Now;
            StringBuilder sb = new StringBuilder();

            //Build the text for the dialog message
            sb.AppendLine("Game Launcher");
            sb.AppendLine("Copyright © " + now.Year + " hignz(fackaboiii) + ibzmannnnnnnnnn(anele)");
            sb.AppendLine("Version: " + version);
            sb.Append(Environment.NewLine);
            sb.AppendLine("Licensed under GPL 2.0");
            sb.Append(Environment.NewLine);
            sb.AppendLine("This program is free software; you can redistribute it and/or modify " +
                "it under the terms of the GNU General Public License as published by " +
                "the Free Software Foundation; either version 2 of the License, or " +
                "any later version.");
            sb.Append(Environment.NewLine);
            sb.AppendLine("This program is distributed in the hope that it will be useful, " +
                "but WITHOUT ANY WARRANTY; without even the implied warranty of " +
                "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the " +
                "GNU General Public License for more details.");

            //Show the dialog
            await dialogCoordinator.ShowMessageAsync(this, "About", sb.ToString());
        }

    }
}
