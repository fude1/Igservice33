using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mtapi.mt5;
using Newtonsoft.Json;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace IGServiceDevelop.MT5
{

    internal class ApiMT5
    {
        MT5API api = null;

        public void Connect()
        {
            try
            {
                api = new MT5API(5012671982, "eswwyxj1", "access.metatrader5.com", 443);
                api.Connect();
            }
            catch
            {

            }
        }

        public List<double> getStockPricebyMonth(string symbol,int month,int timeframe)
        {
            List<double> historicData = new List<double>();
            try
            {
                for (int i = month; i > 0; i--)
                {
                    DateTime date = DateTime.Now;
                    if (month>1)
                        date = DateTime.Now.AddMonths(-(i-1));
                    List<Bar> downloadStock = api.DownloadQuoteHistoryMonth(symbol, date.Year, date.Month, date.Day, timeframe);
                    foreach (Bar _stockPrice in downloadStock)
                    {
                        historicData.Add(_stockPrice.ClosePrice);
                    }

                }
            }
            catch
            {

            }
            return historicData;
        }
        
        public List<Bar> getCandlePriceHTOT(string symbol)
        {
            List<Bar> historicData = new List<Bar>();
            DateTime date = DateTime.Now;
            try
            {

                List<Bar> Data = api.DownloadQuoteHistoryMonth(symbol, 2023, 2, 28, 60);
                foreach (Bar _stockPrice in Data)
                {
                    historicData.Add(_stockPrice);
                }

                Data = api.DownloadQuoteHistoryMonth(symbol, 2023, 4, 10, 60);
                foreach (Bar _stockPrice in Data)
                {
                    historicData.Add(_stockPrice);
                }

                Data = api.DownloadQuoteHistoryMonth(symbol, date.Year, date.Month, date.Day, 60);
                foreach (Bar _stockPrice in Data)
                {
                    historicData.Add(_stockPrice);
                }
            }
            catch
            {

            }
            return historicData;
        }

        public List<Bar> getCandlePrice(string symbol, int month ,int timeframe)
        {
             List<Bar> historicData = new List<Bar>();
            try
            {
                for (int i = month; i > 0; i--)
                {
                    DateTime date = DateTime.Now;
                    if (month > 1)
                        date = DateTime.Now.AddMonths(-(i - 1));
                    historicData = api.DownloadQuoteHistoryMonth(symbol, date.Year, date.Month, date.Day, timeframe);
                }
            }
            catch
            {

            }
            return historicData;
        }

        public Quote getStockPrice(string symbol)
        {
            Quote downloadStock = new Quote();
            try
            {
                while (api.GetQuote(symbol) == null)
                Thread.Sleep(1);

                downloadStock = api.GetQuote(symbol);
            }
            catch
            {

            }

            return downloadStock;
        }

        public void Disconnect(string symbol)
        {
            api.Disconnect();

        }
    }
}
