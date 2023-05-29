using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace IEPP.Controls
{
    public class BookmarkContainer : ContentControl, INotifyPropertyChanged
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
           DependencyProperty.Register("FavIconSource", typeof(BitmapImage), typeof(BookmarkContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(BookmarkContainer), new PropertyMetadata(null));

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

        public string Url { get; set; }
    }
}
