using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{

    [Transaction(TransactionMode.ReadOnly)]
    public class WebViewTestCommand :IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var win = new WebViewTest();
            var dpid = new DockablePaneId(App.PanelGuid);
            DockablePane dp = commandData.Application.GetDockablePane(dpid);
            dp.Show();
            return Result.Succeeded;
        }

    }
}