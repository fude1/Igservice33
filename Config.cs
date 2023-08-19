using System;
using System.Collections.Generic;
using System.Text;


namespace IGConfig
{
    // Define a class that represents the structure of your YAML file
    public class MyConfig
    {
        public string Epic { get; set; }
        public string Monitor { get; set; }
        public string ApiTokenTrading { get; set; }
        public string ChatIdTrading { get; set; }
        public string ApiTokenMonitor { get; set; }
        public string ChatIdMonitor { get; set; }
        public string FattoreMoltiplicatore { get; set; }
        public string OraInizioGiornata { get; set; }
        public string OraFineGiornata { get; set; }
        public string GiorniChiusura { get; set; }
        public string Calendario { get; set; }
        public string VolumeSoglia { get; set; }
        public string SpreadSoglia { get; set; }
        public string Stopmonitor { get; set; }
        public string OpenPosition { get; set; }
        public string CloseAllPosition { get; set; }
        public string ClosePosition { get; set; }
        public string Database { get; set; }
        public string Strategy { get; set; }
        public string currencyCode { get; set; }
      	public string size { get; set; }
       	public string stopLevel { get; set; }
       	public string limitLevel { get; set; }
        public string env { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string apiKey { get; set; }
    }
}
