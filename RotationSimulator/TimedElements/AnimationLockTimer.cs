using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator.TimedElements
{
    public class AnimationLockTimer : ITimedElement
    {
        private int currentTime = 0;
        private int animationLockEnd = 0;

        public AnimationLockTimer(int startTime) {
            currentTime = startTime;
            animationLockEnd = startTime;
        }

        public void AdvanceTime(int time) {
            currentTime += time;
        }

        public int NextEvent() {
            return animationLockEnd;
        }

        /// <summary>
        /// Invoke an animation lock for the listed number of centiseconds.
        /// </summary>
        /// <param name="time"></param>
        public void InvokeAnimationLock(int time) {
            animationLockEnd = currentTime + time;
        }

        public bool IsAnimationLocked { get {
                return animationLockEnd > currentTime;
            } }
    }
}
