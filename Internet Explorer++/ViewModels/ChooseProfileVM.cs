using IEPP.Controls;
using IEPP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace IEPP.ViewModels
{
    public class ChooseProfileVM : INotifyPropertyChanged
    {
        #region PropertyChanged declaration

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private UserContainer selectedUser;
        private Visibility addNewVisibility;

        private Visibility vis;

        public Visibility Vis
        {
            get { return vis; }
            set { vis = value; NotifyPropertyChanged("Vis"); }
        }

        public Visibility AddNewVisibility
        {
            get { return addNewVisibility; }
            set { addNewVisibility = value; NotifyPropertyChanged("AddNewVisibility"); }
        }

        public UserContainer SelectedUser
        {
            get { return selectedUser; }
            set { selectedUser = value; NotifyPropertyChanged("SelectedUser"); Console.WriteLine(selectedUser.Username); Vis = Visibility.Collapsed; }
        }
        public ObservableCollection<UserContainer> UserList { get; set; }

        public RelayCommand AddNewUserCommand { get; set; }

        public ChooseProfileVM()
        {
            UserList = new ObservableCollection<UserContainer>();
            var icon = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/coffee.jpg");
            //var icon2 = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/Add2_144.png");


            var user = new UserContainer() { Username = "sbot", ProfilePic = new BitmapImage(icon) };
            var user2 = new UserContainer() { Username = "sbot2", ProfilePic = new BitmapImage(icon) };
            var user3 = new UserContainer() { Username = "sbot3", ProfilePic = new BitmapImage(icon) };
            var user4 = new UserContainer() { Username = "sbot4", ProfilePic = new BitmapImage(icon) };

            UserList.Add(user);
            UserList.Add(user2);
            UserList.Add(user3);
            UserList.Add(user4);

            AddNewUserCommand = new RelayCommand(o =>
            {
                AddNewVisibility = Visibility.Collapsed;
            });

        }
    }
}
