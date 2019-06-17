using GameLauncher.ViewModels;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace GameLauncher.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            MainViewModel vm = new MainViewModel(DialogCoordinator.Instance);
            DataContext = vm;
        }
    }
}
