using GameLauncher.ViewModels;
using GameLauncher.Views;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GameLauncher
{
    public partial class MainWindow : MetroWindow
    {
        private readonly GameLauncherViewModel vm;
       
        public MainWindow()
        {
            InitializeComponent();
            vm = new GameLauncherViewModel(DialogCoordinator.Instance);
            DataContext = vm;
            
        }
    }
}
