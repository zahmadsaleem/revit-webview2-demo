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

        internal class WVRecieveAction
        {
            public string action;
            public object payload;
        }

        internal class SelectionResult : WVRecieveAction
        {
            public List<string> payload;
        }

        private void OnWebViewInteraction(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var result = JsonConvert.DeserializeObject<WVRecieveAction>(e.WebMessageAsJson);
            switch (result.action)
            {
                case "select":
                    HandleSelect(e.WebMessageAsJson);
                    break;
                case "create-sheet":
                    App.RevitWv2Event.Raise(RevitWv2EventHandler.RevitWv2ActionsEnum.CreateSheet);
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
                var elementIds = selectresult.payload
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
            public PostMessage(Wv2SendAction actionName, object actionPayload)
            {
                action = Enum.GetName(typeof(Wv2SendAction), actionName);
                payload = actionPayload;
            }

            public string action { get; private set; }

            public object payload { get; private set; }
        }

        public enum Wv2SendAction
        {
            SelectionChanged
        }

        public async void SendMessage(PostMessage message)
        {
            try
            {
                await Dispatcher.InvokeAsync(
                    () => webView
                        .ExecuteScriptAsync(
                            $"dispatchWebViewEvent({JsonConvert.SerializeObject(message)})"
                        )
                );
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }

        public void Wv2SelectionChanged(List<string> elids)
        {
            var postMessage = new PostMessage(Wv2SendAction.SelectionChanged, elids);
            SendMessage(postMessage);
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
            win.webView.Source = new Uri(RevitWebView2Demo.Properties.Settings.Default.WEBPATH);
            win.Unloaded += (sender, e) => SelectExternalEventHandler.Subscribers.Remove(win.Wv2SelectionChanged);
            // todo: sync and manual mode
            // win.GotFocus += (s,e)=> App.SelectEvent.Raise();
            SelectExternalEventHandler.Subscribers.Add(win.Wv2SelectionChanged);
            return win;
        }
    }

}