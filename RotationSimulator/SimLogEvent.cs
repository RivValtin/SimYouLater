using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public enum ESimLogLevel
    {
        /// <summary>
        /// Errors are anything that makes the rotation straight-up not work, such as trying to execute Heat Blast without Hypercharge.
        /// </summary>
        Error,
        /// <summary>
        /// Something that doesn't prevent the rotation from working, but is likely not intended. Like a GCD drift.
        /// </summary>
        Warning,
        /// <summary>
        /// Invoke Ability, Start Cast, End Cast (if cast time > 0)
        /// </summary>
        Info,
        /// <summary>
        /// Everything else (e.g. animaton lock ending, effects activating/expiring, etc).
        /// </summary>
        Detail
    }
    public class SimLogEvent
    {
        public string Message { get; init; }
        public int TimeStamp { get; init; }
        public ESimLogLevel LogLevel { get; init; } = ESimLogLevel.Detail;
        /// <summary>
        /// Can be null.
        /// </summary>
        public ActionDef RelevantAction { get; init; } = null;
    }
}
