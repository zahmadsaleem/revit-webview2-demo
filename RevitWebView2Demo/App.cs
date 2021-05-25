using System;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{
    internal class App : IExternalApplication
    {
        public static App ThisApp;
        public static SelectExternalEventHandler SelectEvent;
        public static RevitWv2EventHandler RevitWv2Event;


        private static readonly Timer SelectionCheckTimer = new Timer();

        public Result OnStartup(UIControlledApplication a)
        {
            ThisApp = this;

            SelectEvent = new SelectExternalEventHandler();
            RevitWv2Event = new RevitWv2EventHandler();

            DockPanelHelpers.CreateDockPanel(a);


            var panel = CreateRibbonPanel(a);
            
            var thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            if (panel.AddItem(
                new PushButtonData("WV2", "Show\nPanel", thisAssemblyPath,
                    "RevitWebView2Demo.WebViewTestCommand")) is PushButton button3)
            {
                var uriImage = new Uri("pack://application:,,,/RevitWebView2Demo;component/Resources/code-small.png");
                var largeImage = new BitmapImage(uriImage);
                button3.LargeImage = largeImage;
            }


            StartSelectionRaiseTimer();
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            StopSelectionRaiseTimer();
            return Result.Succeeded;
        }

        private void StartSelectionRaiseTimer()
        {
            SelectionCheckTimer.Elapsed += (s, e) => SelectEvent.Raise();
            SelectionCheckTimer.Interval = 750;
            SelectionCheckTimer.Enabled = true;
        }

        private void StopSelectionRaiseTimer()
        {
            SelectionCheckTimer.Enabled = false;
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
                Util.HandleError(ex);
            }

            try
            {
                ribbonPanel = a.CreateRibbonPanel(tabName, "Default");
                return ribbonPanel;
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
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