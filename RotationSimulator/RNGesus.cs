using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public static class RNGesus
    {
        private static Random random;

        static RNGesus() {
            random = new Random();
        }

        /// <summary>
        /// Returns a number between 95 and 105 representing the random variation of damage dealt on an attack.
        /// </summary>
        /// <returns></returns>
        public static int GetDamageVariance() {
            return 95 + random.Next(0, 10);
        }

        /// <summary>
        /// Rolls a % chance and returns true if it happens. e.g. passing "231" will roll a 23.1% chance, returning true 23.1% of the time, false the rest.
        /// 
        /// Chances >= 1000 will always return true. Chances <= 0 will always return false.
        /// </summary>
        /// <param name="chance"></param>
        /// <returns></returns>
        public static bool RollChance(int chance) {
            return random.Next(0, 999) < chance;
        }
    }
}
