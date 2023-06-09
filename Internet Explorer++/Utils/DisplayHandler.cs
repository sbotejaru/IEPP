using CefSharp;
using CefSharp.Enums;
using CefSharp.Structs;
using CefSharp.Wpf.Experimental;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Nager.PublicSuffix;
using System.IO;
using System.Security.Policy;

namespace IEPP.Utils
{
    public class DisplayHandler : IDisplayHandler, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        bool firstLoad = true;
        //private string cacheDirPath;
        //private string currentDomainName;
        //private bool downloadForTab;
        private FavIconHandler iconHandler;

        public FavIconHandler IconHandler
        {
            get => iconHandler;
        }

        public DisplayHandler()
        {
            iconHandler = new FavIconHandler();
        }

        private object _favIcon;

        /// <summary>
        /// For binding to System.Windows.Window.Icon.
        /// </summary>
        public object FavIcon
        {
            get { return _favIcon; }
            set { _favIcon = value; OnPropertyChanged("FavIcon"); }
        }

        /*private BitmapDecoder _decoder;
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
            if (downloadForTab)
                FavIcon = Decoder.Frames.OrderBy(f => f.Width).FirstOrDefault();

            SaveFavIconToCache(currentDomainName + ".png");

            Decoder = null;
        }

        private void SaveFavIconToCache(string filename)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(Decoder.Frames.OrderBy(f => f.Width).FirstOrDefault());

            var filePath = cacheDirPath + '/' + filename;

            if (!File.Exists(filePath))
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
        }

        public void DownloadFavIcon(Uri url, bool forTab)
        {
            downloadForTab = forTab;

            string favIconUrl = "http://www.google.com/s2/favicons?domain=";
            var baseUrl = url.GetLeftPart(UriPartial.Authority);

            Decoder = BitmapDecoder.Create(new Uri(favIconUrl + baseUrl + "&sz=32"), BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None);
        }*/

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
                var oldDomain = new Uri(chromiumWebBrowser.Address).GetLeftPart(UriPartial.Authority);
                var newDomain = new Uri(addressChangedArgs.Address).GetLeftPart(UriPartial.Authority);

                if (oldDomain != newDomain || firstLoad)
                {
                    var domainParser = new DomainParser(new WebTldRuleProvider());
                    var domainName = domainParser.Parse(newDomain).Domain;

                    iconHandler.GetFavIcon(addressChangedArgs.Address, domainName);

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
