using CefSharp.Wpf;
using CefSharp;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using IEPP.Controls;
using IEPP.ViewModels;
using System.ComponentModel;
using System.Threading;

namespace IEPP.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Uri restoreDownSource = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/restore_down.png");
        Uri maximizeSource = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/maximize.png");
        MainVM dataContextVM;
        public ChooseProfile ChooseProfile;
        private BackgroundWorker bgWorker;
        private bool historyLoaded;

        public MainWindow()
        {
            InitializeComponent();

            InitChooseProfile();

            bgWorker = new BackgroundWorker();
            dataContextVM = DataContext as MainVM;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            historyLoaded = false;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Minimized;
        }

        /* private void Button_Click(object sender, RoutedEventArgs e)
         {
             this.WindowState = WindowState.Maximized;
         }*/

        public void InitChooseProfile()
        {
            ChooseProfile = new ChooseProfile();
            ChooseProfile.IsVisibleChanged += ChooseProfileUC_IsVisibleChanged;
            ChooseProfile.Loaded += ChooseProfileUC_Loaded;
            ChooseProfile.IsEnabledChanged += ChooseProfileUC_IsEnabledChanged;

            ChooseProfileGrid.Children.Add(ChooseProfile);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.MaxBtnIcon.Source = new BitmapImage(restoreDownSource);
                this.MainGrid.Margin = new Thickness(6);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.MaxBtnIcon.Source = new BitmapImage(maximizeSource);
                this.MainGrid.Margin = new Thickness(0);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            /*Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle, 
                new Action(() => WindowStyle = WindowStyle.None)
                );*/
        }

        private void ChooseProfileUC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            /*if (e.NewValue.ToString() == "False")
            {
                var chooseProfileDC = ChooseProfileUC.DataContext as ChooseProfileVM;

                if (chooseProfileDC.SelectedUser != null)
                {
                    dataContextVM.Username = chooseProfileDC.SelectedUser.Username;
                    dataContextVM.UserPath = chooseProfileDC.CurrentUserPath;
                    dataContextVM.AddTabCommand.Execute(null);
                }

                BrowserTabs.Visibility = Visibility.Visible;
            }
            else if (e.NewValue.ToString() == "True")
                BrowserTabs.Visibility = Visibility.Collapsed;*/
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dataContextVM.MaxTabsScreenWidth = e.NewSize.Width - 220.0;
        }

        private void BrowserTabs_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }

        private void ChooseProfileUC_Loaded(object sender, RoutedEventArgs e)
        {
            ChooseProfile.UsersDir = dataContextVM.UsersDir;
        }

        private void bgWorker_LoadHistory(object sender, DoWorkEventArgs e)
        {
            if (!historyLoaded)
                dataContextVM.LoadHistory();
        }

        private void bgWorker_LoadBookmarks(object sender, DoWorkEventArgs e)
        {
            dataContextVM.LoadBookmarks();
        }

        private void bgWorker_LoadHistoryCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            /*Dispatcher.Invoke(() =>
            {
                dataContextVM.HistoryDataToUI();
                dataContextVM.GetSettings().LoadHistoryUI();
            });*/

            historyLoaded = true;
        }

        private void bgWorker_LoadBookmarksCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                dataContextVM.BookmarkDataToUI();
            });

            bgWorker = null;
            StartHistoryLoad();
        }

        private void StartBookmarksLoad()
        {
            bgWorker.DoWork += bgWorker_LoadBookmarks;
            bgWorker.RunWorkerCompleted += bgWorker_LoadBookmarksCompleted;
            bgWorker.RunWorkerAsync();
        }

        public void StartHistoryLoad()
        {
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += bgWorker_LoadHistory;
            bgWorker.RunWorkerCompleted += bgWorker_LoadHistoryCompleted;
            bgWorker.RunWorkerAsync();
        }

        private void ChooseProfileUC_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.ToString() == "False")
            {
                var chooseProfileDC = ChooseProfile.DataContext as ChooseProfileVM;

                if (chooseProfileDC.SelectedUser != null)
                {
                    if (dataContextVM.Username != chooseProfileDC.SelectedUser.Username)
                    {
                        dataContextVM.Username = chooseProfileDC.SelectedUser.Username;
                        dataContextVM.UserPath = chooseProfileDC.CurrentUserPath;
                        dataContextVM.InitCef();

                        StartBookmarksLoad();

                        dataContextVM.AddTabCommand.Execute(null);
                        this.ChooseProfile = null;
                    }
                }

                BrowserTabs.Visibility = Visibility.Visible;
            }
            else if (e.NewValue.ToString() == "True")
            {
                //InitChooseProfile();
                BrowserTabs.Visibility = Visibility.Collapsed;
            }
        }
    }
}
