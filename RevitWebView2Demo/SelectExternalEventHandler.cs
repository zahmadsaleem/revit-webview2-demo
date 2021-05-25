using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RevitWebView2Demo
{

    public class SelectExternalEventHandler : IExternalEventHandler
    {
        private readonly ExternalEvent _externalEvent;
        private List<string> _selectedIds;
        private HashSet<string> _previousSelection = new HashSet<string>();
        public static List<Action<List<string>>> Subscribers = new List<Action<List<string>>>();

        public SelectExternalEventHandler()
        {
            _externalEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication app)
        {
            WebViewPage.uidoc = app.ActiveUIDocument;
            var elids = app.ActiveUIDocument.Selection.GetElementIds();
            _selectedIds = elids
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
                    action(_selectedIds);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }

            }

            _previousSelection = new HashSet<string>(_selectedIds);
        }

        public string GetName()
        {
            return nameof(SelectExternalEventHandler);
        }

        private bool ElementSelectionChanged()
        {
            return !_previousSelection.SetEquals(_selectedIds);
        }

        public ExternalEventRequest Raise()
        {
            return _externalEvent.Raise();
        }
    }
}
