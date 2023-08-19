using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.DTOs
{
    internal class AlgorithmStatus
    {
        public bool bBuyOpen { get; set; }
        public double BuyPrice { get; set; }
        public double BuySpread { get; set; }
        public bool bSellOpen { get; set; }
        public double SellPrice { get; set; }
        public double SellSpread { get; set; }
        public DateTime BuyDate { get; set; }
        public DateTime SellDate { get; set; }
        public double BuyVolume { get; set; }
        public double SellVolume { get; set; }
    }
}
