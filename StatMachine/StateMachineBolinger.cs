using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.StateMachine
{
    public enum StateBolinger
    {
        START_SEQ = 0,
        CANDLE_OUT_UPPER = 1,
        CANDLE_OUT_MIDDLE = 2,
        CANDLE_OUT_LOWER = 3
    }

    internal class StateMachineBolinger
    {
        public bool bSell { get; set; }
        public bool bBuy { get; set; }
        public bool bMidle { get; set; }
        public bool bMidleH { get; set; }
        public bool bMidleL { get; set; }
        public int actualStatus { get; set; }
    }
}
