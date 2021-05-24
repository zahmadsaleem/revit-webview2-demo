using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;

namespace RevitWebView2Demo
{
    public partial class WebViewTest : Page
    {
        public static UIDocument uidoc;
        public WebViewTest()
        {
            this.InitializeComponent();
        }
        
        internal class WebInvokeAction
        {
            public string ActionName;
        }
        internal class SelectionResult : WebInvokeAction
        {
            public List<string> ElementGuids;
        }
        private void OnWebViewInteraction(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var result = JsonConvert.DeserializeObject<WebInvokeAction>(e.WebMessageAsJson);
            switch (result.ActionName)
            {
                case "select":
                    HandleSelect(e.WebMessageAsJson);
                    break;
                case "create-sheet":
                    App.RevitExternalEvent.Raise();
                    break;
                default:
                    Debug.WriteLine("action not defined");
                    break;
            }

        }

        private void HandleSelect(string jsonMessage)
        {
            try
            {
                var selectresult = JsonConvert.DeserializeObject<SelectionResult>(jsonMessage);
                var elementIds = selectresult.ElementGuids
                    .Select(x => uidoc.Document.GetElement(x)?.Id)
                    .Where(x => x != null).ToList();
                uidoc.Selection.SetElementIds(elementIds);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public static void HandleCreateSheet(UIDocument udoc)
        {
            try
            {
                using (Transaction t = new Transaction(udoc.Document, "WebView Transaction"))
                {
                    t.Start();
                    var vs = ViewSheet.Create(uidoc.Document, ElementId.InvalidElementId);
                    vs.Name = "Sheet from WebView2";
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        public class PostMessage
        {
            public string action { get; set; }
    
            public object payload { get; set; }
        }
        public async void SendMessage(List<string> elids)
        {
            var postMessage = new PostMessage
            {
                action = "SELECTION_CHANGED",
                payload = elids
            };
            try
            {
               await Dispatcher.InvokeAsync( ()=> webView.ExecuteScriptAsync($"dispatchWebViewEvent({ JsonConvert.SerializeObject(postMessage)})"));
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

    public class RevitExternalEventHandler : IExternalEventHandler
    {
        private readonly ExternalEvent mainEvent;

        public RevitExternalEventHandler()
        {
            mainEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication app)
        {
            // todo: handle multiple
            WebViewTest.HandleCreateSheet(app.ActiveUIDocument);
        }

        public string GetName()
        {
            return nameof(RevitExternalEventHandler);
        }

        public ExternalEventRequest Raise()
        {
            // todo: take action name as arg
            return mainEvent.Raise();
        }
    }
}
