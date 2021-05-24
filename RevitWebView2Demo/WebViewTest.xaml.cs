using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace RevitWebView2Demo
{
    /// <summary>
    /// Interaction logic for WebViewTest.xaml
    /// </summary>
    public partial class WebViewTest : Page
    {
        public static UIDocument uidoc;
        public WebViewTest()
        {
            this.InitializeComponent();
        }
        
        internal abstract class WebInvokeAction
        {
            public string ActionName;
        }
        internal class SelectionResult : WebInvokeAction
        {
            public List<string> ElementGuids;
        }
        private void OnWebViewInteraction(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            
            try
            {
                var result = JsonConvert.DeserializeObject<SelectionResult>(e.WebMessageAsJson);
                var elementIds = result.ElementGuids
                    .Select(x => uidoc.Document.GetElement(x)?.Id)
                    .Where(x=> x!=null).ToList() ;
                uidoc.Selection.SetElementIds(elementIds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }

        public async void SendMessage(List<string> elids)
        {
            var postMessage = new
            {
                ElementIds = elids
            };
            try
            {
               await Dispatcher.InvokeAsync( ()=> webView.ExecuteScriptAsync($"receiveMessage({ JsonConvert.SerializeObject(postMessage)})"));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

        }

    }
    public class PaneProvider : IDockablePaneProvider
    {
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElementCreator = new BrowserCreator();
            // data.InitialState = new DockablePaneState
            // {
            //     DockPosition = DockPosition.Tabbed,
            //     TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
            // };
        }
    }

    internal class BrowserCreator : IFrameworkElementCreator
    {
        public WebViewTest win;
        // 
        // Implement the creation call back by returning a
        // new WebBrowser each time the callback is triggered. 
        // 
        public FrameworkElement CreateFrameworkElement()
        {
            win = new WebViewTest();
            win.Unloaded += (sender,e) => SelectExternalEventHandler.Subscribers.Remove(win.SendMessage);
            // win.GotFocus += (s,e)=> App.SelectEvent.Raise();
            SelectExternalEventHandler.Subscribers.Add(win.SendMessage);
            return win;
        }
    }
}
