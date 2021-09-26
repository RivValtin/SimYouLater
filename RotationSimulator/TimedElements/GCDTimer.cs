using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    /// <summary>
    /// A timer that ensures that the rotation has a chance to see that the next GCD is available.
    /// </summary>
    public class GCDTimer : ITimedElement
    {
        private int currentTime = 0;
        private int gcdAvailableTime = 0;
        public GCDTimer(int startTime) {
            currentTime = startTime;
            gcdAvailableTime = startTime;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
        }

        public int NextEvent() {
            return gcdAvailableTime;
        }

        /// <summary>
        /// Begins rolling the GCD.
        /// </summary>
        /// <param name="time"></param>
        public void StartGCD(int time) {
            gcdAvailableTime = currentTime + time;
        }

        /// <summary>
        /// End the GCD prematurely (theoretically to handle canceling a cast).
        /// </summary>
        public void EndGCDPrematurely() {
            gcdAvailableTime = currentTime;
        }

        public bool IsGCDAvailable { get {
                return gcdAvailableTime <= currentTime;
            } 
        }
    }
}
