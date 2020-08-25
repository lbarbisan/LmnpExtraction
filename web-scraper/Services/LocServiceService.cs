using CefSharp.MinimalExample.WinForms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using web_scraper.Model;

namespace web_scraper.Services
{
    public class LocServiceService
    {
        public static LocServiceService Instance = new LocServiceService();

        private Dictionary<string, string> villes = new Dictionary<string, string>();
        public LocServiceService()
        {
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("Villes.csv"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    var ville = line.Split(';');
                    
                    if (ville.Length>1)
                    {
                        var key = RemoveDiacritics(ville[1]).ToUpper().Replace("-", " ");
                        var value = RemoveDiacritics(ville[0]).ToUpper();
                        if (!villes.ContainsKey(key))
                            villes.Add(key, value);
                    }
                }
            }
        }


        private static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public LocationTension TensionMarket(BrowserForm form, string ville)
        {
            LocationTension tension = new LocationTension();
            var name = RemoveDiacritics(ville).ToUpper();
            if (name.StartsWith("PARIS") && name.EndsWith("EME"))
            {
                name = name.Replace("EME", "");
                if (name.Length == 7)
                    name = name.Replace(" ", " 0");
            }
            string codeInsee = null;
            if (villes.ContainsKey(name))
                codeInsee = villes[name];
            else if(villes.ContainsKey(name.Replace("-", " ")))
                codeInsee = villes[name.Replace("-", " ")];
            else if (villes.ContainsKey(name.Replace("SAINT ", "ST ")))
                codeInsee = villes[name.Replace("SAINT ", "ST ")];
            else if (villes.ContainsKey(name.Replace("SAINT-", "ST-").Replace("-", " ")))
                codeInsee = villes[name.Replace("SAINT-", "ST-").Replace("-", " ")];

            if (!string.IsNullOrEmpty(codeInsee))
            {
                var url = "https://www.locservice.fr/tensiometre/tensiometre-" + codeInsee + ".html";
                using (WebClient client = new WebClient())
                {
                    form.LoadUrl(url);
                    System.Threading.Thread.Sleep(1500);
                    ///images/tensiometre/fleche8.png
                    var easy = WebUtils.retrieveData(form, "//div[@id='result0']/table/tbody/tr[1]/td[1]/div/img[2]/@src");
                    var budget = WebUtils.retrieveData(form, "//div[@id='result0']/table/tbody/tr[2]/td[1]/div/img[2]/@src");
                    if (!string.IsNullOrEmpty(easy))
                        tension.EasyLocation = long.Parse(easy[26].ToString());
                    if (!string.IsNullOrEmpty(budget))
                        tension.BudgetLocation = long.Parse(budget[26].ToString());
                }
            }
            else
            {
                //Debugger.Break();
            }
            return tension;
        }
    }
}
