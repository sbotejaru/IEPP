using IEPP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using System.Windows.Media;
using IEPP.Utils;

namespace IEPP.Controls
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class BookmarkContainer : ContentControl, INotifyPropertyChanged, ICloneable
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
           DependencyProperty.Register("FavIconSource", typeof(ImageSource), typeof(BookmarkContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BookmarkContainer), new PropertyMetadata(null));

        public ImageSource FavIconSource
        {
            get { return (ImageSource)GetValue(FavIconSourceProperty); }
            set { SetValue(FavIconSourceProperty, value); NotifyPropertyChanged("FavIconSource"); }
        }

        [JsonProperty]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); NotifyPropertyChanged("Title"); }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        //public string Url { get; set; }

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

        public FavIconHandler IconHandler { get; }

        public Bookmark ToModel()
        {
            return new Bookmark() { Title = this.Title, Url = this.Url };
        }

        public BookmarkContainer()
        {
            IconHandler = new FavIconHandler();
        }
    }
}
