using System;
using System.Windows;
using Autodesk.Revit.UI;
using RevitWebView2Demo.Properties;
namespace RevitWebView2Demo
{
    public class DockPanelHelpers
    {
        public static readonly DockablePaneId PanelGuid = new DockablePaneId(new Guid("39FA492A-6F72-465C-83C9-F7662B89F62C"));
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
                win.Unloaded += (sender, e) => SelectExternalEventHandler.Subscribers.Remove(win.Wv2SelectionChanged);
                SelectExternalEventHandler.Subscribers.Add(win.Wv2SelectionChanged);
                return win;
            }
        }
    }
}