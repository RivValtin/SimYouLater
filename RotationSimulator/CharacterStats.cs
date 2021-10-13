using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// Holds the relevant stats of the character (such as crit chance)
    /// </summary>
    public class CharacterStats
    {
        public int CritRate { get; private set; }
        public int CritBonus { get; private set; }
        public int DirectHitRate { get; private set; }
        public int DetBonus { get; private set; }
        /// <summary>
        /// If true, this class is a physical class and typically uses SkS and Attack Power for scaling.
        /// </summary>
        public bool PhysicalClass { get; set; } = true;

        public int CriticalHitSubstat { 
            get {
                return criticalSub;
            } 
            init {
                criticalSub = value;
                CritRate = StatMath.GetCritRate(criticalSub);
                CritBonus = StatMath.GetCritBonus(criticalSub);
            } 
        }
        private int criticalSub;
        public int DirectHitSubstat {
            get {
                return directHitSub;
            }
            init {
                directHitSub = value;
                DirectHitRate = StatMath.GetDHRate(directHitSub);
            }
        }
        private int directHitSub;
        public int DeterminationSubstat {
            get {
                return determinationSub;
            }
            init {
                determinationSub = value;
                DetBonus = StatMath.GetDetBonus(determinationSub);
            }
        }
        private int determinationSub;
        public int SkillSpeed { get; init; }
        public int SpellSpeed { get; init; }
        /// <summary>
        /// Returns skill speed for physical classes, and spell speed for magical ones.
        /// </summary>
        public int RelevantSpeed { get { return PhysicalClass ? SkillSpeed : SpellSpeed; } }
    }
}
