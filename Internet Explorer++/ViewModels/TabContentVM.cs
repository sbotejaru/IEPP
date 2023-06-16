using CefSharp;
using CefSharp.Wpf;
using IEPP.Controls;
using IEPP.Utils;
using IEPP.Views;
using IEPP.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Nager.PublicSuffix;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using IEPP.Enums;
using System.Dynamic;

namespace IEPP.ViewModels
{
    public class TabContentVM : INotifyPropertyChanged
    {
        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private readonly string isSecureIconPath = "/Icons/lock.png";
        private readonly string isNotSecureIconPath = "/Icons/warning.png";
        private readonly string isBookmarkedIconPath = "/Icons/star_full.png";
        private readonly string isNotBookmarkedIconPath = "/Icons/star.png";
        private string searchBarText;
        private readonly Regex urlRegex = new Regex("^[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        private readonly Regex secureUrlRegex = new Regex("^https:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        private readonly Regex httpUrlRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        private void Search(string searchUrl)
        {
            switch (CurrentSearchEngine)
            {
                case SearchEngine.Google:
                    string googleSearch = "https://google.com/search?q=";
                    WebBrowser.Load(googleSearch + searchUrl);
                    break;
                case SearchEngine.Bing:
                    string bingSearch = "https://www.bing.com/search?q=";
                    WebBrowser.Load(bingSearch + searchUrl);
                    break;
                case SearchEngine.DuckDuckGo:
                    string duckSearch = "https://duckduckgo.com/?q=";
                    WebBrowser.Load(duckSearch + searchUrl);
                    break;
                case SearchEngine.Yandex:
                    string yandexSearch = "https://yandex.com/search/?text=";
                    WebBrowser.Load(yandexSearch + searchUrl);
                    break;
                case SearchEngine.Yahoo:
                    string yahooSearch = "https://search.yahoo.com/search?p=";
                    WebBrowser.Load(yahooSearch + searchUrl);
                    break;
                default:
                    string googleSearch2 = "https://google.com/search?q=";
                    WebBrowser.Load(googleSearch2 + searchUrl);
                    break;
            }
        }

        private string GetSearchEngineURL()
        {
            BookmarkBarVisibility = Visibility.Visible;

            switch (CurrentSearchEngine)
            {
                case SearchEngine.Google:
                    return "https://www.google.com/";
                case SearchEngine.Bing:
                    return "https://www.bing.com/";
                case SearchEngine.DuckDuckGo:
                    return "https://duckduckgo.com/";
                case SearchEngine.Yandex:
                    return "https://yandex.com/";
                case SearchEngine.Yahoo:
                    return "https://search.yahoo.com/";
                default:
                    return "https://www.google.com/";
            }
        }

        public ChromiumWebBrowser WebBrowser { get; set; }

        private BookmarkContainer selectedBookmark;
        public BookmarkContainer SelectedBookmark
        {
            get { return selectedBookmark; }
            set
            {
                selectedBookmark = value;

                if (selectedBookmark != null)
                {
                    WebBrowser.Load(selectedBookmark.Url);
                    SelectedBookmark = null;
                }
            }
        }

        private int selectedSettingsTab;
        public int SelectedSettingsTab
        {
            get { return selectedSettingsTab; }
            set
            {
                selectedSettingsTab = value;
                NotifyPropertyChanged("SelectedSettingsTab");
            }
        }

        public void CheckBookmarkedAddress(string url)
        {
            BookmarkedIconPath = IsBookmarked(url) ? isBookmarkedIconPath : isNotBookmarkedIconPath;
        }

        public void CheckSecureAddress(string url)
        {
            if (secureUrlRegex.IsMatch(url))
            {
                SecureIconPath = isSecureIconPath;
                SecureIconToolTip = "Website is secure.";
            }
            else
            {
                SecureIconPath = isNotSecureIconPath;
                SecureIconToolTip = "Website is not secure.";
            }
        }

        private bool IsBookmarked(string url)
        {
            foreach (var bookmark in Bookmarks)
            {
                if (bookmark.Url == url)
                {
                    return true;
                }
            }

            return false;
        }

        public Settings CurrentSettings
        {
            get { return MainWinDC.SettingsData; }
            set
            {
                MainWinDC.SettingsData = value;
            }
        }

        private bool settingsChanged;
        public bool SettingsChanged
        {
            get => settingsChanged;
            set
            {
                settingsChanged = value;

                if (settingsChanged)
                    MainWinDC.ApplySettingsChanges();
            }
        }

        private SearchEngine currentSearchEngine;
        public SearchEngine CurrentSearchEngine
        {
            get { return currentSearchEngine; }
            set
            {
                if (value != currentSearchEngine)
                {
                    currentSearchEngine = value;
                    CurrentSettings.SearchEngine = currentSearchEngine;
                    SettingsChanged = true;
                    NotifyPropertyChanged("CurrentSearchEngine");
                    NotifyPropertyChanged("SelectedEngineIndex");
                }
            }
        }

        private string downloadsFolderPath;
        public string DownloadsFolderPath
        {
            get { return downloadsFolderPath; }
            set
            {
                if (value != downloadsFolderPath)
                {
                    downloadsFolderPath = value;
                    CurrentSettings.DownloadsFolder = downloadsFolderPath;
                    SettingsChanged = true;
                    NotifyPropertyChanged("DownloadsFolderPath");
                }
            }
        }

        private bool bookmarkBarIsVisible;
        public bool BookmarkBarIsVisible
        {
            get => bookmarkBarIsVisible;
            set
            {
                if (value != bookmarkBarIsVisible)
                {
                    bookmarkBarIsVisible = value;

                    CurrentSettings.BookmarkVisible = bookmarkBarIsVisible;
                    SettingsChanged = true;
                    NotifyPropertyChanged("BookmarkBarIsVisible");
                }
            }
        }

        public int SelectedEngineIndex
        {
            get { return (int)CurrentSearchEngine; }
            set
            {
                CurrentSearchEngine = (SearchEngine)value;
            }
        }

        private string url;
        public String Url
        {
            get { return url; }
            set
            {
                url = value;

                if (url.Equals(GetSearchEngineURL()))
                    BookmarkBarVisibility = Visibility.Visible;
                else
                    BookmarkBarVisibility = bookmarkBarIsVisible ? Visibility.Visible : Visibility.Collapsed;

                NotifyPropertyChanged("Url");
            }
        }

        private string secureIconPath;
        public String SecureIconPath
        {
            get { return secureIconPath; }
            set { secureIconPath = value; NotifyPropertyChanged("SecureIconPath"); }
        }

        private string bookmarkedIconPath;
        public String BookmarkedIconPath
        {
            get { return bookmarkedIconPath; }
            set { bookmarkedIconPath = value; NotifyPropertyChanged("BookmarkedIconPath"); }
        }
        public String SearchBarText
        {
            get { return searchBarText; }
            set { searchBarText = value; NotifyPropertyChanged("SearchBarText"); }
        }

        private string secureIconToolTip;
        public string SecureIconToolTip
        {
            get { return secureIconToolTip; }
            set { secureIconToolTip = value; NotifyPropertyChanged("SecureIconToolTip"); }
        }

        private bool wentBackOrForward;
        public bool WentBackOrForward
        {
            get { return wentBackOrForward; }
            set { wentBackOrForward = value; }
        }

        private MainVM mainWinDC;
        public MainVM MainWinDC
        {
            get { return mainWinDC; }
            set
            {
                mainWinDC = value;

                if (!InitSettings())
                {
                    Url = "https://google.com";
                    CurrentSearchEngine = SearchEngine.Google;
                    BookmarkBarIsVisible = true;
                    DownloadsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
                }
            }
        }

        private Visibility historyLoadingTextVisibility;
        public Visibility HistoryLoadingTextVisibility
        {
            get { return historyLoadingTextVisibility; }
            set { historyLoadingTextVisibility = value; NotifyPropertyChanged("HistoryLoadingTextVisibility"); }
        }

        private bool initialHistoryDataLoaded;
        public bool InitialHistoryDataLoaded
        {
            get { return initialHistoryDataLoaded; }
            set { initialHistoryDataLoaded = value; }
        }

        private Visibility bookmarkBarVisibility;
        public Visibility BookmarkBarVisibility
        {
            get { return bookmarkBarVisibility; }
            set { bookmarkBarVisibility = value; NotifyPropertyChanged("BookmarkBarVisibility"); }
        }

        public ObservableCollection<BookmarkContainer> Bookmarks
        {
            get
            {
                if (MainWinDC != null)
                {
                    return MainWinDC.Bookmarks;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task LoadHistory()
        {
            await MainWinDC.LoadHistoryDataToUI(MainWinDC.CurrentSessionHistoryData, true);
            await MainWinDC.LoadHistoryDataToUI(MainWinDC.SavedHistoryData, false);

            InitialHistoryDataLoaded = true;
        }

        private bool InitSettings()
        {
            if (CurrentSettings != null)
            {
                CurrentSearchEngine = CurrentSettings.SearchEngine;
                DownloadsFolderPath = CurrentSettings.DownloadsFolder;
                Url = GetSearchEngineURL();
                BookmarkBarIsVisible = CurrentSettings.BookmarkVisible;

                return true;
            }

            CurrentSettings = new Settings();
            return false;
        }

        public RelayCommand BackCommand { get; set; }
        public RelayCommand ForwardCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand StopLoadCommand { get; set; }
        public RelayCommand AddBookmarkCommand { get; set; }
        public RelayCommand<int> AddSettingsTabCommand { get; set; }

        public void Dispose()
        {
            if (WebBrowser.RequestContext != null)
                WebBrowser.RequestContext.Dispose();

            WebBrowser.Dispose();
        }

        public void LogHistory(string url, string title)
        {
            string date = DateTime.Now.ToString("ddd d MMM, HH:mm");

            string hostName = new Uri(url).Host;
            string fullDomain = new Uri(Url).GetLeftPart(UriPartial.Authority)
                    .Replace("/www.", "/")
                    .Replace("http://", "")
                    .Replace("https://", "")
                    .ToString();

            var newHistoryItem = new HistoryItem()
            {
                Url = url,
                Title = title,
                BrowseDate = date,
                Domain = fullDomain,
                HostName = hostName
            };

            if (
                MainWinDC.CurrentSessionHistoryData.Count > 0 &&
                newHistoryItem != MainWinDC.CurrentSessionHistoryData.Last()
                )
                MainWinDC.AddHistoryItem(newHistoryItem);
            else if (MainWinDC.CurrentSessionHistoryData.Count == 0)
                MainWinDC.AddHistoryItem(newHistoryItem);
        }

        private void Init()
        {
            BookmarkedIconPath = isNotBookmarkedIconPath;
            SecureIconPath = isSecureIconPath;
            WentBackOrForward = false;
            HistoryLoadingTextVisibility = Visibility.Visible;
            InitialHistoryDataLoaded = false;
            SecureIconToolTip = "Website is secure.";

            //see https://stackoverflow.com/a/39220412 for zoom in/out
        }

        public TabContentVM()
        {
            Init();

            BackCommand = new RelayCommand(o =>
            {
                if (WebBrowser.CanGoBack)
                {
                    WentBackOrForward = true;
                    WebBrowser.Back();
                }
            });

            ForwardCommand = new RelayCommand(o =>
            {
                if (WebBrowser.CanGoForward)
                {
                    WentBackOrForward = true;
                    WebBrowser.Forward();
                }
            });

            RefreshCommand = new RelayCommand(o =>
            {
                WebBrowser.Reload();
            });

            StopLoadCommand = new RelayCommand(o =>
            {
                WebBrowser.Stop();
            });

            SearchCommand = new RelayCommand(o =>
            {
                if (urlRegex.IsMatch(SearchBarText) || httpUrlRegex.IsMatch(SearchBarText))
                {
                    WebBrowser.Load(SearchBarText);
                }
                else // change to Search()
                    //WebBrowser.Load("https://google.com/search?q=" + SearchBarText);
                    Search(SearchBarText);

            });

            AddBookmarkCommand = new RelayCommand(o =>
            {
                if (!IsBookmarked(WebBrowser.Address))
                {
                    BookmarkedIconPath = isBookmarkedIconPath;

                    string fullDomain = new Uri(Url).GetLeftPart(UriPartial.Authority)
                        .Replace("/www.", "/")
                        .Replace("http://", "")
                        .Replace("https://", "")
                        .ToString();

                    MainWinDC.AddBookmark(new Bookmark() { Title = WebBrowser.Title, Url = WebBrowser.Address, Domain = fullDomain });
                }
                else
                {
                    foreach (var bookmark in MainWinDC.Bookmarks)
                    {
                        if (bookmark.Url == Url)
                        {
                            MainWinDC.Bookmarks.Remove(bookmark);
                            MainWinDC.BookmarkDeleted = true;
                            break;
                        }
                    }

                    BookmarkedIconPath = isNotBookmarkedIconPath;
                }
            });

            AddSettingsTabCommand = new RelayCommand<int>(index =>
            {
                MainWinDC.AddSettingsTab(index);
            });
        }
    }
}
