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
using IEPP.Models;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;

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
        private string workingDir;
        private string usersDir;
        private string cacheDir;
        private double defaultMaxTabWidth = 200.0;

        public int SettingsTabIndex { get; set; }

        public JsonHelper JsonHelper { get; set; }

        public string CacheDir
        {
            get => cacheDir;
            set => cacheDir = value;
        }

        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; NotifyPropertyChanged("Username"); }
        }

        public string UsersDir
        {
            get { return usersDir; }
            set { usersDir = value; NotifyPropertyChanged("UsersDir"); }
        }

        private string userPath;
        public string UserPath
        {
            get { return userPath; }
            set { userPath = value; JsonHelper = new JsonHelper(userPath); }
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

                foreach (var tab in Tabs)
                {
                    tab.RemoveBookmarkUI();
                }

                if (selectedTabIndex != -1)
                    Tabs[selectedTabIndex].RefreshBookmarks();

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
                NotifyPropertyChanged("MaxTabWidth");
            }
        }

        private void CreateAppDirectory()
        {
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            string subPath = "/Internet Explorer++";
            workingDir = docsFolder + subPath;

            bool exists = Directory.Exists(workingDir);

            if (!exists)
            {
                Directory.CreateDirectory(workingDir);
            }
        }

        private void CreateCacheDirectory()
        {
            CacheDir = workingDir + "/Cache";
            bool exists = Directory.Exists(CacheDir);

            if (!exists)
                Directory.CreateDirectory(CacheDir);
        }

        private void CreateUsersDirectory()
        {
            UsersDir = workingDir + "/Users";
            bool exists = Directory.Exists(usersDir);

            if (!exists)
                Directory.CreateDirectory(usersDir);
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
            var newTab = new BrowserTab(TabType.Browser, this)
            {
                FavIconSource = null,
                Title = "New Tab"
            };

            Tabs.Add(newTab);
        }

        public void AddBrowserTab(string url)
        {
            var newTab = new BrowserTab(this, url)
            {
                FavIconSource = null,
                Title = "New Tab"
            };

            Tabs.Add(newTab);

            SelectedTabIndex = Tabs.Count - 1;

            if (MaxTabWidth * Tabs.Count > MaxTabsScreenWidth)
                ResizeTabs();
        }

        private void AddSettingsTab()
        {
            var newTab = new BrowserTab(TabType.Settings, this)
            {
                Title = "IEPP Settings"
            };

            Tabs.Add(newTab);
        }

        public SettingsControl GetSettings()
        {
            if (SettingsTabIndex != -1 && settingsTabOpen != false)
                return Tabs[SettingsTabIndex].Content as SettingsControl;

            return null;
        }

        public void AddBookmark(Bookmark newBookmark)
        {
            Bookmarks.Add(newBookmark.ToContainer(cacheDir));
        }

        public void AddHistoryItem(HistoryItem newHistoryItem)
        {
            HistoryData.Insert(0, newHistoryItem);
        }

        private void ResizeTabs()
        {
            MaxTabWidth = MaxTabsScreenWidth / Tabs.Count;
        }        

        public void LoadBookmarks()
        {
            BookmarksData = JsonHelper.ReadAllBookmarks();
        }

        public void BookmarkDataToUI()
        {
            foreach (var bookmark in BookmarksData)
            {
                Bookmarks.Add(bookmark.ToContainer(CacheDir));
            }

            BookmarksData.Clear();
        }

        public void HistoryDataToUI()
        {
            /*Thread t = new Thread(() =>
            {
                foreach (var historyItem in HistoryData)
                {
                    History.Add(historyItem.ToContainer());
                }
            });

            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            *//*await Task.Run(() =>
            {
                foreach (var historyItem in HistoryData)
                {
                    History.Add(historyItem.ToContainer());
                }
            });*//*            

            //HistoryData.Clear();*/
        }

        public void LoadHistory()
        {
            JsonHelper.StartRange = 80;
            HistoryData = JsonHelper.ReadPartialHistory();
        }

        public void LoadUserData()
        {
            LoadBookmarks();
            LoadHistory();
            //
        }

        public void InitCef()
        {
            CefSettings settings = new CefSettings();

            if (UserPath != "")
                settings.CachePath = UserPath + "/cache";

            settings.CefCommandLineArgs.Add("disable-threaded-scrolling", "1");
            Cef.Initialize(settings); // closing from chooseprofile after selecting user first time crashes ??? not anymore :)
        }

        private void Init()
        {
            BrowserVis = Visibility.Collapsed;
            Tabs = new ObservableCollection<BrowserTab>();
            Bookmarks = new ObservableCollection<BookmarkContainer>();
            History = new ObservableCollection<HistoryItemContainer>();
            BookmarksData = new List<Bookmark>();
            HistoryData = new List<HistoryItem>();
            MaxTabWidth = defaultMaxTabWidth;
            SettingsTabIndex = -1;
            CreateAppDirectory();
            CreateCacheDirectory();
            CreateUsersDirectory();
        }

        public RelayCommand CloseCommand { get; set; }
        public RelayCommand MaximizeCommand { get; set; }
        public RelayCommand AddTabCommand { get; set; }
        public RelayCommand AddSettingsTabCommand { get; set; }
        public RelayCommand<BrowserTab> CloseTabCommand { get; set; }

        public ObservableCollection<BrowserTab> Tabs { get; set; }
        public ObservableCollection<BookmarkContainer> Bookmarks { get; set; }
        public ObservableCollection<HistoryItemContainer> History { get; set; }
        public List<Bookmark> BookmarksData { get; set; }
        public List<HistoryItem> HistoryData { get; set; }

        public MainVM()
        {
            Init();

            CloseCommand = new RelayCommand(o =>
            {
                foreach (var tab in Tabs)
                    tab.Dispose();

                //JsonHelper.Save(BookmarksData, "bookmarks");
                //JsonHelper.Save(HistoryData, "history");

                this.Dispose();
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

                if (Tabs[closedIndex].Content as SettingsControl != null)
                {
                    settingsTabOpen = false;
                    SettingsTabIndex = -1;
                    History.Clear();
                }

                Tabs[closedIndex].Dispose();
                Tabs.Remove(closedTab);

                if (Tabs.Count == 0)
                    this.Dispose();

                if (MaxTabWidth != defaultMaxTabWidth)
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

                    SettingsTabIndex = SelectedTabIndex;

                    if (MaxTabWidth * Tabs.Count > MaxTabsScreenWidth)
                        ResizeTabs();

                    settingsTabOpen = true;
                }
            });
        }
    }
}
