﻿using System;
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

        /// <summary>
        /// Invoke the effects of the listed action immediately (ignoring cast time, if any).
        /// </summary>
        /// <param name="action"></param>
        public void InvokeAction(ActionDef action, int currentTime) {
            //---- Calculate potency multipliers
            float ePotencyMulti = 1.0f;
            if (ActiveEffectTimer.GetActiveStacks("SMN_Devotion") > 0) {
                ePotencyMulti *= 1.03f;
            }
            if (ActiveEffectTimer.GetActiveStacks("NIN_TrickAttack") > 0) {
                ePotencyMulti *= 1.05f;
            }

            //---- Apply the effect
            int potency = action.Potency;
            bool isComboed = true;
            if (!string.IsNullOrEmpty(action.ComboEffectId) && ActiveEffectTimer.GetActiveStacks(action.ComboEffectId) <= 0) {
                potency = action.UncomboedPotency;
                isComboed = false;
                Trace.WriteLine("WARNING: Invoking below action uncombo'd!");
            }

            SimulationResults.totalPotency += potency; //TODO: Account for crit/dh/det?
            SimulationResults.totalEffectivePotency += potency * ePotencyMulti;
            if (action.IsGCD) {
                Trace.WriteLine("Invoked action " + action.DisplayName + " at " + currentTime / 100.0f + "s");
            } else {
                Trace.WriteLine("-->Invoked action " + action.DisplayName + " at " + currentTime / 100.0f + "s");
            }

            if (isComboed) {
                foreach (EffectApplication effectApplication in action.AppliedEffects) {
                    ActiveEffectTimer.ApplyEffect(effectApplication);
                }
            }

            //---- Remove any effects that activating this ability is meant to strip.
            foreach (Tuple<string, int> removedEffect in action.RemoveEffectStacks) {
                ActiveEffectTimer.RemoveStacks(removedEffect.Item1, removedEffect.Item2);
            }
            if (!string.IsNullOrEmpty(action.ComboEffectId)) {
                ActiveEffectTimer.RemoveStacks(action.ComboEffectId, 0);
            }

            SimulationResults.totalTime = currentTime;
        }
    }
}
