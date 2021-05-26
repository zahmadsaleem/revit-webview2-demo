using RevitWebView2Demo.Properties;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;


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
            mainwindow.Content = win;
        }

        private void WebView2Window_OnClosing(object sender, CancelEventArgs e)
        {
            ShowWebView2Window.Win = null;
        }
    }
}