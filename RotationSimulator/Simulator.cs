using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RotationSimulator.TimedElements;

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
        /// Set a maximum length to a simulation as a fail-safe. Since no fight in the game meets/exceeds 20m, this is currently set to 20m
        /// </summary>
        public int maximumReasonableFightLength = 20 * 60 * 100;

        public CharacterStats CharStats = new CharacterStats(EJobId.PLD);


        /// <summary>
        /// Simulate the given rotation
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="timeOffset">Used when calculating openers, set a negative time value equal to the amount of time between your first ability usage and when the boss becomes active.</param>
        /// <param name="externalEffects">A list of effects that come from other sources, but may interact with your rotation. Used to apply the effects of party buffs and such.</param>
        /// <returns></returns>
        public SimulationResults Simulate(List<RotationStep> rotationActions, int timeOffset = 0, List<ActiveEffect> externalEffects = null, ESimulationMode simMode = ESimulationMode.Simple) {

            //---- Insanely long initialization stpes
            int time = timeOffset;

            if (ESimulationMode.Variation1k == simMode || ESimulationMode.Variation10k == simMode) {
                int runCount = simMode == ESimulationMode.Variation10k ? 10000 : 1000;

                SimLog.Enabled = false;
                SimulationResults results = new SimulationResults();
                results.minDamage = int.MaxValue;
                results.maxDamage = 0;
                float dpsRunAccumulator = 0;

                SortedSet<float> dpsResults = new SortedSet<float>();

                for (int i = 0; i < runCount; i++) {
                    SimulationResults singlePassResult = RunSinglePass(rotationActions, timeOffset, externalEffects, ESimulationMode.SinglePass);
                    dpsResults.Add(singlePassResult.dps);
                    dpsRunAccumulator += singlePassResult.dps;
                }
                results.minDamage = dpsResults.Min;
                results.percentile05damage = dpsResults.ElementAt(dpsResults.Count * 5 / 100);
                results.percentile25damage = dpsResults.ElementAt(dpsResults.Count * 25 / 100);
                results.maxDamage = dpsResults.Max;
                results.percentile75damage = dpsResults.ElementAt(dpsResults.Count * 75 / 100);
                results.percentile95damage = dpsResults.ElementAt(dpsResults.Count * 95 / 100);
                results.averageDamage = dpsRunAccumulator / runCount;

                //Calculate standard deviation
                double sumOfSquareDifferences = 0;
                foreach (float sample in dpsResults) {
                    sumOfSquareDifferences += (sample - results.averageDamage) * (sample - results.averageDamage);
                }
                float standardDev = (float)Math.Sqrt(sumOfSquareDifferences / runCount);
                results.standardDeviation = standardDev;

                return results;
            } else {
                SimLog.Enabled = true;
                return RunSinglePass(rotationActions, timeOffset, externalEffects, simMode);
            }
        }

        private SimulationResults RunSinglePass(List<RotationStep> rotationActions, int timeOffset = 0, List<ActiveEffect> externalEffects = null, ESimulationMode simMode = ESimulationMode.Simple) {
            //---- Insanely long initialization steps
            int time = timeOffset;
            SimulationResults results = new SimulationResults();
            results.SimMode = simMode;

            ActiveEffectTimer activeEffectTimer = new ActiveEffectTimer(time)
            {
                externalEffects = externalEffects,
                simResult = results,
                CharStats = CharStats
            };
            AnimationLockTimer animationLockTimer = new AnimationLockTimer(time);
            AutoAttackTimer autoAttackTimer = new AutoAttackTimer()
            {

            }; ;
            GCDTimer gcdTimer = new GCDTimer(time);
            RecastTimer recastTimer = new RecastTimer(time)
            {
                CharStat = CharStats
            };
            ServerTickTimer serverTickTimer = new ServerTickTimer(time)
            {
                ActiveEffectTimer = activeEffectTimer
            };
            ActionInvoker actionInvoker = new ActionInvoker()
            {
                ActiveEffectTimer = activeEffectTimer,
                RecastTimer = recastTimer,
                SimulationResults = results,
                CharStats = CharStats
            };
            CastTimer castTimer = new CastTimer(time)
            {
                ActionInvoker = actionInvoker,
                AnimationLockTimer = animationLockTimer
            };
            AnimationLockTimer petAnimationLockTimer = new AnimationLockTimer(time);
            CastTimer petCastTimer = new CastTimer(time) { ActionInvoker = actionInvoker, Instigator = "Pet", AnimationLockTimer=petAnimationLockTimer };
            GCDTimer petGCDTimer = new GCDTimer(time);

            StrictRotationTimedElement petRotation = new StrictRotationTimedElement()
            {
                ActionInvoker = actionInvoker,
                ActiveEffectTimer = activeEffectTimer,
                AnimationLockTimer = petAnimationLockTimer,
                CastTimer = petCastTimer,
                CurrentTime = time,
                DefaultAnimationLock = AnimationLock,
                GCDTimer = petGCDTimer,
                RecastTimer = recastTimer,
                RotationSteps = new List<RotationStep>(),
                CharStats = CharStats
            };
            actionInvoker.PetHandler = petRotation;
            StrictRotationTimedElement strictRotation = new StrictRotationTimedElement()
            {
                ActionInvoker = actionInvoker,
                ActiveEffectTimer = activeEffectTimer,
                AnimationLockTimer = animationLockTimer,
                CastTimer = castTimer,
                CurrentTime = time,
                DefaultAnimationLock = AnimationLock,
                GCDTimer = gcdTimer,
                RecastTimer = recastTimer,
                RotationSteps = rotationActions,
                CharStats = CharStats
            };
            //Any timed elements that eventually end their jobs naturally.
            IEnumerable<ITimedElement> finiteTimedElements = new List<ITimedElement>()
            {
                activeEffectTimer,
                animationLockTimer,
                castTimer,
                gcdTimer,
                petAnimationLockTimer,
                petCastTimer,
                petGCDTimer,
                recastTimer,
                strictRotation,
                petRotation
            };
            //Timed elements that continue indefinitely, even without outside input.
            IEnumerable<ITimedElement> infiniteTimedElements = new List<ITimedElement>()
            {
                autoAttackTimer,
                serverTickTimer
            };
            IEnumerable<ITimedElement> allTimedElements = finiteTimedElements.Concat(infiniteTimedElements);

            //---- The main time advancing loop. 
            bool rotationComplete = false;
            int newTime = time;
            while (!rotationComplete && time < maximumReasonableFightLength) {
                //-- Advance time
                foreach (ITimedElement element in allTimedElements) {
                    element.AdvanceTime(newTime - time);
                }
                time = newTime;

                //-- Discover next time to advance to.
                newTime = int.MaxValue;
                foreach (ITimedElement element in finiteTimedElements) {
                    int nextEventTime = element.NextEvent();
                    if (nextEventTime < newTime && nextEventTime > time) {
                        newTime = nextEventTime;
                    }
                }

                if (!strictRotation.ActionsRemain) {
                    rotationComplete = true;
                } else {
                    foreach (ITimedElement element in infiniteTimedElements) {
                        int nextEventTime = element.NextEvent();
                        if (nextEventTime < newTime && nextEventTime > time) {
                            newTime = nextEventTime;
                        }
                    }
                }
            }
            CharStats.ResetBuffs();
            //---- Results!.. were tallied as we went
            return results;
        }
    }
}
