using CefSharp;
using CefSharp.Enums;
using CefSharp.Structs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static Community.CsharpSqlite.Sqlite3;

namespace IEPP.Utils
{
    public class DisplayHandler : IDisplayHandler, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool firstLoad = true;

        private object _favIcon;

        /// <summary>
        /// For binding to System.Windows.Window.Icon.
        /// </summary>
        public object FavIcon
        {
            get { return _favIcon; }
            set { _favIcon = value; OnPropertyChanged("FavIcon"); }
        }

        private BitmapDecoder _decoder;
        private BitmapDecoder Decoder
        {
            get => _decoder;
            set
            {
                if (_decoder != null) _decoder.DownloadCompleted -= decoderDownloadCompleted;
                _decoder = value;
                if (_decoder != null) _decoder.DownloadCompleted += decoderDownloadCompleted;
            }
        }

        private void decoderDownloadCompleted(object sender, EventArgs e)
        {
            FavIcon = Decoder.Frames.OrderBy(f => f.Width).FirstOrDefault();
            Decoder = null;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
            else PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnAddressChanged(IWebBrowser chromiumWebBrowser, AddressChangedEventArgs addressChangedArgs)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var oldUrl = new Uri(chromiumWebBrowser.Address);
                var newUrl = new Uri(addressChangedArgs.Address);

                if ((oldUrl.GetLeftPart(UriPartial.Authority) != newUrl.GetLeftPart(UriPartial.Authority)) || firstLoad)
                {
                    string favIconUrl = "http://www.google.com/s2/favicons?domain=";
                    var baseUrl = newUrl.GetLeftPart(UriPartial.Authority);
                    Decoder = BitmapDecoder.Create(new Uri(favIconUrl + baseUrl + "&sz=32"), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);
                    firstLoad = false;
                }
            });
        }

        public bool OnAutoResize(IWebBrowser chromiumWebBrowser, IBrowser browser, CefSharp.Structs.Size newSize)
        {
            return false;
        }

        public bool OnConsoleMessage(IWebBrowser chromiumWebBrowser, ConsoleMessageEventArgs consoleMessageArgs)
        {
            return false;
        }

        public void OnFaviconUrlChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IList<string> urls)
        {
        }

        public void OnFullscreenModeChange(IWebBrowser chromiumWebBrowser, IBrowser browser, bool fullscreen)
        {
        }

        public void OnLoadingProgressChange(IWebBrowser chromiumWebBrowser, IBrowser browser, double progress)
        {
        }

        public void OnStatusMessage(IWebBrowser chromiumWebBrowser, StatusMessageEventArgs statusMessageArgs)
        {
        }

        public void OnTitleChanged(IWebBrowser chromiumWebBrowser, TitleChangedEventArgs titleChangedArgs)
        {
        }

        public bool OnTooltipChanged(IWebBrowser chromiumWebBrowser, ref string text)
        {
            return false;
        }

        public bool OnCursorChange(IWebBrowser chromiumWebBrowser, IBrowser browser, IntPtr cursor, CursorType type, CursorInfo customCursorInfo)
        {
            return false;
        }
    }
}
