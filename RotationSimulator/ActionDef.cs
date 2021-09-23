using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// For defining the various properties of a specific ability usable by some job or another.
    /// 
    /// Instances of this class should effectively be immutable once created. 
    /// </summary>
    public class ActionDef
    {
        /// <summary>
        /// A unique identifier used e.g. for export/import of rotations
        /// </summary>
        public string UniqueID { get; init; }
        public bool IsGCD { get; init; } = true;

        /// <summary>
        /// If true, should be scaled by Skill Speed.
        /// </summary>
        public bool IsWeaponskill { get; init; } = false;
        /// <summary>
        /// If true, should be scaled by Spell Speed.
        /// </summary>
        public bool IsSpell { get; init; } = false;
        /// <summary>
        /// If true, should be scaled with haste.
        /// </summary>
        public bool IsHastedScaled { get; init; } = true;
        /// <summary>
        /// The amount of time the player must spend casting before the ability is considered executed.
        /// </summary>
        public int CastTime { get; init; } = 0;
        /// <summary>
        /// Only valid if IsGCD is true. Represents the GCD cycling time of that GCD. Simulator will track weaving properly if e.g. off-GCDs are used right after a GCD.
        /// </summary>
        public int RecastGCD { get; init; } = 250;
        /// <summary>
        /// Use if the action has an unusually low animation lock, like dancer steps or ninja mudras.
        /// </summary>
        public int AnimationLockOverride { get; init; } = 0;
        /// <summary>
        /// 0 indicates no non-global cooldown. Will only be used to throw an error if an ability is used before recast is up, but that's NYI. 
        /// Note that an ability can both invoke the global cooldown *and* have its own cooldown (for example, Machinist's Drill)
        /// </summary>
        public int Recast { get; init; } = 0;
        /// <summary>
        /// The damage dealt. Can be set to 0 just fine. Do not set a negative value for healing, it might get interpreted as healing the boss.
        /// </summary>
        public int Potency { get; init; } = 0;
        /// <summary>
        /// The user-visible name of the ability.
        /// </summary>
        public string DisplayName { get; init; } = "NAME MISSING";
        /// <summary>
        /// The name of the icon that this ability uses.
        /// </summary>
        public string IconName { get; init; } = "icon_missing.png";

        /// <summary>
        /// Apply the listed effects when executing this ability.
        /// </summary>
        public List<EffectApplication> AppliedEffects { get; init; } = new List<EffectApplication>();
        /// <summary>
        /// Remove (int) stacks from the active effect of the given type when executed. If stacks hit 0, effect is removed prematurely. <=0 stacks indicates to remove the entire effect regardless of stack count.
        /// </summary>
        public List<Tuple<EActiveEffect, int>> RemoveEffectStacks { get; init; } = new List<Tuple<EActiveEffect, int>>();
        /// <summary>
        /// NYI. Show an error if attempting to use the ability without any of these listed effects active. Stacks variable will be used to evaluate if enough stacks are available (e.g. set it to 3 to require 3 stacks available).
        /// </summary>
        public List<ActiveEffect> RequiredEffects { get; init; } = new List<ActiveEffect>();
        /// <summary>
        /// NYI. Show an error if attempting to use an ability with any of the listed effects active. 
        /// </summary>
        public List<ActiveEffect> RequiredAbsentEffects { get; init; } = new List<ActiveEffect>();
    }
}
