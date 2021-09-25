using System;
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

        private Dictionary<string, ActiveEffect> activeEffects;
        private List<ActiveEffect> extEffects;
        private int time = 0;

        private void ResetState() {
            activeEffects = new Dictionary<string, ActiveEffect>();
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
        public SimulationResults Simulate(IEnumerable<ActionDef> rotationActions, int timeOffset = 0, List<ActiveEffect> externalEffects = null) {
            ResetState();
            time = timeOffset; //total time spent
            extEffects = externalEffects ?? new List<ActiveEffect>();

            int potency = 0; //total effect potency, before accounting for buffs, average crit/dh bonus, det bonus, etc
            float effectivePotency = 0; //total effect potency, after accounting for buffs, average crit/dh bonus, det bonus, etc
            int gcdTimeRemaining = 0; //time left on the GCD before it can be used

            //This is the time at which the cooldown will be *fully* returned. Charged abilities might be usable before this time stamp is reached
            //(e.g. if it has 3 charges at 30s each and you only have 1 left, this could be 55s in the future, which means you have 1 charge and 5s/30s of the next).
            Dictionary<string, int> fullyChargedTime = new Dictionary<string, int>();

            foreach (ActionDef step in rotationActions) {
                int animLock = step.AnimationLockOverride != 0 ? step.AnimationLockOverride : AnimationLock;
                int startTime = time;

                if (step.IsGCD) {
                    //---- If GCD is still rolling, but next step is GCD, roll forward the clock to the point where it can be hit.
                    if (gcdTimeRemaining > 0) {
                        AdvanceTime(gcdTimeRemaining);
                        gcdTimeRemaining = 0;
                        startTime = time;
                    }

                    //---- Set initial GCD time remaining.
                    gcdTimeRemaining = step.RecastGCD;
                }

                int fullCharge = fullyChargedTime.ContainsKey(step.CooldownID) ? fullyChargedTime[step.CooldownID] : int.MinValue;

                bool cancelExecution = false;
                //---- Verify action is not being used too early against recast.
                if (step.Recast > 0 && fullCharge > time) {
                    Trace.WriteLine("\tERROR: " + step.DisplayName + " being executed before recast is over. Needs another " + ((fullCharge - time)/-100.0f) +"s. Skipping action.");
                    cancelExecution = true;
                }

                //---- Verify that any required state for the action is present.
                foreach (EffectRequirement effectRequirement in step.RequiredEffects) {
                    if (!activeEffects.ContainsKey(effectRequirement.effect.UniqueID)) {
                        Trace.WriteLine("\tERROR: Required active effect " + effectRequirement.effect.DisplayName + " missing when trying to execute " + step.DisplayName + ". Skipping action.");
                        cancelExecution = true;
                    } else if (effectRequirement.Stacks > 1 && activeEffects[effectRequirement.effect.UniqueID].Stacks < effectRequirement.Stacks) {
                        Trace.WriteLine("\tERROR: Insufficient stacks of effect " + effectRequirement.effect.DisplayName + " when trying to execute " + step.DisplayName + ". Skipping action.");
                        cancelExecution = true;
                    }
                }

                //---- Verify that any state that *cannot* be present is not present.
                foreach (EffectRequirement effectRequirement in step.RequiredAbsentEffects) {
                    if (activeEffects.ContainsKey(effectRequirement.effect.UniqueID)) {
                        Trace.WriteLine("\tERROR: Active effect " + effectRequirement.effect.DisplayName + " prevents execution of " + step.DisplayName + ". Skipping action.");
                        cancelExecution = true;
                    }
                }
                if (cancelExecution) {
                    continue;
                }

                //---- Set last used time on this action
                fullyChargedTime[step.CooldownID] = (fullCharge == int.MinValue ? time+step.Recast : fullCharge+step.Recast);

                //---- Apply cast time (if any)
                if (step.CastTime > 0) {
                    AdvanceTime(step.CastTime);
                    gcdTimeRemaining -= Math.Max(animLock, step.CastTime);
                }

                //---- Apply the effect
                float ePotencyMulti = 1.0f;
                if (activeEffects.Keys.Contains("SMN_Devotion")) {
                    ePotencyMulti *= 1.03f;
                }
                if (activeEffects.Keys.Contains("NIN_TrickAttack")) {
                    ePotencyMulti *= 1.05f;
                }
                potency += step.Potency; //TODO: Account for crit/dh/det?
                effectivePotency += step.Potency * ePotencyMulti;
                Trace.WriteLine("Executed Ability " + step.DisplayName + " at " + (float)time / 100 + "s");
                foreach (EffectApplication effectApplication in step.AppliedEffects) {
                    ActiveEffect newEffect = new ActiveEffect()
                    {
                        effect = effectApplication.effect,
                        ActiveStartTime = startTime,
                        ActiveEndTime = (effectApplication.Duration == int.MaxValue ? int.MaxValue : startTime + effectApplication.Duration),
                    };
                    activeEffects.Add(effectApplication.effect.UniqueID, newEffect);
                    Trace.WriteLine("\tApplied active effect " + newEffect.effect.DisplayName + " at " + time / 100.0f + "s");
                }

                //---- Remove any effects that activating this ability is meant to strip.
                foreach (Tuple<string, int> removedEffect in step.RemoveEffectStacks) {
                    if (activeEffects.ContainsKey(removedEffect.Item1)) {
                        ActiveEffect e = activeEffects[removedEffect.Item1];

                        if (removedEffect.Item2 <= 0 || removedEffect.Item2 >= e.Stacks) {
                            Trace.WriteLine("\tRemoved active effect " + e.effect.DisplayName + " at " + time / 100.0f + "s");
                            activeEffects.Remove(removedEffect.Item1);
                        } else {
                            Trace.WriteLine("\tReduced active effect " + e.effect.DisplayName + " by " + removedEffect.Item2 + " stacks at " + time / 100.0f + "s");
                            e.Stacks -= removedEffect.Item2;
                        }
                    }
                }

                //---- Roll the clock past animation lock
                if (step.CastTime < animLock) {
                    int extTimePassed = animLock - step.CastTime;

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
                activeEffects.Add(e.effect.UniqueID, e);
                Trace.WriteLine("\tApplied active effect " + e.effect.DisplayName + " at " + e.ActiveStartTime/100.0f + "s");
            }

            time = newTime;

            CleanExpiredBuffs();
        }

        private void CleanExpiredBuffs() {
            foreach (ActiveEffect e in activeEffects.Values) {
                if (e.ActiveEndTime <= time) {
                    Trace.WriteLine("\tRemoved active effect " + e.effect.DisplayName + " at " + e.ActiveEndTime / 100.0f + "s");
                }
            }

            activeEffects = activeEffects.Where(x => x.Value.ActiveEndTime > time).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
