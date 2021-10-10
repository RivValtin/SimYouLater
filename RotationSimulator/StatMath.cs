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
            int bareMultiplier = 1000 - (int)(130.0d * speedSpread / levelBasedConstant);

            return (int)(bareMultiplier * baseCast / 1000.0d);
        }
    }
}
