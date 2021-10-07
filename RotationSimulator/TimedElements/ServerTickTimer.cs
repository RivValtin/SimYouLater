using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    /// <summary>
    /// Models events that occur on server tick, such as MP regeneration and DoTs.
    /// 
    /// Note that technically effects can be offset, despite the tick lengths being the same, but for simplicity this is not modeled.
    /// </summary>
    public class ServerTickTimer : ITimedElement
    {
        private int currentTime = 0;
        private int tickOffset = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime">Starting time of the simulation (e.g. -15s for 15s pull timer)</param>
        /// <param name="serverTickOffset">How much to move the server tick off from 0. Only values between 0 and 299 are valid. A value of e.g. 95 would mean the first post-pull server tick would happen at 0.95s.</param>
        public ServerTickTimer(int startTime, int serverTickOffset=0) {
            if (serverTickOffset < 0 || serverTickOffset >= 300) {
                throw new ArgumentException("Server tick offset cannot be outside the range [0,299]");
            }

            currentTime = startTime;
            tickOffset = serverTickOffset;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
            if (IsOneSecondTick()) {
                SimLog.Detail("Server tick: 1s.", currentTime);
                ServerTickOneSecond?.Invoke();
            }
            if (IsThreeSecondTick()) {
                SimLog.Detail("Server tick: 3s.", currentTime);
                ServerTickThreeSeconds?.Invoke();
                //TODO: MP
            }
        }

        public int NextEvent() {
            int timeUntilTick = 100 - (currentTime + tickOffset) % 100;
            return currentTime + timeUntilTick;
        }

        private bool IsOneSecondTick() {
            return (currentTime + tickOffset) % 100 == 0;
        }

        private bool IsThreeSecondTick() {
            return (currentTime + tickOffset) % 300 == 0;
        }

        public event Action ServerTickOneSecond;
        public event Action ServerTickThreeSeconds;
    }
}
