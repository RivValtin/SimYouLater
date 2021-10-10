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
        public int CritRate { get; init; }
        public int CritBonus { get; init; }
        public int DirectHitRate { get; init; }
        public int DetBonus { get; init; }
        public int SkillSpeed { get; init; }
        public int SpellSpeed { get; init; }
    }
}
