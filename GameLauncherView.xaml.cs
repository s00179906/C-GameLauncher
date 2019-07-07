using GameLauncher.ViewModels;
using MahApps.Metro.Controls.Dialogs;
using System.Windows;

namespace GameLauncher.Views
{
    public partial class GameLauncherView : Window
    {
        private readonly GameLauncherViewModel vm;
        public GameLauncherView()
        {
            InitializeComponent();
            vm = new GameLauncherViewModel(DialogCoordinator.Instance, frmMainFrame);
            DataContext = vm;
        }
    }
}
