using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Net;
using System.Text;
using System.Xml;

namespace web_scraper
{
    public static class LmnpSimulatorService
    {
        public static LmnpSimuatorResult GetSimulatorResult(decimal prixBien, decimal loyer, decimal surface, decimal annee, decimal apportPero, long chargeAnnuelle)
        {
            var nameValueCollection = new NameValueCollection();

            nameValueCollection.Add("dureeSimulation", annee.ToString().Replace(" ", ""));
            nameValueCollection.Add("dateAchatLogement","01/10/2020");
            nameValueCollection.Add("dateDebutLocation", "01/12/2020");
            nameValueCollection.Add("prixLogement",prixBien.ToString().Replace(" ", ""));
            nameValueCollection.Add("pourcentTerrains","15%");
            nameValueCollection.Add("pourcentConstructions","85%");
            nameValueCollection.Add("fraisNotaires",((prixBien/100)*8).ToString().Replace(" ", ""));
            nameValueCollection.Add("dureeAmortissementsConstructions", annee.ToString().Replace(" ", ""));
            nameValueCollection.Add("apportPersonnel",apportPero.ToString().Replace(" ", ""));
            nameValueCollection.Add("montantPret", (prixBien - apportPero).ToString().Replace(" ", ""));
            nameValueCollection.Add("dureePret", annee.ToString().Replace(" ", ""));
            nameValueCollection.Add("tauxPret","1,35%");
            nameValueCollection.Add("montantAssurance","35");
            nameValueCollection.Add("prixAchatEquipements","2500");
            nameValueCollection.Add("dureeAmortissementsEquipements","7");
            nameValueCollection.Add("loyerMensuel", loyer.ToString().Replace(" ", ""));
            nameValueCollection.Add("chargesAnnuellesCopro", ((chargeAnnuelle==0)?25 * surface:chargeAnnuelle).ToString().Replace(" ", ""));
            nameValueCollection.Add("autresChargesAnnuelles","600");
            nameValueCollection.Add("impotsTaxes", (25*surface).ToString().Replace(" ", ""));
            nameValueCollection.Add("revalorisationMontant", "non");
            //nameValueCollection.Add("impotsTaxes",10);
            //nameValueCollection.Add("revalorisationPrixLogement",10);
            //nameValueCollection.Add("revalorisationPrixEquipements",10);
            //nameValueCollection.Add("revalorisationLoyer",10);
            //nameValueCollection.Add("revalorisationCharges",10);
            //nameValueCollection.Add("revalorisationImpotsTaxes",10);
            nameValueCollection.Add("tauxMarginalImposition","30%");
            
            var webClient = new WebClient();
            byte[] responseArray = webClient.UploadValues("http://www.lmnp-simulation.fr/lmnp/simulerlmnp", nameValueCollection);
            string response = Encoding.UTF8.GetString(responseArray);
            var text = response;
            String[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            bool itsthetable=false;
            StringBuilder builder = new StringBuilder();
            foreach (var line in lines)
            {
                if (line.Contains("Synthèse résultat de simulation par année"))
                    itsthetable = true;
                else if (itsthetable && line.Contains("</table"))
                {
                    builder.AppendLine(line);
                    break;
                }
                else if(itsthetable)
                {
                    builder.AppendLine(line);
                }
                
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(builder.ToString().Replace("<b>","").Replace("<br>", "").Replace("</b>", ""));

            /*DataTable dt = new DataTable();

            dt.Columns.Add("Annee");
            dt.Columns.Add("ApportPerso.");
            dt.Columns.Add("Equipemts");
            dt.Columns.Add("EcheancePret");
            dt.Columns.Add("ChargesImpots");
            dt.Columns.Add("Taxes");
            dt.Columns.Add("IR/Benef");
            dt.Columns.Add("Loyers");
            dt.Columns.Add("Dépenses");
            dt.Columns.Add("Recettes");
            dt.Columns.Add("Solde");
            dt.Columns.Add("SoldeCumule");*/
            List<List<decimal>> table = new List<List<decimal>>();


            //Start after header and before footer (sum)
            for (int rowIndex = 1; rowIndex < doc.DocumentElement.ChildNodes.Count - 1; rowIndex++)
            {
                var ndRow = doc.DocumentElement.ChildNodes[rowIndex];
                table.Add(new List<decimal>());

                for (int colIndex = 0; colIndex < ndRow.ChildNodes.Count; colIndex++)
                {
                    var text2 = ndRow.ChildNodes[colIndex].InnerText;
                    if (string.IsNullOrEmpty(text2) || "Total".Equals(text2))
                    {
                        text2 = "0";
                    }
                    table[rowIndex - 1].Add(decimal.Parse(text2.Replace(",", "").Replace(".", ",")));

                }

                

            }

            //Solde à la fin de l'année, prend une ligne au hasarad
            var beneficeParMois = -(table[1][10]) / 12;
            var beneficeSansRevente = -(table[doc.DocumentElement.ChildNodes.Count - 3][10]);
            var beneficeAvecRevente = prixBien - apportPero - (table[doc.DocumentElement.ChildNodes.Count - 3][10]);

            return new LmnpSimuatorResult(beneficeParMois, beneficeSansRevente, beneficeAvecRevente);
        }
    }
}
