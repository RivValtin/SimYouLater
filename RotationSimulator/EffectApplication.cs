using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{

    /// <summary>
    /// Describes applying an active effect, and the rules for how it interacts with any existing application of that effect.
    /// </summary>
    public class EffectApplication
    {
        public EffectDef effect;
        /// <summary>
        /// How long to apply the effect for. Use int.MaxValue to indicate no duration.
        /// </summary>
        public int Duration = int.MaxValue;
        /// <summary>
        /// When IsAdditiveDuration is true, this serves as the true cap on duration for the effect. If set to 0, there is no max (more likely, the max is irrelevant due to relative duration and cooldown)
        /// </summary>
        public int DurationMax = 0;
        /// <summary>
        /// Number of stacks applied. Abilities without stacks should still use a value of 1 (the default).
        /// </summary>
        public int Stacks = 1;
        /// <summary>
        /// When IsAdditiveStacks is true, this serves as the true cap on stacks for the effect. If set to 0, there is no max (more likely, it simply isn't relevant)
        /// </summary>
        public int StackMax = 0;
        /// <summary>
        /// If true, stacks is added to current stacks. If false, stacks are overridden when any new stacks are acquired.
        /// </summary>
        public bool IsAdditiveStacks = false;
        /// <summary>
        /// If true, duration is added to current duration. If false, duration overwrites current duration.
        /// </summary>
        public bool IsAdditiveDuration = false;
        /// <summary>
        /// The chance that the effect actually gets applied. Use for procs. 100 is 100% chance.
        /// </summary>
        public int ProcChance = 100;
    }
}
