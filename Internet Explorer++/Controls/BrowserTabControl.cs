using IEPP.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace IEPP.Controls
{
    public class BrowserTabControl : TabControl
    {
        public static readonly DependencyProperty NewTabCommandProperty =
            DependencyProperty.Register("NewTabCommand", typeof(ICommand), typeof(BrowserTabControl), new PropertyMetadata(null));        

        public static readonly DependencyProperty MaxTabWidthProperty =
            DependencyProperty.Register("MaxTabWidth", typeof(double), typeof(BrowserTabControl), new PropertyMetadata(null));

        public double MaxTabWidth
        {
            get { return (double)GetValue(MaxTabWidthProperty); }
            set { SetValue(MaxTabWidthProperty, value); }
        }

        public ICommand NewTabCommand
        {
            get { return (ICommand)GetValue(NewTabCommandProperty); }
            set { SetValue(NewTabCommandProperty, value); }
        }         

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BrowserTab();
        }

        public BrowserTabControl()
        {
            //Bookmarks = new ObservableCollection<Bookmark>();            
        }
    }
}
