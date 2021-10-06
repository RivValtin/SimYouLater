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

        public ActiveEffectTimer(int startTime) {
            currentTime = startTime;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
            foreach (ActiveEffect effect in activeEffects.Values) {
                if (effect.ActiveEndTime <= currentTime) {
                    Trace.WriteLine("\tExpired effect " + effect.effect.DisplayName + " at " + currentTime / 100.0f + "s");
                }
            }
            foreach (ActiveEffect effect in externalEffects) {
                if (effect.ActiveStartTime == currentTime) {
                    activeEffects.Add(effect.effect.UniqueID, effect);
                    Trace.WriteLine("\tApplied external effect " + effect.effect.DisplayName + " at " + currentTime / 100.0f + "s");
                }
            }
            activeEffects = activeEffects.Where(x => x.Value.ActiveEndTime > currentTime).ToDictionary(x => x.Key, x => x.Value);
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

        public void ApplyEffect(EffectApplication effectApplication) {
            if (activeEffects.ContainsKey(effectApplication.effect.UniqueID)) {
                //Effect already present, update in-place.
                ActiveEffect existingEffect = activeEffects[effectApplication.effect.UniqueID];

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
                activeEffects.Add(effectApplication.effect.UniqueID, newEffect);
            }
            Trace.WriteLine("\tApplied effect " + effectApplication.effect.DisplayName + " at " + currentTime / 100.0f + "s");
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
    }
}
