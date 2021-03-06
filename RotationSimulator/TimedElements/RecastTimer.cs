using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    /// <summary>
    /// Tracks recasts and available charges. 
    /// 
    /// NYI
    /// </summary>
    public class RecastTimer : ITimedElement
    {
        public CharacterStats CharStat { get; init; }

        int currentTime = 0;
        /// <summary>
        /// Maps ActionDef.UniqueID to the time stamp when it will be full recharged.
        /// Any action not in the dictionary is assumed to be fully charged, but
        /// actions in the dictionary could also be fully charged.
        /// </summary>
        Dictionary<string, int> timeWhenFullyCharged = new Dictionary<string, int>();

        public RecastTimer(int startTime) {
            currentTime = startTime;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
        }

        public int NextEvent() {
            int soonestRecast = int.MaxValue;

            foreach (string actionId in timeWhenFullyCharged.Keys) {
                if (timeWhenFullyCharged[actionId] <= currentTime) {
                    //skip anything that's obviously already fully charged.
                    continue;
                }

                ActionDef actionDef = ActionBank.actions[actionId];
                int scaledRecast = GetScaledRecast(actionDef);
                int fullChargeTime = timeWhenFullyCharged[actionId] - currentTime;
                int nextCharge = fullChargeTime % scaledRecast;

                //If a charge was gained right now, but we're not fully charged, then we want to report that *next* recast after that.
                if (nextCharge == 0 && fullChargeTime > 0) {
                    nextCharge = scaledRecast;
                }

                if (nextCharge < soonestRecast && nextCharge > 0) {
                    soonestRecast = nextCharge;
                }
            }

            return currentTime + soonestRecast;
        }
        /// <summary>
        /// Get the recast value, scaled appropriately for speed (where relevant).
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private int GetScaledRecast(ActionDef action) {
            int recast = action.Recast;
            if (action.RecastScalesWithSks) {
                recast = StatMath.GetRecast(recast, CharStat.SkillSpeed);
            }
            if (action.RecastScalesWithSps) {
                recast = StatMath.GetRecast(recast, CharStat.SpellSpeed);
            }
            return recast;
        }

        /// <summary>
        /// Recover the recast timer by the listed amount. For example, 1500 would act as though 15s more had passed for that cooldown.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="recovery"></param>
        public void RecoverRecast(string actionId, int recovery) {
            if (timeWhenFullyCharged.ContainsKey(actionId)) {
                timeWhenFullyCharged[actionId] -= recovery;
            }
        }

        /// <summary>
        /// Consumes a charge of the listed action. Throws an exception if no charges available.
        /// </summary>
        /// <param name="action"></param>
        public void ConsumeCharge(ActionDef action) {
            int scaledRecast = GetScaledRecast(action);
            if (timeWhenFullyCharged.ContainsKey(action.UniqueID)) {
                if (CalculateCharges(action, timeWhenFullyCharged[action.UniqueID]) < 1) {
                    throw new ArgumentException("Cannot consume a charge for an action that is fully on cooldown.");
                }

                if (timeWhenFullyCharged[action.UniqueID] > currentTime) {
                    timeWhenFullyCharged[action.UniqueID] += scaledRecast;
                } else {
                    timeWhenFullyCharged[action.UniqueID] = currentTime + scaledRecast;
                }
            } else {
                timeWhenFullyCharged[action.UniqueID] = currentTime + scaledRecast;
            }
        }

        private int CalculateCharges(ActionDef action, int fullyChargedTime) {
            if (fullyChargedTime <= currentTime) {
                return action.Charges;
            } else {
                return action.Charges - (fullyChargedTime - currentTime - 1) / GetScaledRecast(action) - 1;
            }
        }

        public int GetAvailableCharges(ActionDef action) {
            if (timeWhenFullyCharged.ContainsKey(action.UniqueID)) {
                return CalculateCharges(action, timeWhenFullyCharged[action.UniqueID]);
            } else {
                return action.Charges;
            }
        }
    }
}
