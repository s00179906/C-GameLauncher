using GameLauncher.ViewModels;
using System.Windows.Controls;
using GameLauncher.Models;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for GameDetailedView.xaml
    /// </summary>
    public partial class GameDetailedView : UserControl
    {
        public GameDetailedViewModel VM { get; set; }
        public GameDetailedView()
        {
            InitializeComponent();
            VM = new GameDetailedViewModel();
            DataContext = VM;
        }
    }
}
