using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    public class CastTimer : ITimedElement
    {
        private int currentTime = 0;
        private int castTimeEnd = 0;

        private ActionDef castingAction = null;

        public ActionInvoker ActionInvoker { get; init; }
        public CastTimer(int startTime) {
            currentTime = startTime;
            castTimeEnd = startTime;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
            if (castingAction != null && currentTime >= castTimeEnd) {
                ActionInvoker?.InvokeAction(castingAction, currentTime);
                castingAction = null;
            }
        }

        public int NextEvent() {
            return castTimeEnd;
        }

        /// <summary>
        /// Begins a cast that will take the listed number of centiseconds to complete.
        /// </summary>
        /// <param name="time"></param>
        public void StartCasting(ActionDef action, int time) {
            castTimeEnd = currentTime + time;
            castingAction = action;
            Trace.WriteLine("(Started cast for " + action.DisplayName + " at " + currentTime / 100.0f + "s)");
        }

        public bool IsCasting { get {
                return castTimeEnd > currentTime;
            } }
    }
}
