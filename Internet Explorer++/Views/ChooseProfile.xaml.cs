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
        public ChooseProfile()
        {
            InitializeComponent();
        }

        private void SetChooseProfileToVisible(bool isVisible)
        {
            var dc = DataContext as ChooseProfileVM;
            dc.ChooseProfileVisibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(150);
            SetChooseProfileToVisible(true);
        }
    }
}
