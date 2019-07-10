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
        public MainView MainView { get; set; }

        public SettingsView SettingsView = new SettingsView();
        public static Frame MainFrame { get; set; }
        public CommandRunner SettingsCommand { get; set; }


        public GameLauncherViewModel(Frame frmMainFrame)
        {
            MainFrame = frmMainFrame;
            SettingsCommand = new CommandRunner(Settings);
            
            MainView = new MainView();

        }
        private void Settings(object obj)
        {
            MainFrame.Content = SettingsView;
        }

    }
}
