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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private TabContentVM vm;
        private bool loaded = false;
        public SettingsControl(MainVM mainVM)
        {
            InitializeComponent();
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainVM;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //vm.MainWinDC.bgWorker.DoWork += vm.MainWinDC.bgWorker_LoadHistory;
            HistoryList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            loaded = true;
        }

        public void LoadHistoryUI()
        {
            vm.HistoryLoadingTextVisibility = Visibility.Collapsed;
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (loaded)
            {
                if (vm.SelectedSettingsTab == 1)
                {
                    await vm.LoadHistory();
                }
                else
                    Console.WriteLine("not 1");
            }
        }
    }
}
