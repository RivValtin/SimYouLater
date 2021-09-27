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
        private static int IdCounter = 0;
        /// <summary>
        /// An arbitrary ID only valid for a given run of the program, but is nonetheless unique (barring someone making literally billions of elements in one run).
        /// </summary>
        public int Id { get; } = IdCounter++;

        public ERotationStepType Type { get; init; } = ERotationStepType.Action;
        public Dictionary<string, object> parameters { get; init; } = new Dictionary<string, object>();
    }
}
