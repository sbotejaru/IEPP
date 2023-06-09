using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
using CefSharp;
using CefSharp.Wpf;
using IEPP.Models;
using IEPP.ViewModels;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace IEPP.Views
{
    /// <summary>
    /// Interaction logic for TabContent.xaml
    /// </summary>
    public partial class TabContent : UserControl
    {
        private TabContentVM vm;
        private bool browserLoaded = false;
        private bool addressChanged = false;

        public TabContent()
        {
            InitializeComponent();
        }

        public TabContent(MainVM mainDC)
        {
            InitializeComponent();
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainDC;
        }
        public TabContent(MainVM mainDC, string url)
        {
            InitializeComponent();
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainDC;

            vm.Url = url;
        }

        public TabContent(MainVM mainDC, Settings settings)
        {
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainDC;
        }

        public TabContent(MainVM mainDC, string url, Settings settings)
        {
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainDC;
        }

        private void run_cmd()
        {
            ScriptEngine engine = Python.CreateEngine();
            var searchPaths = engine.GetSearchPaths();

            searchPaths.Add(@"D:\Anaconda\envs\dyve\Lib");
            searchPaths.Add(@"D:\Anaconda\envs\dyve\Lib\site-packages");

            engine.ExecuteFile("D:\\Faculta\\LICENTA\\Internet Explorer++\\Internet Explorer++/gan_test/gan.py");

        }

        private void UserBtn_Click(object sender, RoutedEventArgs e)
        {
            //var outputImg = new Uri("pack://application:,,,/Internet Explorer++;component/gan_test/output.png");
            //run_cmd();
            //UserTestPopup.UserIMG.Source = new BitmapImage(outputImg);


            /*if (this.UserTestPopup.Visibility == Visibility.Collapsed)
            {
                UserTestPopup.Visibility = Visibility.Visible;
            }
            else
                UserTestPopup.Visibility = Visibility.Collapsed;*/

            var mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.InitChooseProfile();
            mainWindow.ChooseProfile.UserListGrid.IsEnabled = true;
            mainWindow.ChooseProfile.IsEnabled = true;
            mainWindow.BrowserTabs.Visibility = Visibility.Collapsed;
            mainWindow.ChooseProfile.Visibility = Visibility.Visible;
        }

        private void webBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            this.SearchBar.Text = this.webBrowser.Address;
            browserLoaded = true;
            Back.GetBindingExpression(IsEnabledProperty).UpdateTarget();
            Fwd.GetBindingExpression(IsEnabledProperty).UpdateTarget();
        }

        private void webBrowser_AddressChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.SearchBar.Text = this.webBrowser.Address;
            addressChanged = true;
        }

        private void SearchBar_LostFocus(object sender, RoutedEventArgs e)
        {
            this.SearchBar.SelectionLength = 0;
            this.SearchBar.SelectionStart = 0;

            SearchBar.GotMouseCapture += SearchBar_GotMouseCapture;
        }

        private void SearchBar_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (SearchBar.SelectionLength == 0)
                this.SearchBar.SelectAll();

            SearchBar.GotMouseCapture -= SearchBar_GotMouseCapture;
        }

        private void webBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (!e.IsLoading)
            {
                ProgressBarVisible(false);
                RefreshButtonVisible(true);
            }
            else
            {
                ProgressBarVisible(true);
                RefreshButtonVisible(false);
            }
        }

        internal void RefreshButtonVisible(bool visible)
        {
            RefreshButton.Dispatcher.Invoke(() =>
            {
                RefreshButton.IsEnabled = visible;
                RefreshButton.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            });

            StopLoadButton.Dispatcher.Invoke(() =>
            {
                StopLoadButton.IsEnabled = !visible;
                StopLoadButton.Visibility = !visible ? Visibility.Visible : Visibility.Collapsed;
            });
        }
        internal void ProgressBarVisible(bool visible)
        {
            progressBar.Dispatcher.Invoke(() =>
            {
                progressBar.IsIndeterminate = visible;
                progressBar.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            });
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //(DataContext as TabContentVM).LoadMainDC();
            //if (vm.MainWinDC.Bookmarks.Count != 0)
               // BookmarkList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        public void RefreshBookmarkList()
        {
            BookmarkList.ItemsSource = vm.Bookmarks;
            BookmarkList.InvalidateVisual();
            //Console.WriteLine("refresh, doesnt do anythng");
        }

        private void MainButton_Click(object sender, RoutedEventArgs e)
        {
            MainButton.ContextMenu.IsOpen = true;
        }

        private void webBrowser_TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (browserLoaded && !vm.WentBackOrForward && addressChanged)
                vm.LogHistory(webBrowser.Address, webBrowser.Title);

            vm.WentBackOrForward = false;
            addressChanged = false;
        }

        private void BookmarkList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            BookmarkList.UnselectAll();
        }
    }
}
