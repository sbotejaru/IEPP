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
        public bool BoolTest
        {
            get { return (bool)GetValue(BoolTestProperty); }
            set { SetValue(BoolTestProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoolTest.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BoolTestProperty =
            DependencyProperty.Register("BoolTest", typeof(bool), typeof(ChooseProfile), new PropertyMetadata(false));


        public ChooseProfile()
        {
            InitializeComponent();            
        }

        private void SetElementsToVisible(bool isVisible)
        {
            ChooseProfileText.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
            ChooseProfileList.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(150);
            SetElementsToVisible(true);
            //await Task.Delay(2000);
            //ChooseProfileList.IsEnabled = false;
            //ChooseProfileText.IsEnabled = false;
        }
    }
}
