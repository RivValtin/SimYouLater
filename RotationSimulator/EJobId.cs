using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    /// <summary>
    /// These job ids must not change their integer values as they are used for persistence.
    /// </summary>
    public enum EJobId
    {
        GLA=1,
        PGL=2,
        MRD=3,
        LNC=4,
        ARC=5,
        CNJ=6,
        THM=7,
        PLD=19,
        MNK=20,
        WAR=21,
        DRG=22,
        BRD=23,
        WHM=24,
        BLM=25,
        ACN=26,
        SMN=27,
        SCH=28,
        ROG=29,
        NIN=30,
        MCH=31,
        DRK=32,
        AST=33,
        SAM=34,
        RDM=35,
        BLU=36,
        GNB=37,
        DNC=38,
        _MAX=39
            //TODO: EW Patch Stuff (add SGE/RPR)
    }
}
