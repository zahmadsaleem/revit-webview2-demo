using RevitWebView2Demo.Properties;
using System;
using System.Windows;


namespace RevitWebView2Demo
{
    public partial class WebView2Window : Window
    {
        public WebView2Window()
        {
            InitializeComponent();
            var win = new WebViewPage
            {
                webView =
                {
                    Source = new Uri(Settings.Default.WEBPATH)
                }
            };
            win.Unloaded += (sender, e) => SelectExternalEventHandler.Subscribers.Remove(win.Wv2SelectionChanged);
            SelectExternalEventHandler.Subscribers.Add(win.Wv2SelectionChanged);
            mainwindow.Content = win;
        }
    }
}
