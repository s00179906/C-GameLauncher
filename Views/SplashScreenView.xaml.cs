using System.ComponentModel;
using System.Windows;

namespace GameLauncher.Views
{
    public partial class SplashScreenView : Window
    {
        public double Progress
        {
            get { return progressBar.Value; }
            set { progressBar.Value = value; }
        }

        public SplashScreenView()
        {
            InitializeComponent();
        }
    }
}
