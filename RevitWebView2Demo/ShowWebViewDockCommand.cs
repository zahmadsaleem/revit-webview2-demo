using System;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{

    [Transaction(TransactionMode.ReadOnly)]
    public class ShowWebViewDockCommand :IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                DockablePane dp = commandData.Application.GetDockablePane(DockPanelHelpers.PanelGuid);
                dp.Show();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return Result.Succeeded;
        }

    }
}