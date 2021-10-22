﻿using System;
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
