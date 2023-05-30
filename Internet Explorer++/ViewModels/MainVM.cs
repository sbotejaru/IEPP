using CefSharp.Wpf;
using CefSharp;
using IEPP.Utils;
using IEPP.Enums;
using Microsoft.Scripting.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ChromeTabs;
using IEPP.Views;
using System.IO.Pipes;
using System.Windows.Controls;
using IEPP.Controls;
using System.Windows.Media.Imaging;
using System.Diagnostics.PerformanceData;
using System.Collections.Specialized;

namespace IEPP.ViewModels
{
    //netron.app
    public class MainVM : INotifyPropertyChanged
    {
        #region PropertyChanged declaration

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        //private string startUrl = "https://google.com";
        private int selectedTabIndex;
        private bool settingsTabOpen = false;
        //private string workingDir;
        private double defaultMaxTabWidth = 200.0;

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; NotifyPropertyChanged("Username"); Console.WriteLine(Username); }
        }

        private Visibility browserVis;
        public Visibility BrowserVis
        {
            get { return browserVis; }
            set { browserVis = value; NotifyPropertyChanged("BrowserVis"); }
        }

        public int SelectedTabIndex
        {
            get { return selectedTabIndex; }
            set
            {
                SetSeparatorVisibilities(selectedTabIndex, value);
                selectedTabIndex = value;
                NotifyPropertyChanged("SelectedTabIndex");
            }
        }

        private double maxTabsScreenWidth;
        public double MaxTabsScreenWidth
        {
            get { return maxTabsScreenWidth; }
            set { maxTabsScreenWidth = value; NotifyPropertyChanged("MaxTabsScreenWidth"); ResizeTabs(); }
        }

        private double maxTabWidth;
        public double MaxTabWidth
        {
            get { return maxTabWidth; }
            set
            {
                maxTabWidth = value > defaultMaxTabWidth ? defaultMaxTabWidth : value;
                Console.WriteLine(maxTabWidth);

                NotifyPropertyChanged("MaxTabWidth");
            }
        }


        private void SetSeparatorVisibilities(int oldIndex, int newIndex)
        {
            if (newIndex == -1 || oldIndex == -1)
                return;

            if (oldIndex == newIndex)
                return;

            if (newIndex == 0 && oldIndex != 0)
            {
                Tabs[oldIndex - 1].SeparatorVisibility = Visibility.Visible;
            }
            else if (oldIndex == newIndex - 1)
            {
                Tabs[oldIndex].SeparatorVisibility = Visibility.Hidden;
                if (oldIndex != 0)
                    Tabs[oldIndex - 1].SeparatorVisibility = Visibility.Visible;
            }
            else if (oldIndex != 0)
            {
                Tabs[oldIndex - 1].SeparatorVisibility = Visibility.Visible;
                Tabs[newIndex - 1].SeparatorVisibility = Visibility.Hidden;
            }
            else if (oldIndex == 0 && newIndex != oldIndex + 1)
            {
                Tabs[newIndex - 1].SeparatorVisibility = Visibility.Hidden;
            }
        }

        private void Dispose()
        {
            Cef.Shutdown();
            Application.Current.Shutdown();
        }

        private void AddBrowserTab()
        {
            var newTab = new BrowserTab(TabType.Browser);

            newTab.FavIconSource = null;
            newTab.Title = "New Tab";

            Tabs.Add(newTab);
        }

        private void AddSettingsTab()
        {
            var newTab = new BrowserTab(TabType.Settings);
            var icon = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/IEPP_gray.ico");

            newTab.FavIconSource = new BitmapImage(icon);
            newTab.Title = "IEPP Settings";

            Tabs.Add(newTab);
        }

        private void ResizeTabs()
        {
            double result = MaxTabsScreenWidth / (double)Tabs.Count;
            MaxTabWidth = result;
        }

        public RelayCommand CloseCommand { get; set; }
        public RelayCommand MaximizeCommand { get; set; }
        public RelayCommand AddTabCommand { get; set; }
        public RelayCommand AddSettingsTabCommand { get; set; }
        public RelayCommand<BrowserTab> CloseTabCommand { get; set; }

        public ObservableCollection<BrowserTab> Tabs { get; set; }
        public ObservableCollection<BookmarkContainer> Bookmarks { get; set; }

        public MainVM()
        {
            BrowserVis = Visibility.Collapsed;
            Tabs = new ObservableCollection<BrowserTab>();
            Bookmarks = new ObservableCollection<BookmarkContainer>();
            MaxTabWidth = defaultMaxTabWidth;

            CloseCommand = new RelayCommand(o =>
            {
                foreach (var tab in Tabs)
                {
                    //if (tab.Content as) DACA E WEBBROWSER
                    tab.Dispose();
                }

                Dispose();
            });

            MaximizeCommand = new RelayCommand(o =>
            {
                bool notMaximized = Application.Current.MainWindow.WindowState != WindowState.Maximized;

                if (notMaximized)
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                else
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
            });

            CloseTabCommand = new RelayCommand<BrowserTab>((closedTab) =>
            {
                int closedIndex = Tabs.IndexOf(closedTab);

                if (Tabs[closedIndex].Content as TabContent == null) // TO BE CHANGED as Settings != null
                    settingsTabOpen = false;

                Tabs[closedIndex].Dispose();
                Tabs.Remove(closedTab);

                if (Tabs.Count == 0)
                    Dispose();

                ResizeTabs();
            });

            AddTabCommand = new RelayCommand(o =>
            {
                AddBrowserTab();
                SelectedTabIndex = Tabs.Count - 1;

                if (MaxTabWidth * Tabs.Count > MaxTabsScreenWidth)
                    ResizeTabs();
            });

            AddSettingsTabCommand = new RelayCommand(o =>
            {
                if (!settingsTabOpen)
                {
                    AddSettingsTab();
                    SelectedTabIndex = Tabs.Count - 1;
                    settingsTabOpen = true;
                }
            });
        }
    }
}
