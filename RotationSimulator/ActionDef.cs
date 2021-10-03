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
        /// If true, should be scaled by Skill Speed. NYI
        /// </summary>
        public bool IsWeaponskill { get; init; } = false;
        /// <summary>
        /// If true, should be scaled by Spell Speed. NYI
        /// </summary>
        public bool IsSpell { get; init; } = false;
        /// <summary>
        /// If true, should be scaled with haste. NYI
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
        /// If Recast > 0, this is the number of charges the ability has. 
        /// Should be left to its default value (1) for any non-charged ability. Setting it to another value may cause errors.
        /// </summary>
        public int Charges { get; init; } = 1;
        public int MPCost { get; init; } = 0;
        /// <summary>
        /// If true, the non-GCD recast also scales with SpS/SkS/Haste
        /// </summary>
        public bool RecastScales { get; init; } = false;
        /// <summary>
        /// The damage dealt. Can be set to 0 just fine. Do not set a negative value for healing, it might get interpreted as healing the boss.
        /// </summary>
        public int Potency { get; init; } = 0;
        /// <summary>
        /// The damage dealt if the ability is used outside of its intended combo. 
        /// </summary>
        public int UncomboedPotency { get; init; } = 0;
        /// <summary>
        /// If a non-null, non-empty string, defines the UniqueId of the EffectDef that this ability needs active to be considered "combo'd". 
        /// If this effect is not active, it will deal the UncomboedPotency damage instead of its normal Potency.
        /// In addition, if this effect is not active, this ability will not apply any of its AppliedEffects.
        /// </summary>
        public string ComboEffectId { get; init; } = "";
        /// <summary>
        /// The user-visible name of the ability.
        /// </summary>
        public string DisplayName { get; init; } = "NAME MISSING";
        /// <summary>
        /// The name of the icon that this ability uses.
        /// </summary>
        public string IconName { get; init; } = "icon_missing.png";
        /// <summary>
        /// If multiple abilities share a cooldown, choose one of them to be the main one and set this value equal
        /// to that cooldown's UniqueID. For example, Drill and Bioblaster share a CD, so you can set Bioblaster's 
        /// "CooldownID" to Drill's UniqueID so that they share.
        /// 
        /// This value causes recast to be ignored in favor of the recast of the other action. May change later,
        /// but for now all shared cooldowns also have the same recast.
        /// </summary>
        public string CooldownID { get {
                if (String.IsNullOrEmpty(cooldownId)) {
                    return UniqueID;
                } else {
                    return cooldownId;
                }
            }
            init {
                cooldownId = value;
            }
        }
        private string cooldownId = String.Empty;

        /// <summary>
        /// If non-null, indicates that this move upgrades to another move identified by (string) at character level (int). For example, Split Shot upgrades to Heated Split Shot at 54.
        /// </summary>
        public Tuple<int, string> LevelBasedUpgrade = null;

        /// <summary>
        /// For each tuple, reduce the cooldown of the CooldownID (string) by an amount equal to (int) centiseconds when this ability is activated.
        /// </summary>
        public IEnumerable<Tuple<string, int>> CooldownReset { get; init; } = new List<Tuple<string, int>>();

        /// <summary>
        /// Apply the listed effects when executing this ability.
        /// </summary>
        public IEnumerable<EffectApplication> AppliedEffects { get; init; } = new List<EffectApplication>();
        /// <summary>
        /// Remove (int) stacks from the active effect with UniqueID of (string) when executed. If stacks hit 0, effect is removed prematurely. <=0 stacks indicates to remove the entire effect regardless of stack count.
        /// </summary>
        public IEnumerable<Tuple<string, int>> RemoveEffectStacks { get; init; } = new List<Tuple<string, int>>();
        /// <summary>
        /// Show an error if attempting to use the ability without any of these listed effects active. Stacks variable will be used to evaluate if enough stacks are available (e.g. set it to 3 to require 3 stacks available).
        /// </summary>
        public IEnumerable<EffectRequirement> RequiredEffects { get; init; } = new List<EffectRequirement>();
        /// <summary>
        /// Show an error if attempting to use an ability with any of the listed effects active. 
        /// </summary>
        public IEnumerable<EffectRequirement> RequiredAbsentEffects { get; init; } = new List<EffectRequirement>();

    }
}
