using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public List<RotationStep> RotationSteps { get; init; }
        private int currentStepIndice = 0;
        public ActionInvoker ActionInvoker { get; init; }

        public int DefaultAnimationLock { get; init; } = 70;

        /// <summary>
        /// For when a wait is requested.
        /// </summary>
        private int waitUntil = 0;

        public void AdvanceTime(int time) {
            CurrentTime += time;
            if(AnimationLockTimer.IsAnimationLocked ||
                CastTimer.IsCasting)
                return;

            //If we're out of steps, we done.
            if (currentStepIndice >= RotationSteps.Count)
                return;

            switch (RotationSteps[currentStepIndice].Type) {
                case ERotationStepType.Action:
                    string actionDefId = RotationSteps[currentStepIndice].Parameters["action"];
                    ActionDef currentAction = ActionBank.actions[actionDefId];
                    if (currentAction.IsGCD && !GCDTimer.IsGCDAvailable)
                        return;

                    if (RecastTimer.GetAvailableCharges(currentAction) <= 0) {
                        return;
                    }

                    if (!PreconditionsMet(currentAction)) {
                        SimLog.Error("Aborting activation due to failed precondition.", CurrentTime, currentAction);
                        currentStepIndice++;
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
                    ActionDef recastAction = currentAction;
                    if (!string.IsNullOrEmpty(currentAction.CooldownID)) {
                        recastAction = ActionBank.actions[currentAction.CooldownID];
                    }
                    if (recastAction.Recast > 0) {
                        RecastTimer.ConsumeCharge(recastAction);
                    }
                    currentStepIndice++;
                    break;
                case ERotationStepType.Wait:
                    int waitTime = Int32.Parse(RotationSteps[currentStepIndice].Parameters["time"]);
                    waitUntil = CurrentTime + waitTime;
                    currentStepIndice++;
                    return;
                default:
                    //TODO: throw error? action conditional might make sense as well.
                    return;
            }
        }

        /// <summary>
        /// Verifies the buff/resource/etc requirements of the action are present.
        /// 
        /// Note that this function only verifies conditions that should make the rotation abort using the ability entirely, skipping a step in the rotation.
        /// In other words, only hard error conditions.
        /// </summary>
        /// <param name="action"></param>
        /// <returns>True if the action can be legally executed. False otherwise.</returns>
        public bool PreconditionsMet(ActionDef action) {
            foreach (EffectRequirement requirement in action.RequiredEffects) {
                if (ActiveEffectTimer.GetActiveStacks(requirement.effect.UniqueID) < requirement.Stacks) {
                    //TODO: Log an error here.
                    return false;
                }
            }

            foreach (EffectRequirement requirement in action.RequiredAbsentEffects) {
                if (ActiveEffectTimer.GetActiveStacks(requirement.effect.UniqueID) >= requirement.Stacks) {
                    //TODO: Log an error here.
                    return false;
                }
            }

            return true;
        }

        public int NextEvent() {
            //Rotation in strict mode doesn't time anything itself, really. It just waits on other timers. So this function just returns indefinite wait time.
            //This works because all factors of its timing - gcd rolling, animation lock, cast times, recasts, etc - are timed externally in a way that ensures
            //    that the AdvanceTime function will be called right on the boundaries of those events. So all this class has to do is check action validity.
            return waitUntil > CurrentTime ? waitUntil : int.MaxValue;
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
