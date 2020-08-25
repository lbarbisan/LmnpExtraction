/*
 * 1078132558801-k9ad3plm2nfir3mo04mor8du33153it9.apps.googleusercontent.com
 * FWQtkUxoUdRpl6D8wG-SXntQ
 * */
using CefSharp.MinimalExample.WinForms;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using HtmlAgilityPack;
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using web_scraper;
using web_scraper.Services;

namespace GmailQuickstart
{
    public class WebScraperService
    {
        public static void RunScrapping(BrowserForm browserForm, IList<Annonce> annonces)
        {
            var tensiometre = LocServiceService.Instance;
            var villeIdeale = VilleIdealeService.Instance;
            foreach (var annonce in annonces)
            {
                System.Threading.Thread.Sleep(5200);
                //using for IDisposable.Dispose()
                using (WebClient client = new WebClient())
                {
                    browserForm.LoadUrl(annonce.url);
                    System.Threading.Thread.Sleep(3450);
                    dynamic data = new System.Dynamic.ExpandoObject();
                    string achatUrl = null;
                    
                    data.quartier = WebUtils.retrieveData(browserForm, "//div[@class='Summarystyled__Address-tzuaot-5 jqlODu']/span");
                    data.prix = WebUtils.retrieveData(browserForm, "(//span[@class='global-styles__TextNoWrap-sc-1aeotog-6 dVzJN'])[1]");
                    var ressult = WebUtils.retrieveData(browserForm, "//div[@class='Summarystyled__Address-tzuaot-5 jqlODu']");
                    if (ressult.Contains(','))
                        data.lieu = ressult.Split(',')[1].Trim();
                    else
                        data.lieu = ressult;
                    ressult = WebUtils.retrieveData(browserForm, "concat((//div[@class='EstateSummary__SummaryText-sc-13fov8-0 bullet EstateSummary__SummaryTag-sc-13fov8-8 hOzXeX'])[0] , '_', (//div[@class='EstateSummary__SummaryText-sc-13fov8-0 bullet EstateSummary__SummaryTag-sc-13fov8-8 hOzXeX'])[1] , '_', (//div[@class='EstateSummary__SummaryText-sc-13fov8-0 bullet EstateSummary__SummaryTag-sc-13fov8-8 hOzXeX'])[2] , '_',  (//div[@class='EstateSummary__SummaryText-sc-13fov8-0 bullet EstateSummary__SummaryTag-sc-13fov8-8 hOzXeX'])[3])");
                    data.surface = ressult.Split('_').FirstOrDefault((x) => (x.Contains("²")));
                    data.type = ressult.Split('_').FirstOrDefault((x) => (x.Contains("pi")));
                    achatUrl = WebUtils.retrieveData(browserForm, "//a[@class='RealEstatePriceLink__Link-sc-1wcupem-0 wdjdG']/@href");
                    var locationUrl = achatUrl.Replace("/vente/", "/location/");

                    browserForm.LoadUrl(achatUrl);
                    System.Threading.Thread.Sleep(3450);
                    data.vendu = WebUtils.retrieveData(browserForm, "//div[contains(@title,'vendus') and @class='sc-iBEsjs coLLbW']");
                    data.avendre  = WebUtils.retrieveData(browserForm, "//div[contains(@title,'marché') and @class='sc-iBEsjs coLLbW']");
                    data.ventem2  = WebUtils.retrieveData(browserForm, "//div[@class='sc-dvCyap llazGV']");
                    data.nombreLogement  = WebUtils.retrieveData(browserForm, "//div[@data-testid='market.numbers-total-residence']").Replace(" ","");
                        
                    browserForm.LoadUrl(locationUrl);
                    System.Threading.Thread.Sleep(5049);
                    //data.locationm2 = WebUtils.retrieveData(browserForm, "//div[@class='sc-dvCyap llazGV']");
                    data.locationm2 = WebUtils.retrieveData(browserForm, "(//div[@class='sc-dxZgTM jDZMAf'])[2]/div").Replace("Prix bas : ", "").Replace(" ", "").Replace("€", "");
                    data.aloue = WebUtils.retrieveData(browserForm, "//div[contains(@title,'marché') and @class='sc-iBEsjs coLLbW']");
                    data.recherche = WebUtils.retrieveData(browserForm, "//div[@class='sc-cugefK eIAcnA']");
                    
                    LmnpSimuatorResult result2515 = new LmnpSimuatorResult(), result2015 = new LmnpSimuatorResult(), result253000 = new LmnpSimuatorResult(), result203000 = new LmnpSimuatorResult();

                    if (!string.IsNullOrEmpty(data.prix) && !string.IsNullOrEmpty(data.locationm2) && !string.IsNullOrEmpty(data.surface))
                    {
                        var price = decimal.Parse(((string)data.prix).Replace("€", "").Replace(" ", ""));
                        var location = decimal.Parse(((string)data.locationm2).Replace("€", "").Replace("m²", "").Replace(" ", ""));
                        var surface = decimal.Parse(((string)data.surface).Replace("m²", "").Replace(" ", ""));

                        result2515 = LmnpSimulatorService.GetSimulatorResult(price, location * surface, surface, 25, 15000, 0);
                        System.Threading.Thread.Sleep(1000);
                        result2015 = LmnpSimulatorService.GetSimulatorResult(price, location * surface, surface, 20, 15000, 0);
                        System.Threading.Thread.Sleep(1000);
                        result253000 = LmnpSimulatorService.GetSimulatorResult(price, location * surface, surface, 25, 30000, 0);
                        System.Threading.Thread.Sleep(1000);
                        result203000 = LmnpSimulatorService.GetSimulatorResult(price, location * surface, surface, 20, 30000, 0);
                    }

                    var tension = tensiometre.TensionMarket(browserForm, data.lieu);
                    var notesVilles = villeIdeale.TensionMarket(browserForm, data.lieu);

                    web_scraper.Services.SheetService.Save(annonce.Date, data.lieu, data.quartier, data.prix, data.surface, data.ventem2, data.locationm2,
                            annonce.url, data.vendu, data.avendre, data.aloue, data.type, data.recherche, data.nombreLogement,
                            result2015.BeneficeParMois, result2015.BeneficeSansRevente, result2015.BeneficeAvecRevente,
                            result2515.BeneficeParMois, result2515.BeneficeSansRevente, result2515.BeneficeAvecRevente,
                            result253000.BeneficeParMois, result253000.BeneficeSansRevente, result253000.BeneficeAvecRevente,
                            result203000.BeneficeParMois, result203000.BeneficeSansRevente, result203000.BeneficeAvecRevente, 
                            tension.EasyLocation, tension.BudgetLocation,
                            notesVilles.Environnement,
                            notesVilles.Transports,
                            notesVilles.Sécurite,
                            notesVilles.Sante,
                            notesVilles.SportsEtLoisirs,
                            notesVilles.Culture,
                            notesVilles.Enseignement,
                            notesVilles.Commerces,
                            notesVilles.QualiteDeVie);
                }
            }
        }
    }
}