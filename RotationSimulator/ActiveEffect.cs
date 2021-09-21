using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public enum EActiveEffect
    {
        SMN_Devotion,
        SMN_BahamutSummoned,
        SMN_PhoenixAvailable,
        SMN_IfritGem,
        SMN_GarudaGem,
        SMN_TitanGem,
        SMN_Ruination,
        NIN_TrickAttack,
    }

    public class EffectApplication
    {
        public EActiveEffect type;
        /// <summary>
        /// How long to apply the effect for. Use int.MaxValue to indicate no duration.
        /// </summary>
        public int Duration;
        /// <summary>
        /// When IsAdditiveDuration is true, this serves as the true cap on duration for the effect. If set to 0, there is no max (more likely, the max is irrelevant due to relative duration and cooldown)
        /// </summary>
        public int DurationMax = 0;
        /// <summary>
        /// Number of stacks applied. Not used by abilities without stacks. 
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
        /// A name to display to the user.
        /// </summary>
        public string DisplayName = "EFFECT NAME MISSING";
        /// <summary>
        /// Relevant buff/debuff icon (if any).
        /// </summary>
        public string IconName;
    }

    public class ActiveEffect
    {
        public EActiveEffect type;
        public int DurationRemaining { get { return ActiveEndTime - ActiveStartTime; } }
        public int ActiveStartTime;
        public int ActiveEndTime;
        public int Stacks;
        public string DisplayName = "EFFECT NAME MISSING";
    }
}
