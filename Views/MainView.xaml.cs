using GameLauncher.ViewModels;
using System.Windows.Controls;

namespace GameLauncher.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
            MainViewModel vm = new MainViewModel();
            DataContext = vm;
        }
    }
}
