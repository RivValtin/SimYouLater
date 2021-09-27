using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    class StrictRotationTimedElement : ITimedElement
    {
        public ActiveEffectTimer ActiveEffectTimer { get; init; }
        public AnimationLockTimer AnimationLockTimer { get; init; }
        public CastTimer CastTimer { get; init; }
        public GCDTimer GCDTimer { get; init; }
        public RecastTimer RecastTimer { get; init; }
        public int CurrentTime { get; set; }
        public List<ActionDef> RotationSteps { get; init; }
        private int currentStepIndice = 0;
        public ActionInvoker ActionInvoker { get; init; }

        public int DefaultAnimationLock { get; init; } = 70;

        public void AdvanceTime(int time) {
            CurrentTime += time;
            if(AnimationLockTimer.IsAnimationLocked ||
                CastTimer.IsCasting)
                return;

            //If we're out of steps, we done.
            if (currentStepIndice >= RotationSteps.Count)
                return;

            ActionDef currentAction = RotationSteps[currentStepIndice];
            if (currentAction.IsGCD && !GCDTimer.IsGCDAvailable)
                return;

            if (RecastTimer.GetAvailableCharges(currentAction) <= 0) {
                return;
            }

            if (currentAction.CastTime > 0) {
                CastTimer.StartCasting(currentAction, currentAction.CastTime); //TODO: Apply speed
            } else {
                ActionInvoker.InvokeAction(currentAction, CurrentTime);
            }
            AnimationLockTimer.InvokeAnimationLock(currentAction.AnimationLockOverride > 0 ? currentAction.AnimationLockOverride : DefaultAnimationLock);
            if (currentAction.IsGCD) {
                GCDTimer.StartGCD(currentAction.RecastGCD); //TODO: Apply speed
            }
            if (currentAction.Recast > 0) {
                RecastTimer.ConsumeCharge(currentAction);
            }
            currentStepIndice++;
        }

        public int NextEvent() {
            //Rotation in strict mode doesn't time anything itself, really. It just waits on other timers. So this function just returns indefinite wait time.
            //This works because all factors of its timing - gcd rolling, animation lock, cast times, recasts, etc - are timed externally in a way that ensures
            //    that the AdvanceTime function will be called right on the boundaries of those events. So all this class has to do is check action validity.
            return int.MaxValue;
        }

        /// <summary>
        /// If true, the simulator will know it's time to start wrapping things up.
        /// </summary>
        /// <returns></returns>
        public bool IsRotationExhausted() {
            return currentStepIndice >= RotationSteps.Count;
        }
    }
}
