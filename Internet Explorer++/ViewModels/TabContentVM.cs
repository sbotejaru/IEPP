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

        private string url = "https://google.com";
        private string isSecureIconPath = "/Icons/lock.png";
        private readonly string isNotSecureIconPath = "/Icons/warning.png";
        private readonly string isBookmarkedIconPath = "/Icons/star_full.png";
        private string bookmarkedIconPath = "/Icons/star_full.png";
        private readonly string isNotBookmarkedIconPath = "/Icons/star.png";
        private string searchBarText;
        private string pageTitle;
        private bool historyLoaded;
        private readonly Regex urlRegex = new Regex("^[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        //private Regex HttpsUrlRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        private readonly Regex httpUrlRegex = new Regex("^(http|https)?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        private void Search(string searchUrl/*engineType?*/)
        {
            /* switch by search engine saved in settings
            string googleSearch = "https://google.com/search?q=";
            string yandexSearch = "https://yandex.com/search/?text=";
            string duckSearch = "https://duckduckgo.com/?q=";
            string yahooSearch = "https://search.yahoo.com/search?p=";
            string bingSearch = "https://www.bing.com/search?q=";
            */
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

                if (selectedSettingsTab == 1 && !historyLoaded)
                {
                    //App.Current.Dispatcher.Invoke(new Action(() => MainWinDC.HistoryDataToUI()), System.Windows.Threading.DispatcherPriority.Background);
                    //MainWinDC.HistoryDataToUI();

                    //LoadHistory();

                    historyLoaded = true;
                }
            }
        }

        public String Url
        {
            get { return url; }
            set { url = value; NotifyPropertyChanged("Url"); }
        }
        public String SecureIconPath
        {
            get { return isSecureIconPath; }
            set { isSecureIconPath = value; NotifyPropertyChanged("IsSecureIconPath"); }
        }
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
        public String PageTitle
        {
            get { return pageTitle; }
            set { pageTitle = value; NotifyPropertyChanged("PageTitle"); }
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
            set { mainWinDC = value; }
        }

        private Visibility historyLoadingTextVisibility;

        public Visibility HistoryLoadingTextVisibility
        {
            get { return historyLoadingTextVisibility; }
            set { historyLoadingTextVisibility = value; NotifyPropertyChanged("HistoryLoadingTextVisibility"); }
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
                    // Handle the case when the DataContext is not of type MainWindowViewModel
                    return null;
                }
            }
        }

        public async Task LoadHistory()
        {
            await Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    for (int index = 0; index < MainWinDC.HistoryData.Count; ++index)
                    {
                        History.Add(MainWinDC.HistoryData[index].ToContainer());
                    }
                });
            });

            /*int batchSize = 4; // Number of items to process per batch
            int currentIndex = 0; // Current index of HistoryData being processed

            while (currentIndex < MainWinDC.HistoryData.Count)
            {
                // Get the next batch of HistoryData to process
                var batchData = MainWinDC.HistoryData.Skip(currentIndex).Take(batchSize).ToList();

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
                            batchUI.Add(historyItem.ToContainer());
                        }

                        // Add the batchUI elements to the ObservableCollection
                        foreach (var historyUI in batchUI)
                        {
                            History.Add(historyUI);
                        }
                    });
                });

                // Increase the currentIndex to process the next batch
                currentIndex += batchSize;

                // Delay to allow the UI to remain responsive
                await Task.Delay(0); // Adjust the delay duration as needed
            }*/


            /*foreach (var tab in MainWinDC.HistoryData)
                History.Add(tab.ToContainer());*/
        }

        public ObservableCollection<HistoryItemContainer> History { get; set; }

        public RelayCommand BackCommand { get; set; }
        public RelayCommand ForwardCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand StopLoadCommand { get; set; }
        public RelayCommand AddBookmarkCommand { get; set; }

        public void Dispose()
        {
            if (WebBrowser.RequestContext != null)
                WebBrowser.RequestContext.Dispose();

            WebBrowser.Dispose();
        }

        public void LogHistory(string url, string title)
        {
            string date = DateTime.Now.ToString("ddd d MMM, HH:mm");

            var domainParser = new DomainParser(new WebTldRuleProvider());
            var domainInfo = domainParser.Parse(url);

            var domain = domainInfo.Domain;
            var hostName = domainInfo.Hostname;

            MainWinDC.AddHistoryItem(new HistoryItem()
            {
                Url = url,
                Title = title,
                BrowseDate = date,
                Domain = domain,
                HostName = hostName
            });
        }

        private void SaveBookmark()
        {
            BookmarkedIconPath = isBookmarkedIconPath;

        }

        private void DeleteBookmark()
        {
            BookmarkedIconPath = isNotBookmarkedIconPath;

        }

        private void LoadBookmarks()
        {

        }

        private void Init()
        {
            BookmarkedIconPath = isNotBookmarkedIconPath;
            WentBackOrForward = false;
            HistoryLoadingTextVisibility = Visibility.Visible;
            historyLoaded = false;
            History = new ObservableCollection<HistoryItemContainer>();
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
                    WebBrowser.Load("https://google.com/search?q=" + SearchBarText);

            });

            AddBookmarkCommand = new RelayCommand(o =>
            {
                var domainParser = new DomainParser(new WebTldRuleProvider());
                var domain = domainParser.Parse(WebBrowser.Address).Domain;
                domainParser = null;

                MainWinDC.AddBookmark(new Bookmark() { Title = WebBrowser.Title, Url = WebBrowser.Address, Domain = domain });
            });

        }
    }
}
