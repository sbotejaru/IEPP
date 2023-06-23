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
using System.Windows.Shapes;

namespace IEPP.Views
{
    /// <summary>
    /// Interaction logic for ChooseProfileWindow.xaml
    /// </summary>
    public partial class ChooseProfileWindow : Window
    {
        private readonly ChooseProfileVM dc;
        public bool FirstSelection { get; set; }

        public ChooseProfileWindow()
        {
            InitializeComponent();
            dc = DataContext as ChooseProfileVM;
            FirstSelection = true;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var docsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string subPath = "/Internet Explorer++/Users";

            dc.WorkingDir = docsFolder + subPath;

            await Task.Delay(150);
            SetChooseProfileToVisible(true);
        }

        private void SetChooseProfileToVisible(bool isVisible)
        {
            dc.ChooseProfileVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void ChooseProfileList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(dc.SelectedUser.Username);
            await InitMainWindow(dc.SelectedUser.Username, dc.CurrentUserPath);
        }

        private async void IncognitoBtn_Click(object sender, RoutedEventArgs e)
        {
            await InitMainWindow("", "");
        }

        private async Task InitMainWindow(string username, string userPath)
        {
            UserListGrid.IsEnabled = false;

            App.Current.MainWindow = null;
            App.Current.MainWindow = new MainWindow(username, userPath);            

            if (FirstSelection)
                await Task.Delay(400);
            else
                await Task.Delay(0);         

            Visibility = Visibility.Collapsed;
            App.Current.MainWindow.Show();
        }
    }
}
