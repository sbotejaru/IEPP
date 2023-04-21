using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using System.Windows;
using CefSharp.Wpf;
using CefSharp;

namespace IEPP
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /*[STAThread]
        public static void Main()
        {
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }*/      
        public App()
        {
            CefSettings s = new CefSettings();
            s.CefCommandLineArgs.Add("disable-threaded-scrolling", "1");
            Cef.Initialize(s);
        }
    }
}
