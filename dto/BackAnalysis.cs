using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGService3.DTOs
{
    public class BackAnalysis
    {
        public string foglio { get; set; }

        public double Size { get; set; }

        public double Tempo { get; set; }

        public double ValT { get; set; }

        public double S3 { get; set; }

        public double S2 { get; set; }

        public double S1 { get; set; }

        public double PV { get; set; }

        public double R1 { get; set; }

        public double R2 { get; set; }

        public double R3 { get; set; }

        public double Bo { get; set; }

        public double BC { get; set; }

        public double Bsg { get; set; }

        public double Bsl { get; set; }

        public double So { get; set; }

        public double SC { get; set; }

        public double Ssg { get; set; }

        public double Ssl { get; set; }

        public double Slop { get; set; }

        public double Trend { get; set; }

        public double Mark { get; set; }

        public double RSI { get; set; }

        public double VolV { get; set; }

        public double GainPerc { get; set; }

        public double Gain { get; set; }

        public int StartO { get; set; }

        public int StopO { get; set; }

        public double Spread { get; set; }

        public double VolM { get; set; }
    }
}
