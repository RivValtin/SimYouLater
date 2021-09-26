using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    /// <summary>
    /// Placeholder stub
    /// </summary>
    public class AutoAttackTimer : ITimedElement
    {
        public void AdvanceTime(int time) {
        }

        public int NextEvent() {
            return int.MaxValue;
        }
    }
}
