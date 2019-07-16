using GameLauncher.Utils;
using GameLauncher.Views;
using MahApps.Metro;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace GameLauncher.ViewModels
{
    public class GameLauncherViewModel : INotifyPropertyChanged
    {
        #region Public Properties
        public SettingsView SettingsView = new SettingsView();
        public static GameDetailedViewModel GameDetailedVM = new GameDetailedViewModel();
        public static Frame MainFrame { get; set; }
        public Window Window { get; set; }
        #endregion

        #region Private Properties
        private int mOuterMarginSize = 10;
        private int mWindowRadius = 10;
        #endregion

        #region WindowState Properties
        public event PropertyChangedEventHandler PropertyChanged;
        public int ResizeBorder { get; set; } = 6;
        public Thickness ResizeBorderThickness { get { return new Thickness(ResizeBorder + OuterMarginSize); } }
        public int OuterMarginSize
        {
            get
            {
                return Window.WindowState == WindowState.Maximized ? 0 : mOuterMarginSize;
            }
            set
            {
                mOuterMarginSize = value;
            }
        }
        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }
        public int WindowRadius
        {
            get
            {
                return Window.WindowState == WindowState.Maximized ? 0 : mWindowRadius;
            }
            set
            {
                mWindowRadius = value;
            }
        }
        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }
        public int TitleHeight { get; set; } = 42; 
        #endregion

        #region Commands
        public CommandRunner WindowCloseCommand { get; set; }
        public CommandRunner WindowMinimizeCommand { get; set; }
        public CommandRunner WindowMaximizeCommand { get; set; }
        public CommandRunner SettingsCommand { get; set; }
        #endregion

        #region Constructor
        public GameLauncherViewModel(Frame frmMainFrame, Window window)
        {
            Window = window;
            MainFrame = frmMainFrame;

            MainFrame.Content = new MainViewModel();
            Window.StateChanged += Window_StateChanged;

            SettingsCommand = new CommandRunner(Settings);
            WindowCloseCommand = new CommandRunner(WindowClose);
            WindowMinimizeCommand = new CommandRunner(WindowMinimize);
            WindowMaximizeCommand = new CommandRunner(WindowMaximize);

            // Fixes an resizing issue with a custom window
            WindowResizer wr = new WindowResizer(Window);

        } 
        #endregion

        #region VM Methods
        private void WindowClose(object obj)
        {
            Window.Close();
        }

        private void WindowMinimize(object obj)
        {
            Window.WindowState = WindowState.Minimized;
        }

        private void WindowMaximize(object obj)
        {
            Window.WindowState ^= WindowState.Maximized;
        }

        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }


        private void Window_StateChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }

        private void Settings(object obj)
        {
            MainFrame.Content = SettingsView;
        } 
        #endregion
    }
}
