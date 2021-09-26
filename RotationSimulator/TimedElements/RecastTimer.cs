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
                int nextCharge = actionDef.Recast - (timeWhenFullyCharged[actionId] - currentTime) % actionDef.Recast;
                if (nextCharge < soonestRecast && nextCharge > 0) {
                    soonestRecast = nextCharge;
                }
            }

            return soonestRecast;
        }

        /// <summary>
        /// Consumes a charge of the listed action. Throws an exception if no charges available.
        /// </summary>
        /// <param name="action"></param>
        public void ConsumeCharge(ActionDef action) {
            if (timeWhenFullyCharged.ContainsKey(action.UniqueID)) {
                if (CalculateCharges(action, timeWhenFullyCharged[action.UniqueID]) < 1) { //this check is here to avoid redunandcy.
                    throw new ArgumentException("Cannot consume a charge for an action that is fully on cooldown.");
                }

                if (timeWhenFullyCharged[action.UniqueID] > currentTime) {
                    timeWhenFullyCharged[action.UniqueID] += action.Recast;
                } else {
                    timeWhenFullyCharged[action.UniqueID] = currentTime + action.Recast;
                }

            } else {
                timeWhenFullyCharged[action.UniqueID] = currentTime + action.Recast;
            }
        }

        private int CalculateCharges(ActionDef action, int fullyChargedTime) {
            if (fullyChargedTime <= currentTime) {
                return action.Charges;
            } else {
                return (fullyChargedTime - currentTime) / action.Recast;
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
