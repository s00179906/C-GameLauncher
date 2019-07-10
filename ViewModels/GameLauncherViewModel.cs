using GameLauncher.Utils;
using GameLauncher.Views;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GameLauncher.ViewModels
{
    public class GameLauncherViewModel
    {
        public SettingsView SettingsView = new SettingsView();
        public static Frame MainFrame { get; set; }
        public CommandRunner SettingsCommand { get; set; }
        public GameLauncherViewModel(Frame frmMainFrame)
        {
            MainFrame = frmMainFrame;
            SettingsCommand = new CommandRunner(Settings);

            MainFrame.Content = new MainViewModel();

        }
        private void Settings(object obj)
        {
            MainFrame.Content = SettingsView;
        }

    }
}
