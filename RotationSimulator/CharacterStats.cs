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
        private CharacterStats() { }
        public CharacterStats(EJobId job) {
            Job = job;
            switch (job) {
                case EJobId.WHM:
                case EJobId.AST:
                case EJobId.SCH:
                //case EJobId.SGE: TODO EW Patch Stuff
                case EJobId.SMN:
                case EJobId.BLM:
                case EJobId.RDM:
                case EJobId.BLU:
                    PhysicalClass = false;
                    break;
                default:
                    PhysicalClass = true;
                    break;
            }
        }
        public EJobId Job { get; private set; }

        public int RelevantAttackPower { get; private set; }
        public int AutoAttackPower { get; private set; }

        public bool IsTank { get {
                switch (Job) {
                    case EJobId.PLD:
                    case EJobId.WAR:
                    case EJobId.DRK:
                    case EJobId.GNB:
                        return true;
                    default:
                        return false;
                }
            } 
        }

        public int Strength { get {
                return strength;
            } 
            set {
                strength = value;
                switch (Job) {
                    case EJobId.DRG:
                    case EJobId.MNK:
                    case EJobId.SAM:
                    case EJobId.PLD:
                    case EJobId.DRK:
                    case EJobId.WAR:
                    case EJobId.GNB:
                        //TODO EW Patch Stuff
                        RelevantAttackPower = value;
                        AutoAttackPower = value;
                        break;
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                        //TODO EW Patch Stuff
                        AutoAttackPower = value;
                        break;
                    default: 
                        break;
                }
            }
        }
        private int strength;
        public int Dexterity {
            get {
                return dexterity;
            }
            set {
                dexterity = value;
                switch (Job) {
                    case EJobId.NIN:
                    case EJobId.BRD:
                    case EJobId.DNC:
                    case EJobId.MCH:
                        RelevantAttackPower = value;
                        AutoAttackPower = value;
                        break;
                    default:
                        break;
                }
            }
        }
        private int dexterity;
        public int Intelligence {
            get {
                return intelligence;
            }
            set {
                intelligence = value;
                switch (Job) {
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                        RelevantAttackPower = value;
                        break;
                    default:
                        break;
                }
            }
        }
        private int intelligence;
        public int Mind {
            get {
                return mind;
            }
            set {
                mind = value;
                switch (Job) {
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                        //TODO EW Patch Stuff
                        RelevantAttackPower = value;
                        break;
                    default:
                        break;
                }
            }
        }
        private int mind;

        public int WeaponDamage { get; private set; }
        public int AutoWeaponDamage { get; private set; }

        public int PhysicalDamage { get {
                return physDamage;
            } 
            init {
                physDamage = value;
                switch (Job) {
                    case EJobId.DRG:
                    case EJobId.MNK:
                    case EJobId.SAM:
                    case EJobId.PLD:
                    case EJobId.DRK:
                    case EJobId.WAR:
                    case EJobId.GNB:
                        //TODO EW Patch Stuff
                        WeaponDamage = value;
                        AutoWeaponDamage = value;
                        break;
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                        //TODO EW Patch Stuff
                        AutoWeaponDamage = value;
                        break;
                    default:
                        break;
                }
            }
        }
        private int physDamage;
        public int MagicalDamage {
            get {
                return magDamage;
            }
            init {
                magDamage = value;
                switch (Job) {
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                        //TODO EW Patch Stuff
                        WeaponDamage = value;
                        break;
                    default:
                        break;
                }
            }
        }
        private int magDamage;

        public int CritRate { get; private set; }
        public int CritBonus { get; private set; }
        public int DirectHitRate { get; private set; }
        public int DetBonus { get; private set; }
        public int TenBonus { get; private set; }
        /// <summary>
        /// If true, this class is a physical class and typically uses SkS and Attack Power for scaling.
        /// </summary>
        public bool PhysicalClass { get; private set; } = true;

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

        public int Tenacity {
            get {
                return tenacitySub;
            } 
            init {
                tenacitySub = value;
                TenBonus = StatMath.GetTenBonus(tenacitySub);
            }
        }
        private int tenacitySub;
        public int SkillSpeed { get; init; }
        public int SpellSpeed { get; init; }
        /// <summary>
        /// Returns skill speed for physical classes, and spell speed for magical ones.
        /// </summary>
        public int RelevantSpeed { get { return PhysicalClass ? SkillSpeed : SpellSpeed; } }
    }
}
