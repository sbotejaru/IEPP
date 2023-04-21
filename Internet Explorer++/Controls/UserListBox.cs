using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace IEPP.Controls
{
    public class UserListBox : ListBox, INotifyPropertyChanged
    {
        #region PropertyChanged declaration
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        public static readonly DependencyProperty AddNewVisibilityProperty =
            DependencyProperty.Register("AddNewVisibility", typeof(Visibility), typeof(UserListBox), new PropertyMetadata(null));

        public static readonly DependencyProperty NewProfileCommandProperty =
            DependencyProperty.Register("NewProfileCommand", typeof(ICommand), typeof(UserListBox), new PropertyMetadata(null));

        public ICommand NewProfileCommand
        {
            get { return (ICommand)GetValue(NewProfileCommandProperty); }
            set { SetValue(NewProfileCommandProperty, value); }
        }

        public Visibility AddNewVisibility
        {
            get { return (Visibility)GetValue(AddNewVisibilityProperty); }
            set { SetValue(AddNewVisibilityProperty, value); NotifyPropertyChanged("AddNewVisibility"); }
        }

        public UserListBox()
        {
            AddNewVisibility = Visibility.Visible;
        }
    }
}
