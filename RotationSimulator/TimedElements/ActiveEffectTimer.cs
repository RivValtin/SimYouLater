using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    public class ActiveEffectTimer : ITimedElement
    {
        private int currentTime = 0;
        Dictionary<string, ActiveEffect> activeEffects = new Dictionary<string, ActiveEffect>();

        //effects applied by another character/entity
        public List<ActiveEffect> externalEffects = new List<ActiveEffect>();
        public SimulationResults simResult;
        public CharacterStats CharStats;

        private int wildfireHitCounter = 0;
        public ActiveEffectTimer(int startTime) {
            currentTime = startTime;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
            foreach (ActiveEffect effect in activeEffects.Values) {
                if (effect.ActiveEndTime <= currentTime) {
                    SimLog.Detail("Expired effect. " + effect.effect.DisplayName, currentTime);
                    if (string.Equals(effect.effect.UniqueID, "MCH_Wildfire")) {
                        ApplyWildfireDamage(effect);
                    }
                }
            }
            foreach (ActiveEffect effect in externalEffects) {
                if (effect.ActiveStartTime == currentTime) {
                    activeEffects.Add(effect.effect.UniqueID, effect);
                    SimLog.Detail("Applied external effect. " + effect.effect.DisplayName, currentTime);
                }
            }
            activeEffects = activeEffects.Where(x => x.Value.ActiveEndTime > currentTime).ToDictionary(x => x.Key, x => x.Value);
        }

        public void ExpireWildfire() {
            if (!activeEffects.ContainsKey("MCH_Wildfire")) {
                return;
            }
            ActiveEffect wildfire = activeEffects["MCH_Wildfire"];
            ApplyWildfireDamage(wildfire);
            activeEffects.Remove("MCH_Wildfire");
        }

        private void ApplyWildfireDamage(ActiveEffect effect) {
            SimLog.Info("Wildfire applied damage at potency " + wildfireHitCounter * 200, currentTime);
            simResult.totalPotency += wildfireHitCounter * 200;
            simResult.totalEffectivePotency += wildfireHitCounter * 200 * effect.SnapshotMulti; //NOTE: Wildfire neither crits nor direct hits, so those are not factors here.
            wildfireHitCounter = 0;
        }

        public int NextEvent() {
            int earliestEventTime = int.MaxValue;
            foreach (ActiveEffect effect in activeEffects.Values) {
                if (effect.ActiveEndTime < earliestEventTime) {
                    earliestEventTime = effect.ActiveEndTime;
                }
            }
            foreach (ActiveEffect effect in externalEffects) {
                if (effect.ActiveStartTime < earliestEventTime && effect.ActiveStartTime > currentTime) {
                    earliestEventTime = effect.ActiveStartTime;
                }
            }
            return earliestEventTime;
        }

        public void ApplyEffect(EffectApplication effectApplication, int critRate, int critBonus, int dh, int speed, float potencyMulti) {
            if (activeEffects.ContainsKey(effectApplication.effect.UniqueID)) {
                //Effect already present, update in-place.
                ActiveEffect existingEffect = activeEffects[effectApplication.effect.UniqueID];
                ApplySnapshot(existingEffect, GetBuffedCritRate(), CharStats.CritBonus, GetBuffedDHRate(), CharStats.RelevantSpeed, potencyMulti);

                //---- Update Duration
                int newEndTime;
                if (effectApplication.IsAdditiveDuration) {
                    newEndTime = existingEffect.ActiveEndTime + effectApplication.Duration;
                    newEndTime = Math.Min(newEndTime, effectApplication.DurationMax);
                } else {
                    newEndTime = effectApplication.Duration == int.MaxValue ? int.MaxValue : currentTime + effectApplication.Duration;
                }
                existingEffect.ActiveEndTime = newEndTime;

                //---- Update Stack Count
                int newStacks;
                if (effectApplication.IsAdditiveStacks) {
                    newStacks = existingEffect.Stacks + effectApplication.Stacks;
                    if (effectApplication.StackMax != 0) {
                        newStacks = Math.Min(newStacks, effectApplication.StackMax);
                    }
                } else {
                    newStacks = effectApplication.Stacks;
                }
                existingEffect.Stacks = newStacks;
            } else {
                //Effect not present, create it.
                ActiveEffect newEffect = new ActiveEffect()
                {
                    effect = effectApplication.effect,
                    ActiveStartTime = currentTime,
                    ActiveEndTime = (effectApplication.Duration == int.MaxValue ? int.MaxValue : currentTime + effectApplication.Duration),
                    Stacks = effectApplication.Stacks
                };

                ApplySnapshot(newEffect, critRate, critBonus, dh, speed, potencyMulti);

                activeEffects.Add(effectApplication.effect.UniqueID, newEffect);
            }
            SimLog.Detail("Applied effect." + effectApplication.effect.DisplayName, currentTime);
        }

        private void ApplySnapshot(ActiveEffect effect, int critRate, int critBonus, int dh, int speed, float multi) {
            effect.SnapshotCritRate = critRate;
            effect.SnapshotCritBonus = critBonus;
            effect.SnapshotDHRate = dh;
            effect.SnapshotMulti = multi;
            effect.SnapshotSpeed = speed;
        }

        /// <summary>
        /// Returns the number of currently active stacks for the given effect. 
        /// </summary>
        /// <param name="effectId"><= 0 if not active. 1 if an unstacked ability. Otherwise returns number of active stacks.</param>
        /// <returns></returns>
        public int GetActiveStacks(string effectId) {
            if (activeEffects.ContainsKey(effectId)) {
                ActiveEffect effect = activeEffects[effectId];
                return effect.Stacks;
            } else {
                return 0;
            }
        }

        /// <summary>
        /// Remove the listed number of stacks from the active effect. A stack quantity of 0 or less removes the effect entirely.
        /// </summary>
        /// <param name="effectId"></param>
        /// <param name="stacks"></param>
        public void RemoveStacks(string effectId, int stacks) {
            if (activeEffects.ContainsKey(effectId)) {
                ActiveEffect e = activeEffects[effectId];

                if (stacks <= 0 || stacks >= e.Stacks) {
                    activeEffects.Remove(effectId);
                } else {
                    e.Stacks -= stacks;
                }
            }
        }

        /// <summary>
        /// Removes the listed active effect.
        /// </summary>
        /// <param name="effectId"></param>
        public void RemoveEffect(string effectId) {
            RemoveStacks(effectId, 0);
        }

        public void WildfireEligibleHitApplied() {
            wildfireHitCounter++;
        }
        public void TickDots() {
            foreach (ActiveEffect effect in activeEffects.Values) {
                if (effect.effect.Potency != 0) {
                    simResult.totalPotency += effect.effect.Potency;

                    float effectivePotency; 

                    if (effect.effect.Snapshots) {
                        int speedMulti = StatMath.GetDotMultiplierFromSpeed(effect.SnapshotSpeed);
                        effectivePotency = effect.effect.Potency * (1 + effect.SnapshotCritBonus / 1000.0f * effect.SnapshotCritRate / 1000.0f) * (1 + effect.SnapshotDHRate / 4000.0f) * effect.SnapshotMulti * (1 + speedMulti / 1000.0f);
                    } else {
                        int speedMulti = StatMath.GetDotMultiplierFromSpeed(CharStats.RelevantSpeed);
                        effectivePotency = effect.effect.Potency * (1 + GetBuffedCritRate() / 1000.0f * CharStats.CritRate/ 1000.0f) * (1 + GetBuffedDHRate() / 4000.0f) * GetDamageMultiplier() * (1 + speedMulti / 1000.0f) * (1 + CharStats.DetBonus / 100.0f);
                    }
                    simResult.totalEffectivePotency += effectivePotency;
                    SimLog.Detail("Dot tick applied at potency " + effect.effect.Potency + " and effective potency " + effectivePotency, currentTime);
                }
            }
        }

        /// <summary>
        /// Returns the damage multiplier based on active buffs (and maybe someday, debuffs).
        /// </summary>
        /// <returns></returns>
        public float GetDamageMultiplier() {
            float buffPotencyMulti = 1.0f;
            if (GetActiveStacks("SMN_SearingLight") > 0) {
                buffPotencyMulti *= 1.03f;
            }
            if (GetActiveStacks("NIN_TrickAttack") > 0) {
                buffPotencyMulti *= 1.05f;
            }
            return buffPotencyMulti;
        }

        /// <summary>
        /// Get the character's current crit rate, as modified by active buffs.
        /// 
        /// Does NOT apply conditional buffs to crit rate, such as Reassemble, or guaranteed crits in general.
        /// </summary>
        /// <returns></returns>
        public int GetBuffedCritRate() {
            return CharStats.CritRate;
        }

        /// <summary>
        /// Get the character's current direct hit rate, as modified by active buffs.
        /// 
        /// Does NOT apply conditional buffs to dh rate, such as Reassemble, or guaranteed dh in general.
        /// </summary>
        /// <returns></returns>
        public int GetBuffedDHRate() {
            return CharStats.DirectHitRate;
        }
    }
}
