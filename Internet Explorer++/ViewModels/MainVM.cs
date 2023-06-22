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
using System.Windows.Markup;

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

        private int selectedTabIndex;
        private bool settingsTabOpen = false;
        private string workingDir;
        private string usersDir;
        private string cacheDir;
        private double defaultMaxTabWidth = 200.0;

        public int SettingsTabIndex { get; set; }

        public Settings SettingsData { get; set; }

        public JsonHelper JsonHelper { get; set; }

        private bool bookmarkDeleted;

        public bool BookmarkDeleted
        {
            get { return bookmarkDeleted; }
            set { bookmarkDeleted = value; }
        }

        private bool historyDeleted;
        public bool HistoryDeleted
        {
            get { return historyDeleted; }
            set { historyDeleted = value; }
        }

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
                if (selectedTabIndex != value && selectedTabIndex != -1)
                    if (selectedTabIndex < Tabs.Count)
                        Tabs[selectedTabIndex].RemoveBookmarkUI();

                if (selectedTabIndex != value)
                {
                    SetSeparatorVisibilities(selectedTabIndex, value);
                    selectedTabIndex = value;

                    NotifyPropertyChanged("SelectedTabIndex");
                }

                if (selectedTabIndex != -1)
                    Tabs[selectedTabIndex].RefreshBookmarks();
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

        public bool FromBookmarksSettings { get; set; }

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
            bool exists = Directory.Exists(UsersDir);

            if (!exists)
                Directory.CreateDirectory(UsersDir);
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

        public void AddSettingsTab(int tabIndex)
        {
            if (!settingsTabOpen)
            {
                var newTab = new BrowserTab(this, tabIndex)
                {
                    Title = "IEPP Settings"
                };

                Tabs.Add(newTab);

                SelectedTabIndex = Tabs.Count - 1;
                SettingsTabIndex = SelectedTabIndex;

                if (MaxTabWidth * Tabs.Count > MaxTabsScreenWidth)
                    ResizeTabs();

                settingsTabOpen = true;
            }
            else
            {
                SelectedTabIndex = SettingsTabIndex;
                Tabs[SettingsTabIndex].UpdateSettingsTab(tabIndex);
            }
        }

        private void UpdateSettingsTabIndex()
        {
            for (int index = 0; index < Tabs.Count; ++index)
            {
                if (Tabs[index].IsSettingsTab())
                    SettingsTabIndex = index;
            }
        }

        public void AddBookmark(Bookmark newBookmark)
        {
            if (Username == "")
                return;

            Bookmarks.Add(newBookmark.ToContainer());
            CurrentSessionBookmarksData.Add(newBookmark);
            if (settingsTabOpen)
                BookmarksSettings.Add(newBookmark.ToContainer().ToHistoryContainer());
        }

        public void AddHistoryItem(HistoryItem newHistoryItem)
        {
            if (Username == "")
                return;

            CurrentSessionHistoryData.Insert(0, newHistoryItem);

            if (CurrentSessionHistory.Count != 0)
                CurrentSessionHistory.Insert(0, newHistoryItem.ToContainer());
        }

        private void ResizeTabs()
        {
            MaxTabWidth = MaxTabsScreenWidth / Tabs.Count;
        }

        public void LoadBookmarks()
        {
            SavedBookmarksData = JsonHelper.ReadAllBookmarks();
        }

        public void BookmarkDataToUI()
        {
            if (Username == "")
                return;

            if (SavedBookmarksData != null)
            {
                foreach (var bookmark in SavedBookmarksData)
                {
                    Bookmarks.Add(bookmark.ToContainer());
                }

                SavedBookmarksData.Clear();
            }
        }

        public void BookmarksToSettingsContainer()
        {
            foreach (var bm in Bookmarks)
            {
                BookmarksSettings.Add(bm.ToHistoryContainer());
            }
        }

        public void LoadHistory(int scrollNumber)
        {
            if (Username == "")
                return;

            if (scrollNumber == 0)
                SavedHistoryData = JsonHelper.ReadPartialHistory();
            else
            {
                JsonHelper.StartRange = scrollNumber * JsonHelper.ItemsNumber + 1; // adding 1 because previous last item will be the next first item, so we skip it
                var temp = JsonHelper.ReadPartialHistory();

                if (temp != null)
                    TemporaryHistoryData.AddRange(temp);
            }
        }

        public async Task LoadTemporaryHistoryData()
        {
            await LoadHistoryDataToUI(TemporaryHistoryData, false);
        }

        public async Task LoadHistoryDataToUI(List<HistoryItem> historyItems, bool isCurrentSessionData)
        {
            int batchSize = 5; // Number of items to process per batch
            int currentIndex = 0; // Current index of HistoryData being processed

            while (currentIndex < historyItems.Count)
            {
                // Get the next batch of HistoryData to process
                var batchData = historyItems.Skip(currentIndex).Take(batchSize).ToList();

                // Process the batch of HistoryData asynchronously
                await Task.Run(() =>
                {
                    // Update the UI on the UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var batchUI = new List<HistoryItemContainer>();

                        // Convert the batchData to HistoryUI elements
                        foreach (var historyItem in batchData)
                        {
                            // Convert historyItem to HistoryUI and add it to the batchUI list
                            // ...
                            if (!DeletedHistoryData.Any(item => item.Url == historyItem.Url && item.BrowseDate == historyItem.BrowseDate))
                                batchUI.Add(historyItem.ToContainer());
                        }

                        // Add the batchUI elements to the ObservableCollection
                        foreach (var historyUI in batchUI)
                        {
                            if (isCurrentSessionData)
                                CurrentSessionHistory.Add(historyUI);
                            else
                                History.Add(historyUI);
                        }
                    });
                });

                // Increase the currentIndex to process the next batch
                currentIndex += batchSize;

                // Delay to allow the UI to remain responsive
                await Task.Delay(0); // Adjust the delay duration as needed
            }
        }

        public void DeleteHistoryItem(HistoryItemContainer hic)
        {
            if (FromBookmarksSettings)
                foreach (var bc in Bookmarks)
                {
                    if (bc == hic.ToBookmarkContainer())
                    {
                        Bookmarks.Remove(bc);
                        BookmarksSettings.Remove(hic);
                        BookmarkDeleted = true;
                        return;
                    }
                }
            else
            {
                if (CurrentSessionHistory.Any(item => item.Url == hic.Url && item.BrowseDate == hic.BrowseDate))
                {
                    CurrentSessionHistory.Remove(hic);
                    foreach (var hd in CurrentSessionHistoryData)
                    {
                        if (hd.Url == hic.Url && hd.BrowseDate == hic.BrowseDate)
                        {
                            CurrentSessionHistoryData.Remove(hd);
                            break;
                        }
                    }
                    HistoryDeleted = true;
                }
                else
                {
                    DeletedHistoryData.Add(hic.ToModel());
                    foreach (var history in History)
                    {
                        if (history.Url == hic.Url && history.BrowseDate == hic.BrowseDate)
                        {
                            History.Remove(history);
                            break;
                        }
                    }
                    HistoryDeleted = true;
                }
            }
        }

        public void LoadSettings()
        {
            SettingsData = JsonHelper.ReadSettings();
        }

        public void ApplySettingsChanges()
        {
            foreach (var tab in Tabs)
            {
                tab.ApplySettingsChanges(SettingsData);
            }
        }

        private void SaveHistoryData()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BgWorker_SaveHistory;
            bw.RunWorkerAsync();
        }

        private void SaveBookmarkData()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BgWorker_SaveBookmarks;
            bw.RunWorkerAsync();
        }

        public void SaveSettingsData()
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += BgWorker_SaveSettings;
            bw.RunWorkerAsync();
        }

        private void BgWorker_SaveHistory(object sender, DoWorkEventArgs e)
        {
            if (CurrentSessionHistoryData.Count == 0 && !HistoryDeleted)
                return;

            var savedHistoryList = JsonHelper.ReadAllHistory();

            if (savedHistoryList == null || savedHistoryList.Count == 0)
            {
                JsonHelper.Save(CurrentSessionHistoryData, "history");
                return;
            }

            var x = DeletedHistoryData.Count;
            while (x != 0)
            {
                foreach (var history in savedHistoryList)
                {
                    if (DeletedHistoryData.Count != 0 && DeletedHistoryData.Any(item => item.Url == history.Url && item.BrowseDate == history.BrowseDate))
                    {
                        savedHistoryList.Remove(history);
                        break;
                    }
                }
                --x;
            }

            CurrentSessionHistoryData.AddRange(savedHistoryList);
            savedHistoryList.Clear();
            JsonHelper.Save(CurrentSessionHistoryData, "history");
            CurrentSessionHistoryData.Clear();
        }

        private void BgWorker_SaveBookmarks(object sender, DoWorkEventArgs e)
        {
            if (CurrentSessionBookmarksData.Count == 0 && !BookmarkDeleted)
                return;

            App.Current.Dispatcher.Invoke(() => JsonHelper.Save(Bookmarks, "bookmarks"));
            //Bookmarks.Clear();
        }

        private void BgWorker_SaveSettings(object sender, DoWorkEventArgs e)
        {
            if (SettingsData != null)
            {
                JsonHelper.Save(SettingsData, "settings");
                SettingsData = null;
            }
        }

        public void InitCef()
        {
            CefSettings settings = new CefSettings();

            if (UserPath != "")
                settings.CachePath = UserPath + "/cache";

            //settings.CefCommandLineArgs.Add("disable-threaded-scrolling", "1");
            settings.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36 /CefSharp Browser" + Cef.CefSharpVersion;
            Cef.Initialize(settings);
        }

        private void Init()
        {
            BrowserVis = Visibility.Collapsed;
            Tabs = new ObservableCollection<BrowserTab>();
            Bookmarks = new ObservableCollection<BookmarkContainer>();
            BookmarksSettings = new ObservableCollection<HistoryItemContainer>();
            History = new ObservableCollection<HistoryItemContainer>();
            CurrentSessionHistory = new ObservableCollection<HistoryItemContainer>();
            SavedBookmarksData = new List<Bookmark>();
            CurrentSessionBookmarksData = new List<Bookmark>();
            SavedHistoryData = new List<HistoryItem>();
            CurrentSessionHistoryData = new List<HistoryItem>();
            TemporaryHistoryData = new List<HistoryItem>();
            DeletedHistoryData = new List<HistoryItem>();
            BookmarkDeleted = false;
            HistoryDeleted = false;
            MaxTabWidth = defaultMaxTabWidth;
            FromBookmarksSettings = true;
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
        public ObservableCollection<HistoryItemContainer> BookmarksSettings { get; set; }
        public ObservableCollection<HistoryItemContainer> History { get; set; }
        public ObservableCollection<HistoryItemContainer> CurrentSessionHistory { get; set; }
        public List<Bookmark> SavedBookmarksData { get; set; }
        public List<Bookmark> CurrentSessionBookmarksData { get; set; }
        public List<HistoryItem> SavedHistoryData { get; set; }
        public List<HistoryItem> TemporaryHistoryData { get; set; }
        public List<HistoryItem> CurrentSessionHistoryData { get; set; }
        public List<HistoryItem> DeletedHistoryData { get; set; }

        public MainVM()
        {
            Init();

            CloseCommand = new RelayCommand(o =>
            {
                foreach (var tab in Tabs)
                    tab.Dispose();

                if (UserPath != "")
                {
                    SaveBookmarkData();
                    SaveSettingsData();
                    SaveHistoryData();                    
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

                if (Tabs[closedIndex].Content as SettingsControl != null)
                {
                    settingsTabOpen = false;
                    SettingsTabIndex = -1;
                    History.Clear();
                    CurrentSessionHistory.Clear();
                    BookmarksSettings.Clear();
                    JsonHelper.NoMoreHistoryItems = false;
                }

                Tabs[closedIndex].Dispose();
                Tabs.Remove(closedTab);

                if (Tabs.Count == 0)
                    Dispose();

                if (MaxTabWidth != defaultMaxTabWidth)
                    ResizeTabs();

                if (settingsTabOpen)
                    UpdateSettingsTabIndex();
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
