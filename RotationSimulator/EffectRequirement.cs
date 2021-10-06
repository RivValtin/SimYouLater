using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// Describes a required active effect and stack quantity.
    /// </summary>
    public class EffectRequirement
    {
        public EffectDef effect;
        /// <summary>
        /// Number of stacks required. When requiring a buff that does not possess stacks, use a value of 1 (the default).
        /// </summary>
        public int Stacks = 1;
    }
}
