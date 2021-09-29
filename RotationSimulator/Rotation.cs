using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public class Rotation {
        /// <summary>
        /// Name to display to the user. Also used as a unique ID.
        /// </summary>
        public string DisplayName { get; set; } = "UNNAMED ROTATION";
        /// <summary>
        /// 3-letter code representing the job this rotation is for. All caps. For obvious reasons, should not be changed.
        /// </summary>
        public string JobCode { get; init; } = "SMN";
        /// <summary>
        /// The amount of time to offset the first action from the start of the pull in centiseconds. 
        /// For example, 1500 means the first action will be treated as happening 15s before the pull.
        /// </summary>
        public int StartTimeOffset { get; set; } = 0;
        /// <summary>
        /// The list of rotation steps that make up the full rotation.
        /// Note that changing the elements of RotationSteps is fine. The init specifier is solely to avoid the list itself from being dynamically swapped.
        /// </summary>
        public List<RotationStep> RotationSteps { get; init; } = new List<RotationStep>();
    }
}
