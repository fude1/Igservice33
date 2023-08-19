using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.Algorithm
{
    internal class PivotPoint
    {

        public  double[] CalculatePivotPoints(double high, double low, double close)
        {
            double pivot = (high + low + close) / 3;
            double resistance1 = (2 * pivot) - low;
            double support1 = (2 * pivot) - high;
            double resistance2 = pivot + (high - low);
            double support2 = pivot - (high - low);
            double resistance3 = high + 2 * (pivot - low);
            double support3 = low - 2 * (high - pivot);

            double[] pivotPoints = { pivot, resistance1, support1, resistance2, support2, resistance3, support3 };
            return pivotPoints;
        }
    }
}
