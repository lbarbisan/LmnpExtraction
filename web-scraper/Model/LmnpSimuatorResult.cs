using System;
using System.Collections.Generic;
using System.Text;

namespace web_scraper
{
    public struct LmnpSimuatorResult
    {
            public LmnpSimuatorResult(decimal beneficeParMois=0, decimal beneficeSansRevente=0, decimal beneficeAvecRevente=0)
        {
            this.BeneficeParMois = beneficeParMois;
            this.BeneficeSansRevente = beneficeSansRevente;
            this.BeneficeAvecRevente = beneficeAvecRevente;
        }

        public decimal BeneficeParMois { get; set; }
        public decimal BeneficeSansRevente { get; set; }
        public decimal BeneficeAvecRevente { get; set; }
    }
}

