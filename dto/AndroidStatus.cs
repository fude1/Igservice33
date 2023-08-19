using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGService3.DTOs
{
    internal class AndroidStatus
    {
        public string Date { get; set; }

        public double Close { get; set; }

        public double Basis { get; set; }

        public double Upper { get; set; }

        public double Lower { get; set; }

        public bool bSell  { get; set; }

        public bool bStrongSell { get; set; }

        public bool bBuy  { get; set; }
   
        public bool bStrongBuy { get; set; }

        public bool bMidleH  { get; set; }

        public bool bMidleL  { get; set; }

        public int mSequentialUpper { get; set; }

        public int mSequentialMidle { get; set; }

        public int mSequentialLower { get; set; }

        public int mSequenzaAlgotirmo { get; set; }

        public string candleDirection { get; set; }

        public double bolingerOscillator { get; set; }
    }
}
