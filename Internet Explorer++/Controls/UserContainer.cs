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
using IEPP.ViewModels;
using CefSharp.Wpf;
using CefSharp.DevTools.HeapProfiler;

namespace IEPP.Controls
{
    public class UserContainer : ContentControl, INotifyPropertyChanged
    {

        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public static readonly DependencyProperty ProfilePicProperty =
            DependencyProperty.Register("ProfilePic", typeof(BitmapImage), typeof(UserContainer), new PropertyMetadata(null));

        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(string), typeof(UserContainer), new PropertyMetadata(null));

        public BitmapImage ProfilePic
        {
            get { return (BitmapImage)GetValue(ProfilePicProperty); }
            set { SetValue(ProfilePicProperty, value); NotifyPropertyChanged("ProfilePic"); }
        }

        public string Username
        {
            get { return (string)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); NotifyPropertyChanged("Username"); }
        }
    }
}
