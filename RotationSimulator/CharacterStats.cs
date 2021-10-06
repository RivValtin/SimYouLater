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
        public int CritRate { get; set; }
        public int CritBonus { get; set; }
        public int DirectHitRate { get; set; }
        public int DetBonus { get; set; }
    }
}
