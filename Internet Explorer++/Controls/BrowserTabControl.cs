using System;
using System.Collections.Generic;
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

        public ICommand NewTabCommand
        {
            get { return (ICommand)GetValue(NewTabCommandProperty); }
            set { SetValue(NewTabCommandProperty, value); }
        }        

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BrowserTab();
        }
    }
}
