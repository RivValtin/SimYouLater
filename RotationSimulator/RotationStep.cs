using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// All times are in centiseconds as integers, since the game does not track sub-centisecond differences anyway.
    /// 
    /// This class should not contain any state that would get modified during a rotation.
    /// </summary>
    public class RotationStep
    {
        public bool IsGCD = true;

        public bool IsWeaponskill = false; //affected by SkS
        public bool IsSpell = false; //affected by SpS

        public int lastExecutedTime = int.MinValue; //used by the simulator, represents the last time this ability was used, for putting up error messages
        public int castTime = 0;
        /// <summary>
        /// Only valid if IsGCD is true. Represents the GCD cycling time of that GCD. Simulator will track weaving properly if e.g. off-GCDs are used right after a GCD.
        /// </summary>
        public int recastGCD = 250;
        /// <summary>
        /// Use if the action has an unusually low animation lock, like dancer steps or ninja mudras.
        /// </summary>
        public int animationLockOverride = 0;
        /// <summary>
        /// 0 indicates no non-global cooldown. Will only be used to throw an error if an ability is used before recast is up, but that's NYI
        /// </summary>
        public int recast = 0;
        /// <summary>
        /// The damage dealt. Can be set to 0 just fine. Do not set a negative value for healing, it might get interpreted as healing the boss.
        /// </summary>
        public int potency = 0;
        public string DisplayName = "NAME MISSING";
        public string IconName = "icon_missing.png";

        /// <summary>
        /// Apply the listed effects when executing this ability.
        /// </summary>
        public List<EffectApplication> appliedEffects = new List<EffectApplication>();
        /// <summary>
        /// Remove (int) stacks from the active effect of the given type when executed. If stacks hit 0, effect is removed prematurely. <=0 stacks indicates to remove the entire effect regardless of stack count.
        /// </summary>
        public List<Tuple<EActiveEffect, int>> removeEffectStacks = new List<Tuple<EActiveEffect, int>>();
        /// <summary>
        /// NYI. Show an error if attempting to use the ability without any of these listed effects active. Stacks variable will be used to evaluate if enough stacks are available (e.g. set it to 3 to require 3 stacks available).
        /// </summary>
        public List<ActiveEffect> requiredEffects = new List<ActiveEffect>();
        /// <summary>
        /// NYI. Show an error if attempting to use an ability with any of the listed effects active. 
        /// </summary>
        public List<ActiveEffect> requiredAbsentEffects = new List<ActiveEffect>();
    }
}
