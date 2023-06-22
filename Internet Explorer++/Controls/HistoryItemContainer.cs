using IEPP.Models;
using IEPP.Utils;
using IEPP.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IEPP.Controls
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class HistoryItemContainer : ContentControl, INotifyPropertyChanged
    {
        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        [JsonProperty]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); NotifyPropertyChanged("Title"); }
        }

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(HistoryItemContainer), new PropertyMetadata(null));

        [JsonProperty]
        public string BrowseDate
        {
            get { return (string)GetValue(BrowseDateProperty); }
            set { SetValue(BrowseDateProperty, value); NotifyPropertyChanged("BrowseDate"); }
        }

        public static readonly DependencyProperty BrowseDateProperty =
            DependencyProperty.Register("BrowseDate", typeof(string), typeof(HistoryItemContainer), new PropertyMetadata(null));


        private string url;

        [JsonProperty]
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string domain;
        [JsonProperty]
        public string Domain
        {
            get => domain;
            set
            {
                domain = value;
                IconHandler.GetFavIcon(Url, domain);
            }
        }

        [JsonProperty]
        public string HostName { get; set; } // for website name

        public FavIconHandler IconHandler { get; }

        public RelayCommand NewTabFromHistory { get; set; }

        public BookmarkContainer ToBookmarkContainer()
        {
            return new BookmarkContainer()
            {
                Title = this.Title,
                Url = this.Url,
                Domain = this.Domain
            };
        }

        public HistoryItem ToModel()
        {
            return new HistoryItem()
            {
                Title = Title,
                Url = Url,
                BrowseDate = BrowseDate,
                Domain = Domain,
                HostName = HostName
            };
        }

        public RelayCommand DeleteHistoryItemCommand { get; set; }

        public HistoryItemContainer()
        {
            IconHandler = new FavIconHandler();

            //Console.WriteLine(this.DataContext);

            NewTabFromHistory = new RelayCommand(o =>
            {
                (App.Current.MainWindow.DataContext as MainVM).AddBrowserTab(Url);
            });

            DeleteHistoryItemCommand = new RelayCommand(o =>
            {
                (App.Current.MainWindow.DataContext as MainVM).DeleteHistoryItem(this);
            });
        }
    }
}
