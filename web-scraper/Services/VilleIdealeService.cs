using CefSharp.MinimalExample.WinForms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using web_scraper.Model;
using web_scraper.Utils;

namespace web_scraper.Services
{
    public class VilleIdealeService
    {
        public static VilleIdealeService Instance = new VilleIdealeService();

        private Dictionary<string, string> villes = new Dictionary<string, string>();
        public VilleIdealeService()
        {
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("Villes.csv"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    var ville = line.Split(';');

                    if (ville.Length > 1)
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

        public VilleIdealeResult TensionMarket(BrowserForm form, string ville)
        {
            VilleIdealeResult result = new VilleIdealeResult();
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
            else if (villes.ContainsKey(name.Replace("-", " ")))
                codeInsee = villes[name.Replace("-", " ")];
            else if (villes.ContainsKey(name.Replace("SAINT ", "ST ")))
                codeInsee = villes[name.Replace("SAINT ", "ST ")];
            else if (villes.ContainsKey(name.Replace("SAINT-", "ST-").Replace("-", " ")))
                codeInsee = villes[name.Replace("SAINT-", "ST-").Replace("-", " ")];

            if (!string.IsNullOrEmpty(codeInsee))
            {
               /* var url = "https://www.ville-ideale.fr/" + name.Replace(" ", "_").ToLower() + "_" + codeInsee;
                using (WebClient client = new WebClient())
                {
                    form.LoadUrl(url);
                    System.Threading.Thread.Sleep(3000);
                    result.Environnement = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[1]/td[1]").ParseNullableInt();
                    result.Transports = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[2]/td[1]").ParseNullableInt();
                    result.Sécurite = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[3]/td[1]").ParseNullableInt();
                    result.Sante = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[4]/td[1]").ParseNullableInt();
                    result.SportsEtLoisirs = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[5]/td[1]").ParseNullableInt();
                    result.Culture = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[6]/td[1]").ParseNullableInt();
                    result.Enseignement = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[7]/td[1]").ParseNullableInt();
                    result.Commerces = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[8]/td[1]").ParseNullableInt();
                    result.QualiteDeVie = WebUtils.retrieveData(form, "//table[@id='tablonotes']/tbody/tr[9]/td[1]").ParseNullableInt();
                }*/
            }
            else
            {
                //Debugger.Break();
            }
            return result;
        }
    }
}
