using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TicTacTec.TA;

namespace IGServiceDevelop.Algorithm
{
    internal class BollingerAlgorithm
    {
        double _BBSellSpread = 0;
        double _BBBuySpread = 0;
        public void CalculateBollingerBands(List<double> prices, int period, double standardDeviations, out double upperBand, out double middleBand, out double lowerBand)
        {
             upperBand = 0;
            middleBand = 0;
            lowerBand = 0;
            double [] _upperBand = new double[prices.Count()+5];
            double[] _middleBand = new double[prices.Count()+5];
            double[] _lowerBand = new double[prices.Count()+5];


            int timePeriod = prices.Count();
            double standardDeviationsUp = 2.0;
            double standardDeviationsDown = 2.0;
            int outBegIdx, outNbElement;
            TicTacTec.TA.Library.Core.MAType optInMAType = TicTacTec.TA.Library.Core.MAType.Sma;
            TicTacTec.TA.Library.Core.Bbands(0, prices.Count()-1, prices.ToArray(), timePeriod, standardDeviationsUp, standardDeviationsDown, optInMAType, out outBegIdx, out outNbElement, _upperBand, _middleBand, _lowerBand);
            upperBand = _upperBand[outNbElement-1];
            middleBand = _middleBand[outNbElement-1];
            lowerBand = _lowerBand[outNbElement-1];

        }

        public double[] GetBollingerB(List<double> prices, int period, double standardDeviations, out bool BBbuy, out bool BBsell, out double BBSellSpread, out double BBBuySpread)
        {
            double[] closePrices  = prices.ToArray();

            BBbuy = false;
            BBsell = false;

            int endIndex = closePrices.Length - 1;
            int startIndex = endIndex - period + 1;
            int outBegIdx, outNbElement;
            double[] upperBand = new double[closePrices.Length];
            double[] middleBand = new double[closePrices.Length];
            double[] lowerBand = new double[closePrices.Length];
            double[] percentB = new double[closePrices.Length];

            double standardDeviationsUp = standardDeviations;
            double standardDeviationsDown = standardDeviations;

            TicTacTec.TA.Library.Core.MAType optInMAType = TicTacTec.TA.Library.Core.MAType.Sma;
            TicTacTec.TA.Library.Core.Bbands(startIndex, endIndex, closePrices, period, standardDeviationsUp, standardDeviationsDown, optInMAType, out outBegIdx, out outNbElement, upperBand, middleBand, lowerBand);
            for (int i = 0; i < outNbElement; i++)
            {
                percentB[i] = (closePrices[i + startIndex] - lowerBand[i]) / (upperBand[i] - lowerBand[i]);
            }
            double _percentB = percentB[outNbElement - 1];
            double LastpercentB = percentB[outNbElement - 2];
            if (_percentB < 1 )
            {
                if (LastpercentB >= 1)
                    BBsell = true;
            }
            if (_percentB > 1)
            {
                if ((_percentB - 1) < _BBSellSpread)
                    _BBSellSpread = (_percentB - 1);
            }
            else
                _BBSellSpread = 0;

            if (_percentB >=  0)
            {
                if (LastpercentB < 0)
                BBbuy = true;
            }

            if (_percentB < 0)
            {
                if ((_percentB) < _BBBuySpread)
                    _BBBuySpread = _percentB;
            }
            else
                _BBBuySpread = 0;

            BBSellSpread = _BBSellSpread;
            BBBuySpread = _BBBuySpread;
            return percentB;
        }

    }

}
