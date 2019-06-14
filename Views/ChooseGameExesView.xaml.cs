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
using System.Windows.Shapes;
using GameLauncher.ViewModels;
using GameLauncher.Models;
using MahApps.Metro.Controls.Dialogs;

namespace GameLauncher.Views
{
    /// <summary>
    /// Interaction logic for ChooseGameExesView.xaml
    /// </summary>
    public partial class ChooseGameExesView : Window
    {
        public ChooseGameExesView(Game game)
        {
            InitializeComponent();
            ChooseGameExesViewModel vm = new ChooseGameExesViewModel(game, DialogCoordinator.Instance);
            this.DataContext = vm;
        }
    }
}
