using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RotationSimulator.TimedElements;

namespace RotationSimulator
{
    public class ActionInvoker {

        public SimulationResults SimulationResults { get; init; }
        public TimedElements.ActiveEffectTimer ActiveEffectTimer {get; init;}
        public TimedElements.RecastTimer RecastTimer { get; init; }
        public CharacterStats CharStats { get; init; }
        /// <summary>
        /// A sub-rotation for handling the pet. For obvious reasons, a pet should not itself define this. 
        /// 
        /// Note that it needs *its own* instance of GCDTimer, CastTimer, and AnimationLockTimer!
        /// </summary>
        public StrictRotationTimedElement PetHandler { get; set; } = null;

        /// <summary>
        /// Invoke the effects of the listed action immediately (ignoring cast time, if any).
        /// 
        /// Does not handle the summoning of pets.
        /// </summary>
        /// <param name="action"></param>
        public void InvokeAction(ActionDef action, int currentTime, string instigator) {
            //WARNING: Modify critRate/dhRate variables for DoT snapshots only. Auto crit/dh does NOT apply to DoT portions (e.g. bioblaster) and as such IR/REA should not modify them, and instead change the crit/dh multi directly after other crit/dh calculations are done.
            int critRate = CharStats.CritRate;
            int dhRate = CharStats.DirectHitRate;

            //---- Calculate potency multipliers
            // NOTE: This is not entirely accurate as the game does not use floating point math anywhere, but since this is only used for estimated effective potency it's ok.
            float ePotencyMulti = 1.0f;
            float buffPotencyMulti = ActiveEffectTimer.GetDamageMultiplier();
            float critMulti = 1 + (critRate / 1000.0f) * (CharStats.CritBonus / 1000.0f);
            float detTenMulti = (1 + CharStats.DetBonus / 1000.0f) * (1 + CharStats.TenBonus / 1000.0f);
            float dhMulti = 1 + (dhRate / 4000.0f);

            if (ActiveEffectTimer.GetActiveStacks("MCH_Reassemble") > 0 && action.IsWeaponskill) {
                critMulti = 1 + CharStats.CritBonus / 1000.0f;
                dhMulti = 1.25f;
                ActiveEffectTimer.RemoveEffect("MCH_Reassemble");
                SimLog.Detail("Applying reassemble", currentTime, action, instigator: instigator);
            }
            ePotencyMulti = buffPotencyMulti * critMulti * detTenMulti * dhMulti;

            //Check for comboed vs uncomboed potency.
            int potency = action.Potency;
            bool isComboed = true;
            if (!string.IsNullOrEmpty(action.ComboEffectId) && ActiveEffectTimer.GetActiveStacks(action.ComboEffectId) <= 0) {
                potency = action.UncomboedPotency;
                isComboed = false;
                SimLog.Warning("Combo action used outside of combo.", currentTime, action, instigator: instigator);
            }

            //---- Because MCH's hypercharge has a stupidly weird bonus effect that exists nowhere else, I'm just hardcoding it here.
            // NOTE: Now that I said that, it looks like SMN may have something like that at low level. Fuckin' 'ell.
            if (action.IsWeaponskill && ActiveEffectTimer.GetActiveStacks("MCH_Hypercharge") > 0 && instigator == "Player") {
                potency += 20;
            }
            //---- Likewise, MCH wildfire is the only buff that cares about how many hits go off while it's active. But it applies it on expiration, so we tell ActiveEffectTimer about it here.
            if (ActiveEffectTimer.GetActiveStacks("MCH_Wildfire") > 0 && action.IsWeaponskill && instigator == "Player") {
                ActiveEffectTimer.WildfireEligibleHitApplied();
            }

            //---- Apply the effect
            SimulationResults.totalPotency += potency;
            SimulationResults.totalEffectivePotency += potency * ePotencyMulti;
            SimLog.Info("Invoked action.", currentTime, action, instigator: instigator);

            //--- Summon Pet, if any.
            if (action.SummonedPet != null) {
                PetHandler.PetName = action.SummonedPet.Item1;
                PetHandler.EndTime = action.PetExpiration + currentTime;
                PetHandler.ResetRotationWith(action.SummonedPet.Item2);

                SimLog.Info("Pet joins the battlefield.", currentTime, instigator: PetHandler.PetName);
            }

            //--- If this action triggers a pet action, do so.
            if (action.TriggersPetAction != null) {
                PetHandler.RotationSteps.Insert(PetHandler.currentStepIndice, new RotationStep
                {
                    Type = ERotationStepType.Action,
                    Parameters = new RotationStep.RotationStepParameters()
                    {
                        {"action", action.TriggersPetAction.UniqueID }
                    }
                });
            }

            //--- Apply additional effects.
            if (isComboed) {
                foreach (EffectApplication effectApplication in action.AppliedEffects) {
                    ActiveEffectTimer.ApplyEffect(effectApplication, critRate, CharStats.CritBonus, dhRate, CharStats.RelevantSpeed, buffPotencyMulti * detTenMulti);
                }
            }

            //---- Remove any effects that activating this ability is meant to strip.
            foreach (Tuple<string, int> removedEffect in action.RemoveEffectStacks) {
                ActiveEffectTimer.RemoveStacks(removedEffect.Item1, removedEffect.Item2);
            }
            if (!string.IsNullOrEmpty(action.ComboEffectId)) {
                ActiveEffectTimer.RemoveStacks(action.ComboEffectId, 0);
            }

            //---- Perform recast resets
            foreach (Tuple<string,int> recastReset in action.CooldownReset) {
                RecastTimer.RecoverRecast(recastReset.Item1, recastReset.Item2);
            }

            SimulationResults.totalTime = currentTime;
        }
    }
}
