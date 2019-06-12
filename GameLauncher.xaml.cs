using GameLauncher.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GameLauncher
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            GameLauncherViewModel vm = new GameLauncherViewModel(DialogCoordinator.Instance);
            this.DataContext = vm;
        }
    }
}
