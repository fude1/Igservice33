using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Skender.Stock.Indicators;
using TicTacTec.TA;

namespace IGServiceDevelop.Algorithm
{


    internal class MACDAlgorithm
    {
        double _rsibuySpread=0;
        double _rsisellSpread=0;

        public void CalculateMACDRSI(List<double> prices, out double macdLine, out double signalLine,out double histogram, out double Rsi, out bool RsiSell, out bool RsiBuy, out double RsiSellSpread, out double RsiBuySpread)
        {

            int outBegIdx1, outNBElement1;
            double[] outReal = new double[prices.Count];
            RsiSell = false;
            RsiBuy = false;
            int outBegIdx2, outNBElement2;
            double[] outMACD = new double[prices.Count];
            double[] outMACDSignal = new double[prices.Count];
            double[] outMACDHist = new double[prices.Count];

            TicTacTec.TA.Library.Core.Rsi(0, prices.Count - 1, prices.ToArray(), 14, out outBegIdx1, out outNBElement1, outReal);
            TicTacTec.TA.Library.Core.Macd(0, prices.Count - 1, prices.ToArray(), 12, 26, 9, out outBegIdx2, out outNBElement2, outMACD, outMACDSignal, outMACDHist);
            macdLine = outMACD[outNBElement2-1];
            signalLine = outMACDSignal[outNBElement2 - 1];
            histogram = outMACDHist[outNBElement2 - 1];
            Rsi = outReal[outNBElement1-1];
            double LastRsi = outReal[outNBElement1 - 2];
            if (Rsi < 68)
            {
                if (LastRsi >= 68)
                    RsiSell = true;
            }
            if (Rsi > 68)
            {
                if ((Rsi-68) > _rsisellSpread)
                    _rsisellSpread = (Rsi-68);
            }
            else
                _rsisellSpread = 0;


            if (Rsi > 32)
            {
                 if (LastRsi <= 32)
                    RsiBuy = true;
            }

            if (Rsi < 32)
            {
                if ((32-Rsi) < _rsibuySpread)
                    _rsibuySpread = (32-Rsi);
            }
            else
                _rsibuySpread = 0;

            RsiSellSpread = _rsisellSpread;
            RsiBuySpread = _rsibuySpread;
        }

    }


}