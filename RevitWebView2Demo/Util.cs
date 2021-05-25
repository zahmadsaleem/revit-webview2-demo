using System;
using System.Diagnostics;

namespace RevitWebView2Demo
{
    public static class Util
    {
        public static void HandleError(Exception ex)
        {
            Debug.WriteLine(ex.Message);
            Debug.WriteLine(ex.Source);
            Debug.WriteLine(ex.StackTrace);
        }
    }
}