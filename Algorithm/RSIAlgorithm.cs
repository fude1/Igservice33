using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.Algorithm
{

    internal class RSIAlgorithm
    {

       public List<double> RSISMACross(List<double> closePrices, int rsiPeriod, out List<bool> buySignals, out List<bool> sellSignals )
       {
            int smaShortPeriod = 10;
            int smaLongPeriod = 20;
            double rsiBuyThreshold = 30;
            double rsiSellThreshold = 70;

            List<bool> _sellSignals = new List<bool>();

            List<double> gains = new List<double>();
            List<double> losses = new List<double>();
            List<double> smaShort = new List<double>();
            List<double> smaLong = new List<double>();
            List<double> rsiValues = new List<double>();
            List<bool> _buySignals = new List<bool>();

            double lastClose = closePrices[0];
            List<double> rsValues = new List<double>();

            for (int i = 1; i<closePrices.Count; i++)
            {
                double close = closePrices[i];

                if (close > lastClose)
                {
                    gains.Add(close - lastClose);
                    losses.Add(0);
                }
                else if (close<lastClose)
                {
                    gains.Add(0);
                    losses.Add(lastClose - close);
                }
                else
                {
                    gains.Add(0);
                    losses.Add(0);
                }

                lastClose = close;

                if (i >= smaShortPeriod)
                {
                    double smaShortValue = closePrices.Skip(i - smaShortPeriod + 1).Take(smaShortPeriod).Average();
                    smaShort.Add(smaShortValue);
                }

                if (i >= smaLongPeriod)
                {
                    double smaLongValue = closePrices.Skip(i - smaLongPeriod + 1).Take(smaLongPeriod).Average();
                    smaLong.Add(smaLongValue);
                }

                if (i >= rsiPeriod)
                {
                    double avgGain = gains.Skip(i - rsiPeriod).Take(rsiPeriod).Average();
                    double avgLoss = losses.Skip(i - rsiPeriod).Take(rsiPeriod).Average();

                    double rs = avgGain / avgLoss;
                    double rsi = 100 - (100 / (1 + rs));

                    rsiValues.Add(rsi);
                }               
            }

            for (int i = 0; i < rsiValues.Count; i++)
            {
                double rsiValue = rsiValues[i];

                if (rsiValue < rsiBuyThreshold && smaShort[i] > smaLong[i])
                {
                    _buySignals.Add(true);
                }
                else
                {
                    _buySignals.Add(false);
                }

                if (rsiValue > rsiSellThreshold && smaShort[i] < smaLong[i])
                {
                    _sellSignals.Add(true);
                }
                else
                {
                    _sellSignals.Add(false);
                }
            }
            buySignals = _buySignals;
            sellSignals = _sellSignals;

            return rsValues;
        }

        public List<double> GetRSI(List<double> closePrices, int period)
        {
         
            List<double> gains = new List<double>();
            List<double> losses = new List<double>();

            double lastClose = closePrices[0];

            for (int i = 1; i < closePrices.Count; i++)
            {
                double close = closePrices[i];

                if (close > lastClose)
                {
                    gains.Add(close - lastClose);
                    losses.Add(0);
                }
                else if (close < lastClose)
                {
                    gains.Add(0);
                    losses.Add(lastClose - close);
                }
                else
                {
                    gains.Add(0);
                    losses.Add(0);
                }

                lastClose = close;
            }

            List<double> avgGains = new List<double>();
            List<double> avgLosses = new List<double>();

            double firstAvgGain = gains.Take(period).Average();
            double firstAvgLoss = losses.Take(period).Average();

            avgGains.Add(firstAvgGain);
            avgLosses.Add(firstAvgLoss);

            for (int i = period; i < gains.Count; i++)
            {
                double currentGain = gains[i];
                double currentLoss = losses[i];

                double prevAvgGain = avgGains[i - period];
                double prevAvgLoss = avgLosses[i - period];

                double avgGain = ((prevAvgGain * (period - 1)) + currentGain) / period;
                double avgLoss = ((prevAvgLoss * (period - 1)) + currentLoss) / period;

                avgGains.Add(avgGain);
                avgLosses.Add(avgLoss);
            }

            List<double> rsValues = new List<double>();

            for (int i = period; i < closePrices.Count; i++)
            {
                double avgGain = avgGains[i - period];
                double avgLoss = avgLosses[i - period];

                double rs = avgGain / avgLoss;
                double rsi = 100 - (100 / (1 + rs));

                rsValues.Add(rsi);
            }
            return rsValues;
       }
    }
}
