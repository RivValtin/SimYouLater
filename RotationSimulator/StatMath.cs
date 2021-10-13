﻿using System;
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
            const int levelBasedConstant = 3300; //an arbitrary level-based constant. Currently hardcoded to lvl 80 value.
            const int baseSpeed = 380; //the base substat you have with nothing increasing it. Increases with level, currently hardcoded to level 80 value.

            int speedSpread = speedSubstat - baseSpeed;
            int bareMultiplier = 1000 - 130 * speedSpread / levelBasedConstant;

            return bareMultiplier * baseCast / 1000;
        }

        /// <summary>
        /// Returns the multiplier to apply to dot ticks based on speed. Also applies to auto-attacks, I believe.
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public static int GetDotMultiplierFromSpeed(int speed) {
            const int levelBasedConstant = 3300; //an arbitrary level-based constant. Currently hardcoded to lvl 80 value.
            const int baseSpeed = 380; //the base substat you have with nothing increasing it. Increases with level, currently hardcoded to level 80 value.

            int speedSpread = speed - baseSpeed;
            int bareMultiplier = 130 * speedSpread / levelBasedConstant;

            return bareMultiplier;
        }

        /// <summary>
        /// Get the crit chance, in tenths of a percent, from crit substat value.
        /// </summary>
        public static int GetCritRate(int critSubstat) {
            const int levelBasedConstant = 3300;
            const int baseCrit = 380;

            int statSpread = critSubstat - baseCrit;
            int bonus = 200 * statSpread / levelBasedConstant + 50;

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
            const int levelBasedConstant = 3300;
            const int baseDirect = 380;

            int statSpread = directHitSubstat - baseDirect;
            int rate = (int)(550 * statSpread / levelBasedConstant);

            return rate;
        }

        /// <summary>
        /// Get the bonus damage from det, in tenths of a percent, from det substat value.
        /// </summary>
        public static int GetDetBonus(int detSubstat) {
            const int levelBasedConstant = 3300;
            const int baseDet = 340;

            int statSpread = detSubstat - baseDet;
            int bonus = 130 * statSpread / levelBasedConstant;

            return bonus;
        }

        /// <summary>
        /// Converts attack power into a multiplier on potency, in integer percent. (i.e. a value of 153 meants to multiply by 1.53)
        /// </summary>
        /// <param name="attackPower"></param>
        /// <returns></returns>
        public static int GetAttackPowerMultiplier (int attackPower, bool isTank) {
            int baseMultiplier = isTank ? 115 : 165; //does not vary with level

            return baseMultiplier * (attackPower - 340) / 340 + 100;
        }
    }
}
