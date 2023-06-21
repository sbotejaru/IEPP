using IEPP.Models;
using IEPP.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private bool controlLoaded = false;
        private bool historyLoaded = false;
        private int scrollNumber;
        private int startIndex = 0;

        public SettingsControl(MainVM mainVM)
        {
            InitializeComponent();
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainVM;
            scrollNumber = 1;
        }

        public SettingsControl(MainVM mainVM, int tabIndex)
        {
            InitializeComponent();
            vm = DataContext as TabContentVM;
            vm.MainWinDC = mainVM;
            startIndex = tabIndex;
            scrollNumber = 1;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            CurrentSessionHistoryList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            PreviousSessionsHistoryList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            BookmarksList.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            if (!controlLoaded)
            {
                controlLoaded = true;
                vm.SelectedSettingsTab = startIndex;
            }

            if (vm.MainWinDC.JsonHelper.NoMoreHistoryItems)
                RemoveHistoryLoadingText();
        }

        public void RemoveHistoryLoadingText()
        {
            vm.HistoryLoadingTextVisibility = Visibility.Collapsed;
        }

        private async void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (controlLoaded && !historyLoaded)
            {
                if (vm.SelectedSettingsTab == 1)
                {
                    await vm.LoadHistory();
                    historyLoaded = true;
                }
            }
        }

        private void HistoryScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            HistoryScrollViewer.ScrollToVerticalOffset(HistoryScrollViewer.VerticalOffset - e.Delta * 0.5);
            e.Handled = true;
        }

        private void HistoryScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (HistoryScrollViewer.VerticalOffset == HistoryScrollViewer.ScrollableHeight && vm.InitialHistoryDataLoaded)
            {
                if (!vm.MainWinDC.JsonHelper.NoMoreHistoryItems)
                {
                    using (var backgroundWorker = new BackgroundWorker())
                    {
                        backgroundWorker.DoWork += bgWorker_LoadTempHistoryData;
                        backgroundWorker.RunWorkerCompleted += bgWorker_LoadTempHistoryDataEnded;
                        backgroundWorker.RunWorkerAsync();                        
                    }
                }
                else
                    RemoveHistoryLoadingText();
            }
        }

        public void UpdateSelectedTab(int tabIndex)
        {
            vm.SelectedSettingsTab = tabIndex;
        }

        private void bgWorker_LoadTempHistoryData(object sender, DoWorkEventArgs e)
        {
            vm.MainWinDC.LoadHistory(scrollNumber);
        }

        private async void bgWorker_LoadTempHistoryDataEnded(object sender, RunWorkerCompletedEventArgs e)
        {
            await vm.MainWinDC.LoadTemporaryHistoryData();
            vm.MainWinDC.TemporaryHistoryData.Clear();
            ++scrollNumber;
        }
    }
}
