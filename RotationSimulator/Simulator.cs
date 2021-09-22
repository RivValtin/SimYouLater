﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// All times in centiseconds. For the moment is just a potency counter.
    /// 
    /// Effects of substats NYI.
    /// </summary>
    public class Simulator
    {
        public int AnimationLock = 70;
        /// <summary>
        /// In tenths of a percent (so 20% is stored as 200).
        /// </summary>
        public int critRate = 205;
        /// <summary>
        /// In tenths of a percent (so 40% is stored as 400).
        /// </summary>
        public int critBonus = 555;
        /// <summary>
        /// In tenths of a percent
        /// </summary>
        public int directHitRate = 473;
        /// <summary>
        /// In tenths of a percent.
        /// </summary>
        public int detBonus = 86;

        private Dictionary<EActiveEffect, ActiveEffect> activeEffects;
        private List<ActiveEffect> extEffects;
        private int time = 0;

        private void ResetState() {
            activeEffects = new Dictionary<EActiveEffect, ActiveEffect>();
            time = 0;
            extEffects = new List<ActiveEffect>();
        }

        /// <summary>
        /// Simulate the given rotation
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="timeOffset">Used when calculating openers, set a negative time value equal to the amount of time between your first ability usage and when the boss becomes active.</param>
        /// <param name="externalEffects">A list of effects that come from other sources, but may interact with your rotation. Used to apply the effects of party buffs and such.</param>
        /// <returns></returns>
        public SimulationResults Simulate(IEnumerable<RotationStep> rotation, int timeOffset = 0, List<ActiveEffect> externalEffects = null) {
            ResetState();
            time = timeOffset; //total time spent
            extEffects = externalEffects ?? new List<ActiveEffect>();

            int potency = 0; //total effect potency, before accounting for buffs, average crit/dh bonus, det bonus, etc
            float effectivePotency = 0; //total effect potency, after accounting for buffs, average crit/dh bonus, det bonus, etc
            int gcdTimeRemaining = 0; //time left on the GCD before it can be used

            foreach (RotationStep step in rotation) {
                int animLock = step.animationLockOverride != 0 ? step.animationLockOverride : AnimationLock;
                int startTime = time;

                if (step.IsGCD) {
                    //---- If GCD is still rolling, but next step is GCD, roll forward the clock to the point where it can be hit.
                    if (gcdTimeRemaining > 0) {
                        AdvanceTime(gcdTimeRemaining);
                        gcdTimeRemaining = 0;
                        startTime = time;
                    }

                    //---- Set initial GCD time remaining.
                    gcdTimeRemaining = step.recastGCD;
                }

                //---- Verify action is not being used too early against recast.
                if (step.recast > 0 && step.lastExecutedTime + step.recast > time) {
                    Trace.WriteLine("\tWARNING: " + step.DisplayName + " being executed before recast is over. Needs another " + (time - step.lastExecutedTime - step.recast)/-100.0f +"s");
                }

                //---- Set last used time on this action
                step.lastExecutedTime = time;

                //---- Apply cast time (if any)
                if (step.castTime > 0) {
                    AdvanceTime(step.castTime);
                    gcdTimeRemaining -= Math.Max(animLock, step.castTime);
                }

                //---- Apply the effect
                float ePotencyMulti = 1.0f;
                if (activeEffects.Keys.Contains(EActiveEffect.SMN_Devotion)) {
                    ePotencyMulti *= 1.03f;
                }
                if (activeEffects.Keys.Contains(EActiveEffect.NIN_TrickAttack)) {
                    ePotencyMulti *= 1.05f;
                }
                potency += step.potency; //TODO: Account for crit/dh/det?
                effectivePotency += step.potency * ePotencyMulti;
                Trace.WriteLine("Executed Ability " + step.DisplayName + " at " + (float)time / 100 + "s");
                foreach (EffectApplication effect in step.appliedEffects) {
                    ActiveEffect newEffect = new ActiveEffect()
                    {
                        type = effect.type,
                        ActiveStartTime = startTime,
                        ActiveEndTime = (effect.Duration == int.MaxValue ? int.MaxValue : startTime + effect.Duration),
                        DisplayName = effect.DisplayName,
                    };
                    activeEffects.Add(effect.type, newEffect);
                    Trace.WriteLine("\tApplied active effect " + newEffect.DisplayName + " at " + time / 100.0f + "s");
                }

                //---- Remove any effects that activating this ability is meant to strip.
                foreach (Tuple<EActiveEffect, int> removedEffect in step.removeEffectStacks) {
                    if (activeEffects.ContainsKey(removedEffect.Item1)) {
                        ActiveEffect e = activeEffects[removedEffect.Item1];

                        if (removedEffect.Item2 <= 0 || removedEffect.Item2 >= e.Stacks) {
                            Trace.WriteLine("\tRemoved active effect " + e.DisplayName + " at " + time / 100.0f + "s");
                            activeEffects.Remove(removedEffect.Item1);
                        } else {
                            Trace.WriteLine("\tReduced active effect " + e.DisplayName + " by " + removedEffect.Item2 + " stacks at " + time / 100.0f + "s");
                            e.Stacks -= removedEffect.Item2;
                        }
                    }
                }

                //---- Roll the clock past animation lock
                if (step.castTime < animLock) {
                    int extTimePassed = animLock - step.castTime;

                    AdvanceTime(extTimePassed);
                    gcdTimeRemaining -= extTimePassed;

                    gcdTimeRemaining = Math.Max(0, gcdTimeRemaining);
                }
            }

            SimulationResults results = new SimulationResults();
            results.pps = potency / ((float)time / 100);
            results.epps = effectivePotency / ((float)time / 100);
            results.totalTime = time;
            return results;
        }

        /// <summary>
        /// Advance time by the amount listed, applying new external effects and removing old buffs as needed.
        /// </summary>
        /// <param name="timeAdvanced"></param>
        private void AdvanceTime(int timeAdvanced) {
            int newTime = time + timeAdvanced;

            List<ActiveEffect> newEffects = extEffects.Where(x => x.ActiveStartTime <= newTime && x.ActiveStartTime > time).ToList();

            foreach (ActiveEffect e in newEffects) {
                activeEffects.Add(e.type, e);
                Trace.WriteLine("\tApplied active effect " + e.DisplayName + " at " + e.ActiveStartTime/100.0f + "s");
            }

            time = newTime;

            CleanExpiredBuffs();
        }

        private void CleanExpiredBuffs() {
            foreach (ActiveEffect e in activeEffects.Values) {
                if (e.ActiveEndTime <= time) {
                    Trace.WriteLine("\tRemoved active effect " + e.DisplayName + " at " + e.ActiveEndTime / 100.0f + "s");
                }
            }

            activeEffects = activeEffects.Where(x => x.Value.ActiveEndTime > time).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
