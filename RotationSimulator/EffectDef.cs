using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
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

    }
}
