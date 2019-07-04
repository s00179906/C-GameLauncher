using GameLauncher.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GameLauncher.Models;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for GameDetailedView.xaml
    /// </summary>
    public partial class GameDetailedView : Page
    {
        public GameDetailedViewModel VM { get; set; }
        public GameDetailedView(Game SelectedGame)
        {
            InitializeComponent();
            VM = new GameDetailedViewModel(SelectedGame);
            DataContext = VM;
        }
    }
}
