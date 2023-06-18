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
                    string fullDomain = newDomain.Replace("/www.", "/").Replace("http://", "").Replace("https://", "").ToString();

                    iconHandler.GetFavIcon(addressChangedArgs.Address, fullDomain);

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
