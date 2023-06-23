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
using System.Diagnostics;

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
        //public ChooseProfileWindow ChooseProfile;
        private BackgroundWorker bgWorker;
        private bool historyLoaded;

        public MainWindow(string username, string userPath)
        {
            InitializeComponent();

            bgWorker = new BackgroundWorker();
            dataContextVM = DataContext as MainVM;

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            historyLoaded = false;

            InitWindow(username, userPath);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Minimized;
        }

        private int newProcId;

        public int NewProcId
        {
            get { return newProcId; }
            set { newProcId = value; }
        }


        private void ProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (sender as Process != null)
            {
                if (e.Data == dataContextVM.Username)
                {
                    Process.GetProcessById(NewProcId).Kill();
                } 
            }
        }

        public void InitChooseProfile()
        {
            using (Process pProcess = new Process())
            {
                pProcess.StartInfo.FileName = "Internet Explorer++.exe";
                pProcess.StartInfo.UseShellExecute = false;
                pProcess.StartInfo.RedirectStandardOutput = true;
                pProcess.Start();
                pProcess.OutputDataReceived += ProcessOutputDataReceived;
                pProcess.BeginOutputReadLine();
                NewProcId = pProcess.Id;
            }
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

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            dataContextVM.MaxTabsScreenWidth = e.NewSize.Width - 220.0;
        }

        private void BrowserTabs_Loaded(object sender, RoutedEventArgs e)
        {
            //
        }

        private void bgWorker_LoadHistory(object sender, DoWorkEventArgs e)
        {
            if (!historyLoaded)
                dataContextVM.LoadHistory(0);
        }

        private void bgWorker_LoadBookmarks(object sender, DoWorkEventArgs e)
        {
            dataContextVM.LoadBookmarks();
        }

        private void bgWorker_LoadHistoryCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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
            bgWorker = new BackgroundWorker();
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

        private void InitWindow(string username, string userPath)
        {
            if (username != null && userPath != null)
            {
                if (dataContextVM.Username != username)
                {
                    dataContextVM.Username = username;
                    dataContextVM.UserPath = userPath;
                    dataContextVM.InitCef();

                    dataContextVM.LoadSettings();
                    StartBookmarksLoad();

                    dataContextVM.AddTabCommand.Execute(null);
                }
            }
        }

        public void ChangeVisibilityToChooseProfile()
        {
            InitChooseProfile();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            dataContextVM.SaveData();
        }
    }
}
