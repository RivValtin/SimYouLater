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
                    BaseDamageBonus = 300;
                    break;
                case EJobId.MCH:
                case EJobId.BRD:
                case EJobId.DNC:
                    BaseDamageBonus = 200;
                    PhysicalClass = true;
                    break;
                default:
                    PhysicalClass = true;
                    break;
            }
        }
        public EJobId Job { get; private set; }

        /// <summary>
        /// The base bonus to damage from class passives. In tenths of a percent.
        /// </summary>
        public int BaseDamageBonus { get; private set; }

        /// <summary>
        /// Basically equal to main damage stat, this returns either attack power or magic power depending on job.
        /// As of this comment, only SMN Physik mismatches this, and we aren't simming healing here so who cares.
        /// </summary>
        public int RelevantAttackPower { get; private set; }
        /// <summary>
        /// Returns the multiplier to damage received from the relevant attack power stat.
        /// </summary>
        public int RelevantAttackPowerMultiplier { get; private set; }
        /// <summary>
        /// Returns the attack power used for auto-attacks. For phys classes this is the same as RelevantAttackPower,
        /// but for magical classes it will instead be equal to strength.
        /// </summary>
        public int AutoAttackPower { get; private set; }
        /// <summary>
        /// Returns the multiplier to autoattack damage received from attack power.
        /// </summary>
        public int AutoAttackPowerMultiplier { get; private set; }

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
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        AutoAttackPower = value;
                        AutoAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
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
                        AutoAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
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
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        AutoAttackPower = value;
                        AutoAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
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
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
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
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        break;
                    default:
                        break;
                }
            }
        }
        private int mind;

        /// <summary>
        /// Returns the weapon damage of the type this class uses (phys or mag) for everything except auto attacks.
        /// </summary>
        public int WeaponDamage { get; private set; }
        /// <summary>
        /// Returns the multiplier to damage from weapon damage.
        /// </summary>
        public int WeaponDamageMultiplier { get; private set; }
        /// <summary>
        /// Returns the weapon damage of the type this class uses (phys or mag) for auto-attacks specifically.
        /// </summary>
        public int AutoWeaponDamage { get; private set; }
        /// <summary>
        /// Returns the multiplier to auto-attack damage from weapon damage.
        /// </summary>
        public int AutoWeaponDamageMultiplier { get; private set; }

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
                        WeaponDamageMultiplier = StatMath.GetWeaponDamageMultiplier(WeaponDamage, Job);
                        AutoWeaponDamage = value;
                        AutoWeaponDamageMultiplier = StatMath.GetWeaponDamageMultiplier(WeaponDamage, Job);
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
                        AutoWeaponDamageMultiplier = StatMath.GetWeaponDamageMultiplier(WeaponDamage, Job);
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
                        WeaponDamageMultiplier = StatMath.GetWeaponDamageMultiplier(WeaponDamage, Job);
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
        public int SkillSpeed { 
            get {
                return skillSpeed;
            } 
            init {
                skillSpeed = value;
                if (PhysicalClass) {
                    RelevantSpeed = value;
                    RelevantSpeedDotMultiplier = StatMath.GetDotMultiplierFromSpeed(value);
                }
            } 
        }
        private int skillSpeed;
        public int SpellSpeed {
            get {
                return spellSpeed;
            }
            init {
                spellSpeed = value;
                if (!PhysicalClass) {
                    RelevantSpeed = value;
                    RelevantSpeedDotMultiplier = StatMath.GetDotMultiplierFromSpeed(value);
                }
            }
        }
        private int spellSpeed;
        /// <summary>
        /// Returns skill speed for physical classes, and spell speed for magical ones.
        /// </summary>
        public int RelevantSpeed { get; private set; }
        /// <summary>
        /// Returns the multiplier to dot damage applied by the speed stat.
        /// </summary>
        public int RelevantSpeedDotMultiplier { get; private set; }


        #region Temporary Buffs Section
        //this section contains properties representing temporary buffs to stats. These should be set *after* the baseline stats!
        public int StrengthBuff { 
            get {
                return strengthBuff;
            } 
            set {
                strengthBuff = value;
                int buffedStr = strength + strengthBuff;
                switch (Job) {
                    case EJobId.DRG:
                    case EJobId.MNK:
                    case EJobId.SAM:
                    case EJobId.PLD:
                    case EJobId.DRK:
                    case EJobId.WAR:
                    case EJobId.GNB:
                        //TODO EW Patch Stuff
                        RelevantAttackPower = buffedStr;
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        AutoAttackPower = buffedStr;
                        AutoAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        break;
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                        //TODO EW Patch Stuff
                        AutoAttackPower = buffedStr;
                        AutoAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        break;
                    default:
                        break;
                }
            }
        }
        private int strengthBuff = 0;

        public int DexterityBuff {
            get {
                return dexterityBuff;
            }
            set {
                dexterityBuff = value;
                int buffedDex = dexterity + value;
                switch (Job) {
                    case EJobId.NIN:
                    case EJobId.BRD:
                    case EJobId.DNC:
                    case EJobId.MCH:
                        RelevantAttackPower = buffedDex;
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        AutoAttackPower = buffedDex;
                        AutoAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        break;
                    default:
                        break;
                }
            }
        }
        private int dexterityBuff = 0;

        public int IntelligenceBuff {
            get {
                return intelligenceBuff;
            }
            set {
                intelligenceBuff = value;
                int buffedIntelligence = intelligence + value;
                switch (Job) {
                    case EJobId.SMN:
                    case EJobId.BLM:
                    case EJobId.RDM:
                    case EJobId.BLU:
                        RelevantAttackPower = buffedIntelligence;
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        break;
                    default:
                        break;
                }
            }
        }
        private int intelligenceBuff = 0;

        public int MindBuff {
            get {
                return mindBuff;
            }
            set {
                mindBuff = value;
                int buffedMind = mind + value;
                switch (Job) {
                    case EJobId.WHM:
                    case EJobId.AST:
                    case EJobId.SCH:
                        //TODO EW Patch Stuff
                        RelevantAttackPower = buffedMind;
                        RelevantAttackPowerMultiplier = StatMath.GetAttackPowerMultiplier(RelevantAttackPower, IsTank);
                        break;
                    default:
                        break;
                }
            }
        }
        private int mindBuff = 0;

        public int CritRateBuff {
            get {
                return critRateBuff;
            }
            set {
                critRateBuff = value;
                CritRate = StatMath.GetCritRate(CriticalHitSubstat) + value;
            }
        }
        private int critRateBuff = 0;

        public int DirectHitRateBuff {
            get {
                return dhRateBuff;
            }
            set {
                dhRateBuff = value;
                DirectHitRate = StatMath.GetDHRate(DirectHitSubstat) + value;
            }
        }
        private int dhRateBuff = 0;

        public void ResetBuffs() {
            StrengthBuff = DexterityBuff = IntelligenceBuff = MindBuff = 0;
            CritRateBuff = DirectHitRateBuff = 0;
        }
        #endregion
    }
}
