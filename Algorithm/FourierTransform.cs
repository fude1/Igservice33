using Accord.Math;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TicTacTec.TA.Library;

namespace IGServiceDevelop.Algorithm
{
    internal class FourierTransformAlgorithm
    {
        public void CalculateFourierTransform (double[] prices, Complex[] fft)
        {
            // Calculate the FFT using the MathNet.Numerics library
            fft = new Complex[prices.Length];
            FourierTransform.FFT(fft, FourierTransform.Direction.Forward);
        }

    }
}
