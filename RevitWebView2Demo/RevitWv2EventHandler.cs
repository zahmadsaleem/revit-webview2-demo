using System.Diagnostics;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{
    public class RevitWv2EventHandler : IExternalEventHandler
    {
        public enum RevitWv2ActionsEnum
        {
            Invalid = -1,
            CreateSheet
        }

        private readonly ExternalEvent mainEvent;
        private RevitWv2ActionsEnum _currentRevitWv2Actions;
        public RevitWv2EventHandler()
        {

            mainEvent = ExternalEvent.Create(this);
        }

        public void Execute(UIApplication app)
        {
            switch (_currentRevitWv2Actions)
            {
                case RevitWv2ActionsEnum.CreateSheet:
                    WebViewTest.HandleCreateSheet(app.ActiveUIDocument);
                    break;
                default:
                    Debug.WriteLine("RevitWv2EventHandler action not defined");
                    break;
            }
            /* this shit wont work if its asynchronous ; but revit has a event queue I suppose*/
            _currentRevitWv2Actions = RevitWv2ActionsEnum.Invalid;
        }

        public string GetName()
        {
            return nameof(RevitWv2EventHandler);
        }

        public ExternalEventRequest Raise(RevitWv2ActionsEnum revitWv2ActionsName)
        {
            _currentRevitWv2Actions = revitWv2ActionsName;
            return mainEvent.Raise();
        }
    }
}