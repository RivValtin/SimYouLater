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
            bool recalculateStats = false;
            foreach (ActiveEffect effect in activeEffects.Values) {
                if (effect.ActiveEndTime <= currentTime) {
                    if (effect.effect.StackDecayDuration > 0 && GetActiveStacks(effect.effect.UniqueID) > 1) {
                        effect.ActiveEndTime = currentTime + effect.effect.StackDecayDuration;
                        effect.Stacks--;
                    } else {
                        SimLog.Detail("Expired effect. " + effect.effect.DisplayName, currentTime);
                        if (effect.effect.IsStatModifier) {
                            recalculateStats = true;
                        }
                        if (string.Equals(effect.effect.UniqueID, "MCH_Wildfire")) {
                            ApplyWildfireDamage(effect);
                        }
                    }
                }
            }
            foreach (ActiveEffect effect in externalEffects) {
                if (effect.ActiveStartTime == currentTime) {
                    activeEffects.Add(effect.effect.UniqueID, effect);
                    if (effect.effect.IsStatModifier) {
                        recalculateStats = true;
                    }
                    SimLog.Detail("Applied external effect. " + effect.effect.DisplayName, currentTime);
                }
            }
            activeEffects = activeEffects.Where(x => x.Value.ActiveEndTime > currentTime).ToDictionary(x => x.Key, x => x.Value);
            if (recalculateStats) {
                UpdateCharStatsBonuses();
            }
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
                ApplySnapshot(existingEffect, CharStats.CritRate, CharStats.CritBonus, CharStats.DirectHitRate, CharStats.RelevantSpeed, potencyMulti);

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

            if (effectApplication.effect.IsStatModifier) {
                UpdateCharStatsBonuses();
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
                    if (effect.effect.Snapshots) {
                        simResult.ApplyPotency(effect.effect.Potency, CharStats, effect.SnapshotCritRate, effect.SnapshotDHRate, effect.SnapshotMulti, applyDotBonus:true);
                    } else {
                        simResult.ApplyPotency(effect.effect.Potency, CharStats, CharStats.CritRate, CharStats.DirectHitRate, GetDamageMultiplier(), applyDotBonus: true);
                    }
                }
            }
        }

        /// <summary>
        /// Returns the damage multiplier based on active buffs (and maybe someday, debuffs).
        /// TODO: Phase this out in favor of a version that actually uses the proper integer math.
        /// </summary>
        /// <returns></returns>
        public float GetDamageMultiplier() {
            float buffPotencyMulti = 1.0f;

            foreach (ActiveEffect effect in activeEffects.Values) {
                buffPotencyMulti *= 1.0f + effect.effect.DamageBuff * effect.Stacks / 100.0f;
            }
            return buffPotencyMulti;
        }

        private void UpdateCharStatsBonuses() {
            int critBonuses = 0;
            int dhBonuses = 0;
            CharStats.StrengthBuff = CharStats.IntelligenceBuff = CharStats.DexterityBuff = CharStats.MindBuff = 0;

            foreach (ActiveEffect effect in activeEffects.Values) {
                if (effect.effect.MainStatBuff == 0) {
                    continue;
                }
                switch (effect.effect.MainStatBuffed) {
                    case EJobModifierId.STR:
                        int strBuffAmount = CharStats.Strength * effect.effect.MainStatBuff / 100;
                        if (strBuffAmount > effect.effect.MainStatBuffCap) {
                            CharStats.StrengthBuff = effect.effect.MainStatBuffCap;
                        } else {
                            CharStats.StrengthBuff = strBuffAmount;
                        }
                        break;
                    case EJobModifierId.INT:
                        int intBuffAMount = CharStats.Intelligence * effect.effect.MainStatBuff / 100;
                        if (intBuffAMount > effect.effect.MainStatBuffCap) {
                            CharStats.IntelligenceBuff = effect.effect.MainStatBuffCap;
                        } else {
                            CharStats.IntelligenceBuff = intBuffAMount;
                        }
                        break;
                    case EJobModifierId.DEX:
                        int dexBuffAmount = CharStats.Dexterity * effect.effect.MainStatBuff / 100;
                        if (dexBuffAmount > effect.effect.MainStatBuffCap) {
                            CharStats.DexterityBuff = effect.effect.MainStatBuffCap;
                        } else {
                            CharStats.DexterityBuff = dexBuffAmount;
                        }
                        break;
                    case EJobModifierId.MND:
                        int mndBuffAmount = CharStats.Mind * effect.effect.MainStatBuff / 100;
                        if (mndBuffAmount > effect.effect.MainStatBuffCap) {
                            CharStats.MindBuff = effect.effect.MainStatBuffCap;
                        } else {
                            CharStats.MindBuff = mndBuffAmount;
                        }
                        break;
                    default:
                        break;
                }
            }
            if (GetActiveStacks("DRG_BattleLitany") > 0) {
                critBonuses += 100;
            }
            if (GetActiveStacks("SCH_ChainStratagem") > 0) {
                critBonuses += 100;
            }
            if (GetActiveStacks("DNC_Devilment") > 0) {
                critBonuses += 200;
                dhBonuses += 200;
            }
            if (GetActiveStacks("BRD_WanderersMinuet_Party") > 0) {
                critBonuses += 20;
            }
            if (GetActiveStacks("BRD_ArmysPaeon_Party") > 0) {
                dhBonuses += 30;
            }
            if (GetActiveStacks("BRD_BattleVoice") > 0) {
                dhBonuses += 200;
            }

            CharStats.CritRateBuff = critBonuses;
            CharStats.DirectHitRateBuff = dhBonuses;
        }
    }
}
