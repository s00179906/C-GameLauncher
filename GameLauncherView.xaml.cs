using GameLauncher.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;
using System.Windows;

namespace GameLauncher.Views
{
    public partial class GameLauncherView : Window
    {
        public GameLauncherView()
        {
            InitializeComponent();
            DataContext = new GameLauncherViewModel(frmMainFrame, this);
        }
    }
}
