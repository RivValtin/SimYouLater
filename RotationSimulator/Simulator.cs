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


        /// <summary>
        /// Simulate the given rotation
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="timeOffset">Used when calculating openers, set a negative time value equal to the amount of time between your first ability usage and when the boss becomes active.</param>
        /// <param name="externalEffects">A list of effects that come from other sources, but may interact with your rotation. Used to apply the effects of party buffs and such.</param>
        /// <returns></returns>
        public SimulationResults Simulate(List<RotationStep> rotationActions, int timeOffset = 0, List<ActiveEffect> externalEffects = null) {

            //---- Insanely long initialization stpes
            int time = timeOffset;
            SimulationResults results = new SimulationResults();

            ActiveEffectTimer activeEffectTimer = new ActiveEffectTimer(time)
            {
                externalEffects = externalEffects
            };
            ActionInvoker actionInvoker = new ActionInvoker()
            {
                ActiveEffectTimer = activeEffectTimer,
                SimulationResults = results
            };
            AnimationLockTimer animationLockTimer = new AnimationLockTimer(time);
            AutoAttackTimer autoAttackTimer = new AutoAttackTimer()
            {

            };;
            GCDTimer gcdTimer = new GCDTimer(time);
            RecastTimer recastTimer = new RecastTimer(time);
            ServerTickTimer serverTickTimer = new ServerTickTimer(time);
            CastTimer castTimer = new CastTimer(time)
            {
                ActionInvoker = actionInvoker
            };
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
                RotationSteps = rotationActions
            };
            //Any timed elements that eventually end their jobs naturally.
            IEnumerable<ITimedElement> finiteTimedElements = new List<ITimedElement>()
            {
                activeEffectTimer,
                animationLockTimer,
                castTimer,
                gcdTimer,
                recastTimer,
                strictRotation
            };
            //Timed elements that continue indefinitely, even without outside input.
            IEnumerable<ITimedElement> infiniteTimedElements = new List<ITimedElement>()
            {
                autoAttackTimer,
                serverTickTimer
            };
            IEnumerable<ITimedElement> allTimedElements = finiteTimedElements.Concat(infiniteTimedElements);

            //---- The dummy thin loop
            bool allFiniteElementsComplete = false;
            int newTime = time;
            while (!allFiniteElementsComplete) {
                //-- Advance time
                foreach (ITimedElement element in allTimedElements) {
                    element.AdvanceTime(newTime - time);
                }
                time = newTime;

                //-- Discover next time to advance to (MaxInt means we're done).
                newTime = int.MaxValue;
                foreach (ITimedElement element in finiteTimedElements) {
                    int nextEventTime = element.NextEvent();
                    if (nextEventTime < newTime && nextEventTime > time) {
                        newTime = nextEventTime;
                    }
                }
                if (newTime == int.MaxValue) {
                    allFiniteElementsComplete = true;
                } else {
                    foreach (ITimedElement element in infiniteTimedElements) {
                        int nextEventTime = element.NextEvent();
                        if (nextEventTime < newTime && nextEventTime > time) {
                            newTime = nextEventTime;
                        }
                    }
                }
            }

            //---- Results!.. were tallied by ActionInvoker as we went.
            return results;
        }
    }
}
