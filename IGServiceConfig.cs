using System.Collections.Generic;

namespace IGService3
{
    public class Ticker
    {
        public string ticker { get; set; }
        public int tradingEnable { get; set; }
        public int priceTrade { get; set; }
        public int Size { get; set; }
        public int Gain { get; set; }
        public int GainStopEnable { get; set; }
        public int Loss { get; set; }
        public int StopLossEnable { get; set; }
    }

    public class TickerLong
    {
        public List<Ticker> tickers { get; set; }
    }

    public class TickerShort
    {
        public List<Ticker> tickers { get; set; }
    }

    public class TickerMonitor
    {
        public string ticker { get; set; }
        public int monitorEnable { get; set; }
    }

    public class Config
    {
        public int tradingLong { get; set; }
        public int tradingShort { get; set; }
        public int closeTradingEndOfDay { get; set; }
        public string endOfDayTime { get; set; }
        public TickerLong tickerLong { get; set; }
        public TickerShort tickerShort { get; set; }
        public TickerMonitor tickerMonitor { get; set; }
    }

    public class Root
    {
        public Config Config { get; set; }
    }



}