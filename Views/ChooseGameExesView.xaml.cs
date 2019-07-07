using System.Windows;
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
            ChooseGameExesViewModel vm = new ChooseGameExesViewModel(game);
            DataContext = vm;
        }
    }
}
