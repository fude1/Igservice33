using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.Algorithm
{
    internal class VolumeStrategy
    {
        public bool GetVolumeStrategy(List<double> closePrices, List<int> volumes, out double avgVolume, out double currentVolume)
        {
            bool Strategy = false;
            // Calculate the average volume
            int sumVolume = 0;
            for (int i = 0; i < volumes.Count; i++)
            {
                sumVolume += volumes[i];
            }
            double _avgVolume = (double)sumVolume / volumes.Count;

            // Check if the current volume is above the average volume
            int _currentVolume = volumes[volumes.Count - 1];

            if (_currentVolume > (_avgVolume*2))
                Strategy = true;
            currentVolume = _currentVolume;

            avgVolume = _avgVolume;

            return Strategy;
        }
    }
}
