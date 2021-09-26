using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{

    /// <summary>
    /// A currently-active effect, when it started, when it will end, and how many stacks it currently has.
    /// </summary>
    public class ActiveEffect
    {
        public EffectDef effect;
        public int ActiveStartTime;
        public int ActiveEndTime;
        public int Stacks = 1;
    }
}
