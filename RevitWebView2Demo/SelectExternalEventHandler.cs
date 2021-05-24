using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{

    public class SelectExternalEventHandler : IExternalEventHandler
    {
        private readonly ExternalEvent mainEvent;
        private List<string> SelectedIds;
        private HashSet<string> PreviousSelection = new HashSet<string>();
        public static List<Action<List<string>>> Subscribers = new List<Action<List<string>>>();

        public SelectExternalEventHandler()
        {
            mainEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication app)
        {
            WebViewTest.uidoc = app.ActiveUIDocument;
            var elids = app.ActiveUIDocument.Selection.GetElementIds();
            SelectedIds = elids
                .Select(x=> app.ActiveUIDocument.Document.GetElement(x).UniqueId)
                .ToList();

            if (!ElementSelectionChanged())
            {
                return;
            }

            foreach (var action in Subscribers)
            {
                try
                {
                    action(SelectedIds);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            }
            PreviousSelection = new HashSet<string>(SelectedIds);
        }

        public string GetName()
        {
            return nameof(SelectExternalEventHandler);
        }

        private bool ElementSelectionChanged()
        {
            return !PreviousSelection.SetEquals(SelectedIds);
        }

        public ExternalEventRequest Raise()
        {
            return mainEvent.Raise();
        }
    }
}
