using System.Threading.Tasks;
using System.Windows;
using GameLauncher.Views;

namespace GameLauncher
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //initialize the splash screen and set it as the application main window
            var splashScreen = new SplashScreenView();
            this.MainWindow = splashScreen;
            splashScreen.Show();

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() =>
            {
                //we need to do the work in batches so that we can report progress
                for (int i = 1; i <= 100; i++)
                {
                    //simulate a part of work being done
                    System.Threading.Thread.Sleep(30);

                    //because we're not on the UI thread, we need to use the Dispatcher
                    //associated with the splash screen to update the progress bar
                    splashScreen.Dispatcher.Invoke(() => splashScreen.Progress = i);
                    splashScreen.Dispatcher.Invoke(() => splashScreen.tblckPercentage.Text = splashScreen.progressBar.Value.ToString() + "%");
                }

                //once we're done we need to use the Dispatcher
                //to create and show the main window
                this.Dispatcher.Invoke(() =>
                {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen
                    var mainWindow = new GameLauncherView();
                    this.MainWindow = mainWindow;
                    mainWindow.Show();
                    splashScreen.Close();
                });
            });
        }
    }
}
