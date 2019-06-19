using GameLauncher.ViewModels;
using System.Windows.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.ComponentModel;

namespace GameLauncher.Views
{
    public partial class MainView : Page
    {
        
        public MainView()
        {
            InitializeComponent();
            MainViewModel vm = new MainViewModel(DialogCoordinator.Instance);
            DataContext = vm;
        }
    }
}
