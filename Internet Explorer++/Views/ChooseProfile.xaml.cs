using CefSharp.DevTools.CSS;
using IEPP.Controls;
using IEPP.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IEPP.Views
{
    /// <summary>
    /// Interaction logic for ChooseProfile.xaml
    /// </summary>
    public partial class ChooseProfile : UserControl
    {
        private readonly ChooseProfileVM dc;
        public bool FirstSelection { get; set; }

        public string UsersDir
        {
            get { return (string)GetValue(UsersDirProperty); }
            set { SetValue(UsersDirProperty, value); dc.WorkingDir = value; }
        }

        public static readonly DependencyProperty UsersDirProperty =
            DependencyProperty.Register("UsersDir", typeof(string), typeof(ChooseProfile), new PropertyMetadata(null));

        public ChooseProfile()
        {
            InitializeComponent();
            dc = DataContext as ChooseProfileVM;
            FirstSelection = true;
        }

        private void SetChooseProfileToVisible(bool isVisible)
        {
            dc.ChooseProfileVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(150);
            SetChooseProfileToVisible(true);
        }

        private async void ChooseProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserListGrid.IsEnabled = false;
            this.IsEnabled = false;

            if (FirstSelection)
                await Task.Delay(400);
            else
                await Task.Delay(0);

            Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            dc.SelectedUser = new UserContainer() { Username = "" };

            UserListGrid.IsEnabled = false;
            this.IsEnabled = false;

            if (FirstSelection)
                await Task.Delay(400);
            else
                await Task.Delay(0);

            Visibility = Visibility.Collapsed;
        }
    }
}
