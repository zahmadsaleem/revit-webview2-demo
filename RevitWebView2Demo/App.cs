using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Autodesk.Revit.UI;
using Timer = System.Timers.Timer;

namespace RevitWebView2Demo
{
    internal class App : IExternalApplication
    {
        public static App ThisApp;
        public static SelectExternalEventHandler SelectEvent;
        public static RevitWv2EventHandler RevitWv2Event;

        public Result OnStartup(UIControlledApplication a)
        {
            ThisApp = this;

            SelectEvent = new SelectExternalEventHandler();
            RevitWv2Event = new RevitWv2EventHandler();

            // DockPanelHelpers.CreateDockPanel(a);


            var panel = CreateRibbonPanel(a);
            
            var thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            /*if (panel.AddItem(
                new PushButtonData("WV2", "Show\nPanel", thisAssemblyPath,
                    "RevitWebView2Demo.ShowWebViewDockCommand")) is PushButton button3)
            {
                var uriImage = new Uri("pack://application:,,,/RevitWebView2Demo;component/Resources/code-small.png");
                var largeImage = new BitmapImage(uriImage);
                button3.LargeImage = largeImage;
            }*/
            

            if (panel.AddItem(
                new PushButtonData("WV2-1", "Show\nWindow", thisAssemblyPath,
                    "RevitWebView2Demo.ShowWebView2Window")) is PushButton button4)
            {
                var uriImage = new Uri("pack://application:,,,/RevitWebView2Demo;component/Resources/code-small.png");
                var largeImage = new BitmapImage(uriImage);
                button4.LargeImage = largeImage;
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }

        public RibbonPanel CreateRibbonPanel(UIControlledApplication a)
        {
            var tabName = "WebView2Demo";
            RibbonPanel ribbonPanel = null;
            try
            {
                a.CreateRibbonTab(tabName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            try
            {
                ribbonPanel = a.CreateRibbonPanel(tabName, "Default");
                return ribbonPanel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            var panels = a.GetRibbonPanels(tabName);
            foreach (var p in panels)
            {
                if (p.Name == "Default")
                {
                    return p;
                }
                    
            }
                

            return ribbonPanel;
        }
    }
}