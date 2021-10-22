using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    /// <summary>
    /// Used for both the player rotation and the pet rotation. However, the pet rotation gets modified sometimes by activated actions (such as Enkindle).
    /// </summary>
    public class StrictRotationTimedElement : ITimedElement
    {
        public ActiveEffectTimer ActiveEffectTimer { get; init; }
        public AnimationLockTimer AnimationLockTimer { get; init; }
        public CastTimer CastTimer { get; init; }
        public GCDTimer GCDTimer { get; init; }
        public RecastTimer RecastTimer { get; init; }
        public int CurrentTime { get; set; }
        public List<RotationStep> RotationSteps { get; init; }
        internal int currentStepIndice = 0;
        public ActionInvoker ActionInvoker { get; init; }
        public CharacterStats CharStats { get; init; }
        /// <summary>
        /// If defined, this rotation will output messages based on the relevant pet.
        /// </summary>
        public string PetName { get; set; } = null;
        /// <summary>
        /// Sets what time the rotation should hard end itself, refusing to execute further. If a pet, it should log a "withdraw" message.
        /// </summary>
        public int EndTime { get; set; } = int.MaxValue;

        public int DefaultAnimationLock { get; init; } = 70;

        public bool ActionsRemain { get {
                return currentStepIndice < RotationSteps.Count;
            } 
        }

        /// <summary>
        /// Resets the rotation to start anew, with a new rotation given by steps.
        /// </summary>
        /// <param name="steps"></param>
        public void ResetRotationWith(List<RotationStep> steps) {
            RotationSteps.Clear();
            RotationSteps.AddRange(steps);
            currentStepIndice = 0;
        }

        /// <summary>
        /// For when a wait is requested.
        /// </summary>
        private int waitUntil = 0;
        private bool gcdDriftWarningSent = false;

        public void AdvanceTime(int time) {
            CurrentTime += time;

            if (EndTime == CurrentTime) {
                SimLog.Info("Pet withdraws from battlefield.", CurrentTime, instigator: PetName);
                return;
            } else if (EndTime < CurrentTime) {
                return;
            }

            //If we're out of steps, we done.
            if (currentStepIndice >= RotationSteps.Count)
                return;

            //--- Check for a "wait" command.
            if (waitUntil > CurrentTime) {
                return;
            }

            //--- If it's an action step, grab the action here so we can detect drift.
            ActionDef currentAction = null;
            if (RotationSteps[currentStepIndice].Type == ERotationStepType.Action) {
                string actionDefId = RotationSteps[currentStepIndice].Parameters["action"];
                currentAction = ActionBank.actions[actionDefId];

                if (currentAction.IsGCD && AnimationLockTimer.IsAnimationLocked && GCDTimer.IsGCDAvailable) {
                    if (!gcdDriftWarningSent) {
                        SimLog.Warning("GCD drift detected.", CurrentTime, currentAction);
                        gcdDriftWarningSent = true;
                    }
                }
            }

            if (AnimationLockTimer.IsAnimationLocked ||
                CastTimer.IsCasting) {
                return;
            }

            gcdDriftWarningSent = false;

            switch (RotationSteps[currentStepIndice].Type) {
                case ERotationStepType.Action:
                    if (currentAction.IsGCD && !GCDTimer.IsGCDAvailable)
                        return;

                    ActionDef recastAction = currentAction;
                    if (!string.IsNullOrEmpty(currentAction.CooldownID)) {
                        recastAction = ActionBank.actions[currentAction.CooldownID];
                    }

                    if (RecastTimer.GetAvailableCharges(recastAction) <= 0) {
                        if (currentAction.IsGCD && GCDTimer.IsGCDAvailable) {
                            SimLog.Warning("GCD drift detected.", CurrentTime);
                        }
                        return;
                    }

                    if (!PreconditionsMet(currentAction)) {
                        SimLog.Error("Aborting activation due to failed precondition.", CurrentTime, currentAction);
                        currentStepIndice++;
                        return;
                    }

                    if (currentAction.CastTime > 0) {
                        if (ActiveEffectTimer.GetActiveStacks("Role_Swiftcast") > 0) {
                            ActionInvoker.InvokeAction(currentAction, CurrentTime, "Player");
                        } else {
                            CastTimer.StartCasting(currentAction, GetScaledCastTime(currentAction));
                        }
                    } else {
                        ActionInvoker.InvokeAction(currentAction, CurrentTime, "Player");
                    }
                    AnimationLockTimer.InvokeAnimationLock(currentAction.AnimationLockOverride > 0 ? currentAction.AnimationLockOverride : DefaultAnimationLock);
                    if (currentAction.IsGCD) {
                        GCDTimer.StartGCD(GetScaledGCDRecastTime(currentAction));
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

        private int GetScaledCastTime(ActionDef action) {
            if (action.IsWeaponskill) {
                return StatMath.GetCast(action.CastTime, CharStats.SkillSpeed);
            } else {
                return StatMath.GetCast(action.CastTime, CharStats.SpellSpeed);
            }
        }

        private int GetScaledGCDRecastTime(ActionDef action) {
            if (action.IsWeaponskill) {
                return StatMath.GetRecast(action.RecastGCD, CharStats.SkillSpeed);
            } else {
                return StatMath.GetRecast(action.RecastGCD, CharStats.SpellSpeed);
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
            //Rotation in strict mode doesn't time much itself, really. It mostly waits on other timers. 
            //This works because all factors of its timing - gcd rolling, animation lock, cast times, recasts, etc - are timed externally in a way that ensures
            //    that the AdvanceTime function will be called right on the boundaries of those events. 
            int nextEvent = int.MaxValue;
            if (waitUntil > CurrentTime) {
                nextEvent = waitUntil;
            }
            if (EndTime != int.MaxValue && EndTime < nextEvent && EndTime > CurrentTime) {
                nextEvent = EndTime;
            }

            return nextEvent;
        }

        /// <summary>
        /// If true, the simulator will know it's time to start wrapping things up.
        /// </summary>
        /// <returns></returns>
        public bool IsRotationExhausted() {
            return currentStepIndice >= RotationSteps.Count && !CastTimer.IsCasting;
        }
    }
}
