using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.Algorithm
{
    internal class SlopeCalculator
    {
        public double GetSlope(List<double> stockPrices)
        {
            double sum = 0;
            foreach (var price in stockPrices)
            {
                sum += price;
            }
            double average = sum / stockPrices.Count;

            // Calculate the slope
            double slope = 0;
            for (int i = 0; i < stockPrices.Count; i++)
            {
                slope += (i + 1) * (stockPrices[i] - average);
            }
            slope /= stockPrices.Count * (stockPrices.Count + 1) / 2;
            return slope;
        }
    }
    
}
