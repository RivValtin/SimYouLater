using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// Super basic interface so that various independent timed elements that happen during a fight can
    /// be kept synchronized by a generalized loop.
    /// </summary>
    public interface ITimedElement
    {
        /// <summary>
        /// Returns the timestamps, in centiseconds, of the next time this element will change state.
        /// 
        /// Can return either int.MaxValue or a time <= current time to indicate no planned next event.
        /// Even if that happens, this will be called again after time advances one step.
        /// Times equal to current time are treated as "no next event" to prevent simulation lockup.
        /// </summary>
        /// <returns></returns>
        int NextEvent();

        /// <summary>
        /// Advance time by the listed amount of centiseconds. Where possible, elements should process events that will now happen thanks to the advancement of time.
        /// 
        /// This function will only be called with a time of <= 0 once, on the first iteration, to get an initial action.
        /// </summary>
        /// <param name="time"></param>
        void AdvanceTime(int time);
    }
}
