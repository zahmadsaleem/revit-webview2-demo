using System;
using System.Diagnostics;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;

namespace RevitWebView2Demo
{
    public static class WebView2EventHandlers
    {
        public static void HandleSelect(UIDocument udoc, string jsonMessage)
        {
            try
            {
                var selectionResult = JsonConvert.DeserializeObject<WebViewPage.SelectionResult>(jsonMessage);
                var elementIds = selectionResult.payload
                    .Select(x => udoc.Document.GetElement(x)?.Id)
                    .Where(x => x != null).ToList();
                udoc.Selection.SetElementIds(elementIds);
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
                    var vs = ViewSheet.Create(udoc.Document, ElementId.InvalidElementId);
                    vs.Name = "Sheet from WebView2";
                    t.Commit();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}