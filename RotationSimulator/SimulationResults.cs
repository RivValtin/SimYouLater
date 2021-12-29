namespace RotationSimulator
{
    public class SimulationResults
    {
        public ESimulationMode SimMode;
        /// <summary>
        /// Potency per second. A raw total, not accounting for attack power, weapon damage, critical hit, direct hit, or determination.
        /// </summary>
        public float pps { get {
                return totalPotency / (totalTime / 100.0f);
            } 
        }

        /// <summary>
        /// As potency per second it does not account for AP or WD, but does account for crit/dh/det/etc.
        /// </summary>
        public float epps {
            get {
                return totalEffectivePotency / (totalTime / 100.0f);
            }
        }

        /// <summary>
        /// Actual DPS number. 
        /// </summary>
        public float dps {
            get {
                return totalDamage / (totalTime / 100.0f);
            }
        }

        public int totalTime;
        public int totalPotency;
        public int totalDamage;
        public float totalEffectivePotency;

        //for variation runs
        public float minDamage;
        public float percentile05damage;
        public float percentile25damage;
        public float averageDamage;
        public float percentile75damage;
        public float percentile95damage;
        public float maxDamage;

        public float standardDeviation;

        public void ApplyPotency(int potency, CharacterStats charStats, int critRate, int dhRate, float buffMultipliers, bool applyDotBonus = false) {
            totalPotency += potency;
            int effectivePotency = potency * (1000 + charStats.DetBonus) / 1000 * (1000 + charStats.TenBonus) / 1000;
            switch (SimMode) {
                case ESimulationMode.Simple:
                    //apply a bonus equal to the *average* bonus equal to crit and direct hit.
                    effectivePotency = effectivePotency * (1000 + critRate * charStats.CritBonus / 1000) / 1000 * (1000 + dhRate * 250 / 1000) / 1000;
                    break;
                default:
                    if (RNGesus.RollChance(critRate)) {
                        effectivePotency = effectivePotency * (1000 + charStats.CritBonus) / 1000;
                    }
                    if (RNGesus.RollChance(dhRate)) {
                        effectivePotency = effectivePotency * 125 / 100;
                    }
                    effectivePotency = effectivePotency * RNGesus.GetDamageVariance() / 100;
                    break;
            }
            effectivePotency = (int)(effectivePotency * buffMultipliers);
            if (applyDotBonus) {
                effectivePotency = effectivePotency* (1000 + charStats.RelevantSpeedDotMultiplier) / 1000;
            }
            totalEffectivePotency += effectivePotency;
            totalDamage += effectivePotency * (1000 + charStats.BaseDamageBonus) / 1000 * charStats.WeaponDamageMultiplier / 100 * charStats.RelevantAttackPowerMultiplier / 100;
        }
    }
}