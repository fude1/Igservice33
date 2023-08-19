using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGServiceDevelop.StateMachine
{
    public enum stateBolingerOscillator
    {
        BOLINGERB_IN_RANGE = 0,
        BOLINGERB_OUT_RANGE_LOWER = 1,
        BOLINGERB_OUT_RANGE_UPPER = 2
    }
    internal class StateMachineBolingerOscillator
    {
        public int actualStatus { get; set; }
    }
}
