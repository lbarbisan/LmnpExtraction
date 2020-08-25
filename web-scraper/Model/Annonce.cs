using System;
using System.Collections.Generic;
using System.Text;

namespace web_scraper
{
    public class Annonce
    {
        private long ChargeAnnuelle { get; set; }

        public Annonce(DateTime date, string url)
        {
            Date = date;
            this.url = url;
        }

        public Annonce(DateTime date, string url, long chargeAnnuelle) : this(date, url)
        {
            
            this.ChargeAnnuelle = chargeAnnuelle;
        }

        public string url { get; set; }
        public DateTime Date { get; set; }
    }
}
