using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CefSharp;
using CefSharp.Wpf;
using IEPP.Utils;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Security.Permissions;
using ChromeTabs;

namespace IEPP.ViewModels
{
    public class TabContentVM : INotifyPropertyChanged
    {
        private string url = "https://google.com";
        private string isSecureIconPath = "/Icons/lock.png";
        private string isNotSecureIconPath = "/Icons/warning.png";
        private string isBookmarkedIconPath = "/Icons/star_full.png";
        private string isNotBookmarkedIconPath = "/Icons/star.png";
        private string searchBarText;
        private string pageTitle;
        private Regex urlRegex = new Regex("^[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        //private Regex HttpsUrlRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");
        private Regex httpUrlRegex = new Regex("^(http|https)?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

        public ChromiumWebBrowser WebBrowser { get; set; }

        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

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
            get { return isBookmarkedIconPath; }
            set { isBookmarkedIconPath = value; NotifyPropertyChanged("IsBookmarkedIconPath"); }
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

        public RelayCommand CloseCommand { get; set; }
        public RelayCommand BackCommand { get; set; }
        public RelayCommand ForwardCommand { get; set; }
        public RelayCommand RefreshCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }
        public RelayCommand StopLoadCommand { get; set; }

        public void Dispose()
        {
            WebBrowser.Dispose();
            //WebBrowser.RequestContext.Dispose();            
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
                else
                    WebBrowser.Load("https://google.com/search?q=" + SearchBarText);
            });
        }
    }
}
