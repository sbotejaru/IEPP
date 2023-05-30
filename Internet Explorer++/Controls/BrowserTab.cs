using IEPP.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using IEPP.Utils;
using IEPP.Enums;
using IEPP.ViewModels;
using CefSharp.Wpf;
using CefSharp.DevTools.HeapProfiler;

namespace IEPP.Controls
{
    public class BrowserTab : TabItem, INotifyPropertyChanged
    {
        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public static readonly DependencyProperty FavIconSourceProperty =
            DependencyProperty.Register("FavIconSource", typeof(BitmapImage), typeof(BrowserTab), new PropertyMetadata(null));
        
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BrowserTab), new PropertyMetadata(null));

        public DisplayHandler DisplayHandler { get; set; }

        public BitmapImage FavIconSource
        {
            get { return (BitmapImage)GetValue(FavIconSourceProperty); }
            set { SetValue(FavIconSourceProperty, value); NotifyPropertyChanged("FavIconSource"); }
        }

        public string Title
        { 
            get { return (string)GetValue(TitleProperty); } 
            set { SetValue(TitleProperty, value); NotifyPropertyChanged("Title"); }
        }

        private Visibility separatorVisibility;
        public Visibility SeparatorVisibility
        {
            get { return separatorVisibility; }
            set { separatorVisibility = value; NotifyPropertyChanged("SeparatorVisibility"); }
        }

        private BrowserTabControl btabControl;

        public BrowserTabControl BTabControl
        {
            get { return btabControl; }
            set { btabControl = value; }
        }

        protected ChromiumWebBrowser GetCurrentBrowser()
        {
            if (this.Content as TabContent != null)
            {
                var tabDataContext = (this.Content as TabContent).webBrowser;
                return tabDataContext;
            }

            return null;
        }        

        public void Dispose()
        {
            if (this.Content as TabContent != null)
            {
                var tabDataContext = (this.Content as TabContent).DataContext as TabContentVM;
                tabDataContext.Dispose();
            }
        }

        private void TitleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Title = e.NewValue.ToString();
        }

        private void InitializeBrowser()
        {
            var browser = GetCurrentBrowser();
            browser.TitleChanged += TitleChanged;
            browser.DisplayHandler = DisplayHandler;
            var icon = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/IEPP_gray.ico");
            DisplayHandler.FavIcon = new BitmapImage(icon);
        }

        private void InitializeSettings()
        {
            var icon = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/settings.png");
            DisplayHandler.FavIcon = new BitmapImage(icon);
        }

        private void GetTabControl()
        {
            var mainWin = App.Current.MainWindow as MainWindow;
            BTabControl = mainWin.BrowserTabs;
        }

        public BrowserTab()
        {
        }

        public string GetURL()
        {
            return null;
        }

        public BrowserTab(TabType tabType)
        {
            DisplayHandler = new DisplayHandler();            

            switch (tabType)
            {
                case TabType.Browser:
                    this.Content = new TabContent();
                    InitializeBrowser();
                    break;

                case TabType.Settings:
                    this.Content = new SettingsControl();
                    InitializeSettings();
                    break;                    
            }

            GetTabControl();
        }

        public BrowserTab(TabContent browser)
        {
            this.Content = browser;
        }
    }
}