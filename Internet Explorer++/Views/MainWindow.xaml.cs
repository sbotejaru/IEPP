using CefSharp.Wpf;
using CefSharp;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using IEPP.Controls;
using IEPP.ViewModels;

namespace IEPP.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Uri restoreDownSource = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/restore_down.png");
        Uri maximizeSource = new Uri("pack://application:,,,/Internet Explorer++;component/Icons/maximize.png");
        MainVM dataContextVM;

        public MainWindow()
        {
            InitializeComponent();
            dataContextVM = DataContext as MainVM;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            //this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.WindowState = WindowState.Minimized;
        }

        /* private void Button_Click(object sender, RoutedEventArgs e)
         {
             this.WindowState = WindowState.Maximized;
         }*/

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.MaxBtnIcon.Source = new BitmapImage(restoreDownSource);
                this.MainGrid.Margin = new Thickness(6);
            }
            else if (this.WindowState == WindowState.Normal)
            {
                this.MaxBtnIcon.Source = new BitmapImage(maximizeSource);
                this.MainGrid.Margin = new Thickness(0);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            /*Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle, 
                new Action(() => WindowStyle = WindowStyle.None)
                );*/
        }

        private void ChooseProfileUC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue.ToString() == "False")
            {
                var chooseProfileDC = ChooseProfileUC.DataContext as ChooseProfileVM;

                if (chooseProfileDC.SelectedUser != null)
                    dataContextVM.Username = chooseProfileDC.SelectedUser.Username;

                BrowserTabs.Visibility = Visibility.Visible;
            }
            else if (e.NewValue.ToString() == "True")
                BrowserTabs.Visibility = Visibility.Collapsed;
        }
    }
}
