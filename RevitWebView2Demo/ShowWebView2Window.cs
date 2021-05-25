using System;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{
    [Transaction(TransactionMode.ReadOnly)]
    public class ShowWebView2Window: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var win = new WebView2Window();
            win.Show();
            return Result.Succeeded;
        }
    }
}