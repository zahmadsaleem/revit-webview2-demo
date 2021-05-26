using System;
using System.Windows;
using Autodesk.Revit.UI;
using RevitWebView2Demo.Properties;
namespace RevitWebView2Demo
{
    public class DockPanelHelpers
    {
        public static readonly DockablePaneId PanelGuid = new DockablePaneId(new Guid("FC603B95-4116-44EF-834F-3E0BBD76C05E"));
        private static string _webView2DockPanelName = "Webview2 DockPanel";
        public static void CreateDockPanel(UIControlledApplication app)
        {
            var pane = new DockPanelHelpers.PaneProvider();
            app.RegisterDockablePane(PanelGuid, _webView2DockPanelName, pane);
        }
        public class PaneProvider : IDockablePaneProvider
        {
            public void SetupDockablePane(DockablePaneProviderData data)
            {
                data.FrameworkElementCreator = new BrowserCreator();
            }
        }

        internal class BrowserCreator : IFrameworkElementCreator
        {
            public WebViewPage win;

            public FrameworkElement CreateFrameworkElement()
            {
                win = new WebViewPage
                {
                    webView =
                    {
                        Source = new Uri(Settings.Default.WEBPATH)
                    }
                };
                return win;
            }
        }
    }
}