using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public enum EEffectType
    {
        /// <summary>
        /// A standard buff on the player.
        /// </summary>
        Buff,
        /// <summary>
        /// A debuff placed on an enemy target.
        /// </summary>
        Debuff,
        /// <summary>
        /// Used to track a resource, rather than being a true buff. Separate from buff/hidden so that it can warn about overcap.
        /// If you don't really overcap it, use a different category.
        /// </summary>
        Resource,
        /// <summary>
        /// A buff for tracking combo state.
        /// </summary>
        Combo,
        /// <summary>
        /// A hidden state-tracking buff, meant for things not directly visible to the player like the short window after a summon GCD where EW SMN cannot use devotion.
        /// </summary>
        Hidden,
        /// <summary>
        /// A ground effect.
        /// </summary>
        Ground
    }
    /// <summary>
    /// Contains the readonly information about a single active effect, such as a buff, debuff, or even special job-specific states like "is phoenix the next summoned demi?"
    /// 
    /// May eventually contain further information such as tooltip for UI purposes.
    /// </summary>
    public class EffectDef
    {
        /// <summary>
        /// ID unique to this particular effect.
        /// </summary>
        public string UniqueID { get; init; }
        /// <summary>
        /// A name to display to the user.
        /// </summary>
        public string DisplayName { get; init; } = "EFFECT NAME MISSING";
        /// <summary>
        /// Relevant buff/debuff icon (if any).
        /// </summary>
        public string IconName { get; init; } = "icon_missing.png";
        /// <summary>
        /// If true, this effect comes in stacks and should show their quantity (even if it's 1).
        /// </summary>
        public bool UsesStacks { get; init; } = false;
        /// <summary>
        /// If greater than 0, this effect should not expire all at once and instead remove one stack then reset remaining duration to this value.
        /// </summary>
        public int StackDecayDuration { get; init; } = 0;
        public EEffectType Type { get; init; } = EEffectType.Buff;
        /// <summary>
        /// For DoTs only. Specifies the potency per tick. Default value of 0 indicates that this effect is not a dot.
        /// </summary>
        public int Potency = 0;
        /// <summary>
        /// If a dot, this determines whether it should snapshot. Generally speaking, ground effects do not snapshot, but debuffs do.
        /// </summary>
        public bool Snapshots = true;
    }
}
