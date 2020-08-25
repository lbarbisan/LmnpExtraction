using CefSharp.MinimalExample.WinForms;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace web_scraper
{
    public class WebUtils
    {
        public static string retrieveData(BrowserForm form, string xpathExpression)
        {
            string result = "";
            var quartierTask = form.EvaluateScriptAsync(xpathExpression).ContinueWith(x =>
            {
                result = x.Result;
            });

            Task.WaitAll(quartierTask);
            return result;
        }
    }
}
