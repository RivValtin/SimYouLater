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

        /// <summary>
        /// The crit rate at the time of effect application.
        /// </summary>
        public int SnapshotCritRate = -1;
        /// <summary>
        /// The crit bonus at the time of effect application.
        /// NOTE: At this time no abilities modify multiplier, that I'm aware, but this is here anyway.
        /// </summary>
        public int SnapshotCritBonus = -1;
        /// <summary>
        /// The Direct Hit rate at the time of effect application.
        /// </summary>
        public int SnapshotDHRate = -1;
        /// <summary>
        /// The skill/spell speed (whichever the ability cares about) at the time of application.
        /// NOTE: At this time no abilities would modify this, so "snapshotting" is meaningless, but oh well.
        /// </summary>
        public int SnapshotSpeed = -1;
        /// <summary>
        /// The total multiplier from all flat multiplier sources (buffs, det, etc).
        /// </summary>
        public float SnapshotMulti = 1.0f;
    }
}
