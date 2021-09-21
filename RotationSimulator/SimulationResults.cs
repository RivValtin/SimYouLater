namespace RotationSimulator
{
    public class SimulationResults
    {
        /// <summary>
        /// Potency per second. A raw total, not accounting for attack power, weapon damage, critical hit, direct hit, or determination.
        /// </summary>
        public float pps;

        /// <summary>
        /// As potency per second it does not account for AP or WD, but does account for crit/dh/det. NYI
        /// </summary>
        public float epps;

        /// <summary>
        /// Actual DPS number. NYI
        /// </summary>
        public float dps;

        public int totalTime;
        public int totalPotency;
        public float totalEffectivePotency;
    }
}