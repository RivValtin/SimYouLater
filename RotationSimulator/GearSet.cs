using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// Despite its name, this class actually just tracks the resulting stats, not the individual gear pieces/melds.
    /// 
    /// </summary>
    //At some point prob gonna have integration with some other gear tool, because implementing a full gear management system is a lot.
    //Then again, maybe I'll get bored. Who knows.
    public class GearSet
    {
        /// <summary>
        /// Arbitrary gear set name chosen by the user.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The job this gear set is intended for. UI should show an error/warning if rotation/gearset chosen are mismatched.
        /// </summary>
        public string JobCode { get; set; }
        /// <summary>
        /// The level this gear set is intended for. UI should show an error/warning if rotation/gearset chosen are mismatched on this.
        /// </summary>
        public int Level { get; set; } = 80;
        /// <summary>
        /// Physical damage stat of the equipped weapon. TODO: Set to a default value representing the hidden value used by magic classes to handle auto-attacks.
        /// </summary>
        public int PhysicalDamage { get; set; } = 1; //this value is BS, just haven't looked up what the real hidden value is on magical weapons.
        /// <summary>
        /// Magic damage stat of the equipped weapon.
        /// </summary>
        public int MagicalDamage { get; set; } = 1;
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Intelligence { get; set; }
        public int Mind { get; set; }
        public int Vitality { get; set; }

        public int CriticalHit { get; set; }
        public int DirectHit { get; set; }
        public int Determination { get; set; }
        public int SkillSpeed { get; set; }
        public int SpellSpeed { get; set; }
        public int Tenacity { get; set; }
        public int Piety { get; set; }
    }
}
