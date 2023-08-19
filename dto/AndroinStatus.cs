using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGService3.DTOs
{
    internal class AndroinStatus
    {
        public string Date { get; set; }

        public string Trend1m { get; set; }

        public string Trend15m { get; set; }

        public string Trend1h { get; set; }

        public string SP500Offer { get; set; }

        public string SP500Bid { get; set; }

        public string SP500Perc { get; set; }

        public string Bolinger { get; set; }

        public string Rsi1m { get; set; }

        public string Rsi15m { get; set; }

        public string Rsi1h { get; set; }

        public string DailyBalance { get; set; }
    }
}
