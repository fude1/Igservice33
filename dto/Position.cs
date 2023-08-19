using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGWebApi.DTOs
{
    public class Position
    {
        public string date { get; set; }

        public decimal? size { get; set; }

        public string epic { get; set; }

        public string direction { get; set; }

        public string dealId { get; set; }

        public bool forceOpen { get; set; }

        public string currencyCode { get; set; }

        public decimal? price { get; set; }

        public decimal? limitLevel { get; set; }

        public decimal? stopLevel { get; set; }

         public decimal? LimitDistance { get; set; }

        public decimal? StopDistance { get; set; }

    }
}
