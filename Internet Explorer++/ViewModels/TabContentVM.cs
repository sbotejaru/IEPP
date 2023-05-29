using CefSharp;
using CefSharp.Wpf;
using IEPP.Controls;
using IEPP.Utils;
using IEPP.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

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
        private string isNotSecureIconPath = "/Icons/warning.png";
        private string isBookmarkedIconPath = "/Icons/star_full.png";
        private string bookmarkedIconPath = "/Icons/star_full.png";
        private string isNotBookmarkedIconPath = "/Icons/star.png";
        private string searchBarText;
        private string pageTitle;
        private Regex urlRegex = new Regex("^[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        //private Regex HttpsUrlRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        private Regex httpUrlRegex = new Regex("^(http|https)?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        private string googleUrl;

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

        public void LoadMainDC()
        {
            var mainwinDC = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
            MainWinDC = mainwinDC;
        }

        public ChromiumWebBrowser WebBrowser { get; set; }

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

        private MainVM mainWinDC;
        public MainVM MainWinDC
        {
            get { return mainWinDC; }
            set { mainWinDC = value; }
        }

        public RelayCommand CloseCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand ForwardCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand StopLoadCommand { get; set; }
        public RelayCommand AddBookmarkCommand { get; set; }

        public void Dispose()
        {
            WebBrowser.Dispose();
            //WebBrowser.RequestContext.Dispose();            
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

        public TabContentVM()
        {
            BookmarkedIconPath = isNotBookmarkedIconPath;
            
            //WebBrowser.Load(Url);    

            BackCommand = new RelayCommand(o =>
            {
                if (WebBrowser.CanGoBack)
                {
                    WebBrowser.Back();
                }
            });

            ForwardCommand = new RelayCommand(o =>
            {
                if (WebBrowser.CanGoForward)
                {
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
                Bookmark b = new Bookmark() { Title = "Google", Url = "https://www.google.com" };
                //var mainwinDC = (App.Current.MainWindow as MainWindow).DataContext as MainVM;
                MainWinDC.Bookmarks.Add(b.ToContainer());
                Console.WriteLine(MainWinDC.Bookmarks.Count);
            });

        }
    }
}
