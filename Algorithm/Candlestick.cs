using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.Algorithm
{
    public class Candlestick
    {
        // Hammer pattern is defined as having a small body (less than 10% of the total length of the candlestick),
        //a long lower shadow (at least twice the length of the body), and a short upper shadow (less than the length of the body).
        public bool IsHammer(double open, double high, double low, double close)
        {
            double body = Math.Abs(close - open);
            double upperShadow = high - Math.Max(open, close);
            double lowerShadow = Math.Min(open, close) - low;
            double totalLength = upperShadow + lowerShadow + body;

            // Check if the candlestick has a small body and a long lower shadow
            if (body < totalLength * 0.1 && lowerShadow > body * 2 && upperShadow < body)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Doji pattern is defined as having a small body(less than 10% of the total length of the candlestick)
        // and no or very small upper and lower shadows(also less than 10% of the total length of the candlestick).
        public bool IsDoji(double open, double high, double low, double close)
        {
            double body = Math.Abs(close - open);
            double upperShadow = high - Math.Max(open, close);
            double lowerShadow = Math.Min(open, close) - low;
            double totalLength = upperShadow + lowerShadow + body;

            // Check if the candlestick has a small body and no or very small shadows
            if (body < totalLength * 0.1 && upperShadow < body * 0.1 && lowerShadow < body * 0.1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        // Shooting Star pattern is defined as having a small body(less than 10% of the total length of the candlestick),
        // a long upper shadow(at least twice the length of the body), and a short lower shadow(less than the length of the body).
        public bool IsShootingStar(double open, double high, double low, double close)
        {
            double body = Math.Abs(close - open);
            double upperShadow = high - Math.Max(open, close);
            double lowerShadow = Math.Min(open, close) - low;
            double totalLength = upperShadow + lowerShadow + body;

            // Check if the candlestick has a small body and a long upper shadow
            if (body < totalLength * 0.1 && upperShadow > body * 2 && lowerShadow < body)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //bullish Engulfing pattern is defined as the current day's candlestick opening below the previous day's close and closing above
        //the previous day's open, completely engulfing the previous day's candlestick.
        public bool IsBullishEngulfing(double open, double high, double low, double close, double prevOpen, double prevClose)
        {
            // Check if the current candlestick opens below the previous day's close
            // and closes above the previous day's open, completely engulfing it.
            if (open < prevClose && close > prevOpen && close > open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // bearish Engulfing pattern is defined as the current day's candlestick opening above the previous day's close and closing below the previous day's open,
        // completely engulfing the previous day's candlestick     

        public bool IsBearishEngulfing(double open, double high, double low, double close, double prevOpen, double prevClose)
        {
            // Check if the current candlestick opens above the previous day's close
            // and closes below the previous day's open, completely engulfing it.
            if (open > prevClose && close < prevOpen && close < open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //Big Candlestick pattern is defined as a candlestick with a range (high minus low) greater than the
        //threshold value and a body (absolute difference between close and open) that is at least half of the range.
        public bool IsBigCandle(double open, double high, double low, double close, double threshold)
        {
            double range = high - low;
            double body = Math.Abs(close - open);

            // Check if the candlestick's range is greater than the threshold and its body is at least half of the range
            if (range > threshold && body > range / 2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
