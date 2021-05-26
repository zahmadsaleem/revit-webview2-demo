using Autodesk.Revit.UI;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace RevitWebView2Demo
{
    public partial class WebViewPage : Page
    {
        public static UIDocument uidoc;

        public WebViewPage()
        {
            this.InitializeComponent();
            App.SelectEvent.Subscribers.Add(Wv2SelectionChanged);
            StartSelectionRaiseTimer();
        }

        internal class WvReceiveAction
        {
            public string action;
            public object payload;
        }

        internal class SelectionResult : WvReceiveAction
        {
            public List<string> payload;
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
        private void OnWebViewInteraction(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            WvReceiveAction result = null;
            try
            {
                result = JsonConvert.DeserializeObject<WvReceiveAction>(e.WebMessageAsJson);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
                
            
            if (result == null)
            {
                return;
            }
            
            switch (result.action)
            {
                case "select":
                    WebView2EventHandlers.HandleSelect(uidoc, e.WebMessageAsJson);
                    break;
                case "create-sheet":
                    App.RevitWv2Event.Raise(RevitWv2EventHandler.RevitWv2ActionsEnum.CreateSheet);
                    break;
                default:
                    Debug.WriteLine("action not defined");
                    break;
            }
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
            var elements = elids.Select(x =>
            {
                var el = uidoc.Document.GetElement(x);
                return new
                {
                    name = el.Name,
                    id = x
                };
            });
            var postMessage = new PostMessage(Wv2SendAction.SelectionChanged, elements);
            SendMessage(postMessage);
        }
        private DispatcherTimer _selectionCheckTimer = null;

        private void StartSelectionRaiseTimer()
        {
            _selectionCheckTimer = new DispatcherTimer();
            _selectionCheckTimer.Tick += (s, e) => Dispatcher.Invoke(() => App.SelectEvent.Raise());
            _selectionCheckTimer.Interval = TimeSpan.FromMilliseconds(750);
            _selectionCheckTimer.Start();
        }

        private void StopSelectionRaiseTimer()
        {
            _selectionCheckTimer.Stop();
        }

        private void WebView2Window_OnClosing(object sender, CancelEventArgs e)
        {
            ShowWebView2Window.Win = null;
            StopSelectionRaiseTimer();
        }

        private void WebView_OnUnloaded(object sender, RoutedEventArgs e)
        {
            App.SelectEvent.Subscribers.Remove(Wv2SelectionChanged);
            StopSelectionRaiseTimer();
        }
    }

}