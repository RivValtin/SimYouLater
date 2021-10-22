using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// Just stores some conversion functions from substat->usable values.
    /// </summary>
    public static class StatMath
    {
        //All three constants below are level-based. These are the lvl 80 ShB values, hardcoded for now.
        //TODO: EW Patch Stuff
        public const int LEVEL_MAIN = 340;
        public const int LEVEL_SUB = 380;
        public const int LEVEL_DIV = 3300;

        /// <summary>
        /// Gets the sks/sps-modified GCD value based on the raw substat amount.
        /// 
        /// Note that this cannot return a value below 150 (1.5s), as no scaling recast is allowed to go below that value by the game.
        /// </summary>
        /// <param name="baseRecast">Base recast in centiseconds.</param>
        /// <param name="speedSubstat"></param>
        /// <returns></returns>
        public static int GetRecast(int baseRecast, int speedSubstat) {
            int castTime = GetCast(baseRecast, speedSubstat);
            if (castTime < 150) {
                return 150;
            } else {
                return castTime;
            }
        }

        /// <summary>
        /// Gets the sks/sps-modified cast time based on raw substat amount.
        /// </summary>
        /// <param name="baseCast"></param>
        /// <param name="speedSubstat"></param>
        /// <returns></returns>
        public static int GetCast(int baseCast, int speedSubstat) {
            int speedSpread = speedSubstat - LEVEL_SUB;
            int bareMultiplier = 1000 - 130 * speedSpread / LEVEL_DIV;

            return bareMultiplier * baseCast / 1000;
        }

        /// <summary>
        /// Returns the multiplier to apply to dot ticks based on speed. Also applies to auto-attacks, I believe.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static int GetDotMultiplierFromSpeed(int speed) {
            int speedSpread = speed - LEVEL_SUB;
            int bareMultiplier = 130 * speedSpread / LEVEL_DIV;

            return bareMultiplier;
        }

        /// <summary>
        /// Get the crit chance, in tenths of a percent, from crit substat value.
        /// </summary>
        public static int GetCritRate(int critSubstat) {
            int statSpread = critSubstat - LEVEL_SUB;
            int bonus = 200 * statSpread / LEVEL_DIV + 50;

            return bonus;
        }

        /// <summary>
        /// Get the crit bonus, in tenths of a percent, from crit substat value.
        /// </summary>
        public static int GetCritBonus(int critSubstat) {
            return GetCritRate(critSubstat) + 350;
        }

        /// <summary>
        /// Get the direct hit rate, in tenths of a percent, from the direct hit substat value.
        /// </summary>
        /// <param name="directHitSubstat"></param>
        /// <returns></returns>
        public static int GetDHRate(int directHitSubstat) {
            int statSpread = directHitSubstat - LEVEL_SUB;
            int rate = 550 * statSpread / LEVEL_DIV;

            return rate;
        }

        /// <summary>
        /// Get the bonus damage from det, in tenths of a percent, from det substat value.
        /// </summary>
        public static int GetDetBonus(int detSubstat) {
            int statSpread = detSubstat - LEVEL_MAIN;
            int bonus = 130 * statSpread / LEVEL_DIV;

            return bonus;
        }

        /// <summary>
        /// Get the bonus damage from det, in tenths of a percent, from det substat value.
        /// </summary>
        public static int GetTenBonus(int tenSubstat) {
            int statSpread = tenSubstat - LEVEL_SUB;
            int bonus = 100 * statSpread / LEVEL_DIV;

            return bonus;
        }

        /// <summary>
        /// Converts attack power into a multiplier on potency, in integer percent. (i.e. a value of 153 meants to multiply by 1.53)
        /// </summary>
        /// <param name="attackPower"></param>
        /// <returns></returns>
        public static int GetAttackPowerMultiplier (int attackPower, bool isTank) {
            int baseMultiplier = isTank ? 115 : 165; //does not vary with level

            return baseMultiplier * (attackPower - LEVEL_MAIN) / LEVEL_MAIN + 100;
        }

        /// <summary>
        /// Returns the multiplier to potency from weapon damage in integer % (100 = 1*)
        /// </summary>
        /// <returns></returns>
        public static int GetWeaponDamageMultiplier (int weaponDamage, EJobId job) {
            int classConstant = GetWeaponDamageConstant(job);

            return LEVEL_MAIN * classConstant / 1000 + weaponDamage;
        }

        public static int GetWeaponDamageMultiplierForAutos(int weaponDamage, EJobId job) {
            int classConstant = GetWeaponDamageConstantForAutos(job);

            return LEVEL_MAIN * classConstant / 1000 + weaponDamage;
        }

        private static int GetWeaponDamageConstant(EJobId job) {
            switch (job) {
                case EJobId.CNJ:
                case EJobId.WHM:
                case EJobId.AST:
                case EJobId.SCH:
                //case EJobId.SGE: TODO: EW Patch Stuff
                    return JobModifiers.Get(job, EJobModifierId.MND);
                case EJobId.ROG:
                case EJobId.NIN:
                case EJobId.ARC:
                case EJobId.BRD:
                case EJobId.MCH:
                case EJobId.DNC:
                    return JobModifiers.Get(job, EJobModifierId.DEX);
                case EJobId.ACN:
                case EJobId.SMN:
                case EJobId.THM:
                case EJobId.BLM:
                case EJobId.RDM:
                case EJobId.BLU:
                    return JobModifiers.Get(job, EJobModifierId.INT);
                default:
                    return JobModifiers.Get(job, EJobModifierId.STR);
            }
        }

        private static int GetWeaponDamageConstantForAutos(EJobId job) {
            switch (job) {
                case EJobId.ROG:
                case EJobId.NIN:
                case EJobId.ARC:
                case EJobId.BRD:
                case EJobId.MCH:
                case EJobId.DNC:
                    return JobModifiers.Get(job, EJobModifierId.DEX);
                default:
                    return JobModifiers.Get(job, EJobModifierId.STR);
            }
        }

        /// <summary>
        /// Only functions for Str, dex, vit, int, mnd and doesn't include race modifier. 
        /// </summary>
        /// <param name="job"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        public static int GetBaseStat(EJobId job, EJobModifierId stat) {

            return LEVEL_MAIN * JobModifiers.Get(job, stat) / 100 + (JobModifiers.GetMainStat(job) == stat ? 48 : 0);
        }
    }
}
