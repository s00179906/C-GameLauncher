using GameLauncher.ViewModels;
using System.Windows.Controls;

namespace GameLauncher.Views
{
    public partial class MainView : UserControl
    {
        public MainViewModel VM { get; set; }
        public MainView()
        {
            InitializeComponent();
            VM = new MainViewModel();
            DataContext = VM;
        }
    }
}
