﻿using IEPP.Utils;
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

        public static readonly DependencyProperty BookmarksSourceProperty =
            DependencyProperty.Register("BookmarksSource", typeof(ItemCollection), typeof(BrowserTabControl), new PropertyMetadata(null));

        public ICommand NewTabCommand
        {
            get { return (ICommand)GetValue(NewTabCommandProperty); }
            set { SetValue(NewTabCommandProperty, value); }
        }  
        
        public ItemCollection BookmarksSource
        {
            get { return (ItemCollection)GetValue(BookmarksSourceProperty); }
            set { SetValue(BookmarksSourceProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BrowserTab();
        }

       // public ObservableCollection<Bookmark> Bookmarks { get; set; }

        public RelayCommand AddBookmarkCommand { get; set; } = new RelayCommand(o =>
        {
            Console.WriteLine("works tab control");
        });

        public BrowserTabControl()
        {
            //Bookmarks = new ObservableCollection<Bookmark>();            
        }
    }
}
