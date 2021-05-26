using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitWebView2Demo
{
    [Transaction(TransactionMode.ReadOnly)]
    public class ShowWebView2Window: IExternalCommand
    {
        public static  WebView2Window Win = null;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            if (Win == null)
            {
                Win = new WebView2Window();
                Win.Show();
                
                return Result.Succeeded;
            }

            Win.Activate();
            return Result.Cancelled;
        }



    }
}