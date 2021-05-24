using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;


namespace RevitWebView2Demo
{

    class App : IExternalApplication
    {

        public static App ThisApp;
        public static SelectExternalEventHandler SelectEvent;
        public static RevitExternalEventHandler RevitExternalEvent;
        public static readonly Guid PanelGuid = new Guid("39FA492A-6F72-465C-83C9-F7662B89F62C");

        public Result OnStartup(UIControlledApplication a)
        {
            SelectEvent = new SelectExternalEventHandler();
            RevitExternalEvent = new RevitExternalEventHandler();
            var pane = new PaneProvider();
            var dpid = new DockablePaneId(PanelGuid);
            a.RegisterDockablePane(dpid, "Webview2 DockPanel", pane);

            ThisApp = this; 

            RibbonPanel panel = RibbonPanel(a);
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;

            if (panel.AddItem(
                new PushButtonData("WV2", "Show\nPanel", thisAssemblyPath,
                    "RevitWebView2Demo.WebViewTestCommand")) is PushButton button3)
            {
                Uri uriImage = new Uri("pack://application:,,,/RevitWebView2Demo;component/Resources/code-small.png");
                BitmapImage largeImage = new BitmapImage(uriImage);
                button3.LargeImage = largeImage;
            }


            StartSelectionRaiseTimer();
            a.ApplicationClosing += a_ApplicationClosing; 
            a.Idling += a_Idling;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {

            StopSelectionRaiseTimer();
            a.ApplicationClosing -= a_ApplicationClosing; 
            a.Idling -= a_Idling;
            return Result.Succeeded;
        }


        #region Idling & Closing

        /// <summary>
        /// What to do when the application is idling. (Ideally nothing)
        /// </summary>

        private static readonly System.Timers.Timer selectionCheckTimer = new System.Timers.Timer();
        void a_Idling(object sender, IdlingEventArgs e)
        {
        }

        void StartSelectionRaiseTimer()
        {
            selectionCheckTimer.Elapsed += new ElapsedEventHandler((s,e)=>SelectEvent.Raise());
            selectionCheckTimer.Interval = 750;
            selectionCheckTimer.Enabled = true;
        }

        void StopSelectionRaiseTimer()
        {
            selectionCheckTimer.Enabled = false;
        }
        /// <summary>
        /// What to do when the application is closing.)
        /// </summary>
        void a_ApplicationClosing(object sender, ApplicationClosingEventArgs e)
        {
        }

        #endregion

        #region Ribbon Panel

        public RibbonPanel RibbonPanel(UIControlledApplication a)
        {
            string tab = "WebView2Demo"; // Tab name
            // Empty ribbon panel 
            RibbonPanel ribbonPanel = null;
            // Try to create ribbon tab. 
            try
            {
                a.CreateRibbonTab(tab);
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
            }

            // Try to create ribbon panel.
            try
            {
                RibbonPanel panel = a.CreateRibbonPanel(tab, "Default");
            }
            catch (Exception ex)
            {
                Util.HandleError(ex);
            }

            // Search existing tab for your panel.
            List<RibbonPanel> panels = a.GetRibbonPanels(tab);
            foreach (RibbonPanel p in panels.Where(p => p.Name == "Default"))
            {
                ribbonPanel = p;
            }

            //return panel 
            return ribbonPanel;
        }

        #endregion
    }
}