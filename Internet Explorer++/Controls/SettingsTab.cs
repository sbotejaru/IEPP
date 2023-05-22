using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace IEPP.Controls
{
    public class SettingsTab : TabItem, INotifyPropertyChanged
    {
        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion        
        
        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register("IconSource", typeof(ImageSource), typeof(SettingsTab), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(SettingsTab), new PropertyMetadata(null));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); NotifyPropertyChanged("Title"); }
        }

        public ImageSource IconSource
        {
            get { return (ImageSource)GetValue(IconSourceProperty); }
            set { SetValue(IconSourceProperty, value); NotifyPropertyChanged("IconSource"); }
        }
    }
}
