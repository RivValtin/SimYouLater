using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public enum ERotationStepType
    {
        /// <summary>
        /// Parameters:
        ///     "action" - (ActionDef) defining what action to execute.
        /// </summary>
        Action,
        /// <summary>
        /// Parameters:
        ///     "time" - (int?) Amount of time to wait. Note that "int?" is an actual type.
        /// </summary>
        Wait,
        ActionConditional,
        Bookmark,
        Jump
    }
    public class RotationStep
    {
        public ERotationStepType Type { get; init; } = ERotationStepType.Action;
        public Dictionary<string, object> parameters { get; init; } = new Dictionary<string, object>();
    }
}
