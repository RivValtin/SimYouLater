using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public class ActionInvoker {

        public SimulationResults SimulationResults { get; init; }
        public TimedElements.ActiveEffectTimer ActiveEffectTimer {get; init;}
        public TimedElements.RecastTimer RecastTimer { get; init; }
        public CharacterStats CharStats { get; init; }

        /// <summary>
        /// Invoke the effects of the listed action immediately (ignoring cast time, if any).
        /// </summary>
        /// <param name="action"></param>
        public void InvokeAction(ActionDef action, int currentTime) {
            //WARNING: Modify critRate/dhRate variables for DoT snapshots only. Auto crit/dh does NOT apply to DoT portions (e.g. bioblaster) and as such IR/REA should not modify them, and instead change the crit/dh multi directly after other crit/dh calculations are done.
            int critRate = CharStats.CritRate;
            int dhRate = CharStats.DirectHitRate;

            //---- Calculate potency multipliers
            float ePotencyMulti = 1.0f;
            float buffPotencyMulti = 1.0f;
            if (ActiveEffectTimer.GetActiveStacks("SMN_Devotion") > 0) {
                buffPotencyMulti *= 1.03f;
            }
            if (ActiveEffectTimer.GetActiveStacks("NIN_TrickAttack") > 0) {
                buffPotencyMulti *= 1.05f;
            }
            float critMulti = 1 + (critRate / 1000.0f) * (CharStats.CritBonus / 1000.0f);
            float detMulti = 1 + CharStats.DetBonus / 1000.0f;
            float dhMulti = 1 + (dhRate / 4000.0f);

            if (ActiveEffectTimer.GetActiveStacks("MCH_Reassemble") > 0 && action.IsWeaponskill) {
                critMulti = 1 + CharStats.CritBonus / 1000.0f;
                dhMulti = 1.25f;
                ActiveEffectTimer.RemoveEffect("MCH_Reassemble");
                SimLog.Detail("Applying reassemble", currentTime, action);
            }
            ePotencyMulti = buffPotencyMulti * critMulti * detMulti * dhMulti;

            //---- Apply the effect
            int potency = action.Potency;
            bool isComboed = true;
            if (!string.IsNullOrEmpty(action.ComboEffectId) && ActiveEffectTimer.GetActiveStacks(action.ComboEffectId) <= 0) {
                potency = action.UncomboedPotency;
                isComboed = false;
                SimLog.Warning("Combo action used outside of combo.", currentTime, action);
            }

            //---- Because MCH's hypercharge has a stupidly weird bonus effect that exists nowhere else, I'm just hardcoding it here.
            if (action.IsWeaponskill && ActiveEffectTimer.GetActiveStacks("MCH_Hypercharge") > 0) {
                potency += 20;
            }
            //---- Likewise, MCH wildfire is the only buff that cares about how many hits go off while it's active. But it applies it on expiration, so we tell ActiveEffectTimer about it here.
            if (ActiveEffectTimer.GetActiveStacks("MCH_Wildfire") > 0 && action.IsWeaponskill) {
                ActiveEffectTimer.WildfireEligibleHitApplied();
            }

            SimulationResults.totalPotency += potency; //TODO: Account for crit/dh/det?
            SimulationResults.totalEffectivePotency += potency * ePotencyMulti;
            SimLog.Info("Invoked action.", currentTime, action);

            if (isComboed) {
                foreach (EffectApplication effectApplication in action.AppliedEffects) {
                    int speed = 0;
                    if (CharStats.PhysicalClass) {
                        speed = CharStats.SkillSpeed;
                    } else {
                        speed = CharStats.SpellSpeed;
                    }
                    ActiveEffectTimer.ApplyEffect(effectApplication, critRate, CharStats.CritBonus, dhRate, speed, buffPotencyMulti * detMulti);
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
