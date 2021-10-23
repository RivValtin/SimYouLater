using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public static class JobModifiers
    {
        private static int[,] jobModifierTable = new int[(int)EJobId._MAX, (int)EJobModifierId._MAX];
        static JobModifiers() {
            //TODO: Add GLA, PGL, MRD, LNC, ARC, CNJ, THM here for completeness.

            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.HP]  = 120;
            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.MP]  =  59;
            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.STR] = 100;
            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.VIT] = 110;
            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.DEX] =  95;
            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.INT] =  60;
            jobModifierTable[(int)EJobId.PLD, (int)EJobModifierId.MND] = 100;

            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.HP]  = 110;
            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.MP]  =  43;
            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.STR] = 110;
            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.DEX] = 105;
            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.INT] =  50;
            jobModifierTable[(int)EJobId.MNK, (int)EJobModifierId.MND] =  90;

            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.HP]  = 125;
            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.MP]  =  38;
            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.STR] = 105;
            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.VIT] = 110;
            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.DEX] =  95;
            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.INT] =  40;
            jobModifierTable[(int)EJobId.WAR, (int)EJobModifierId.MND] =  55;

            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.HP]  = 115;
            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.MP]  =  49;
            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.STR] = 115;
            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.VIT] = 105;
            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.INT] =  45;
            jobModifierTable[(int)EJobId.DRG, (int)EJobModifierId.MND] =  65;

            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.MP]  =  79;
            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.STR] =  90;
            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.DEX] = 115;
            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.INT] =  85;
            jobModifierTable[(int)EJobId.BRD, (int)EJobModifierId.MND] =  80;

            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.MP]  = 124;
            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.STR] =  55;
            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.DEX] = 105;
            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.INT] = 105;
            jobModifierTable[(int)EJobId.WHM, (int)EJobModifierId.MND] = 115;

            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.MP]  = 129;
            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.STR] =  45;
            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.INT] = 115;
            jobModifierTable[(int)EJobId.BLM, (int)EJobModifierId.MND] =  75;

            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.HP]  = 100;
            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.MP]  = 110;
            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.STR] =  85;
            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.VIT] =  95;
            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.DEX] =  95;
            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.INT] = 105;
            jobModifierTable[(int)EJobId.ACN, (int)EJobModifierId.MND] =  75;

            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.MP]  = 111;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.STR] =  90;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.INT] = 115;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.MND] =  80;

            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.MP]  = 119;
            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.STR] =  90;
            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.INT] = 105;
            jobModifierTable[(int)EJobId.SCH, (int)EJobModifierId.MND] = 115;

            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.HP]  = 103;
            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.MP]  =  38;
            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.STR] =  80;
            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.VIT] =  95;
            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.INT] =  60;
            jobModifierTable[(int)EJobId.ROG, (int)EJobModifierId.MND] =  70;

            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.HP]  = 108;
            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.MP]  =  48;
            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.STR] =  85;
            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.DEX] = 110;
            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.INT] =  65;
            jobModifierTable[(int)EJobId.NIN, (int)EJobModifierId.MND] =  75;

            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.MP]  =  79;
            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.STR] =  85;
            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.DEX] = 115;
            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.INT] =  80;
            jobModifierTable[(int)EJobId.MCH, (int)EJobModifierId.MND] =  85;

            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.HP]  = 120;
            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.MP]  =  79;
            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.STR] = 105;
            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.VIT] = 110;
            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.DEX] =  95;
            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.INT] =  60;
            jobModifierTable[(int)EJobId.DRK, (int)EJobModifierId.MND] =  40;

            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.MP]  = 124;
            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.STR] =  50;
            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.INT] = 105;
            jobModifierTable[(int)EJobId.AST, (int)EJobModifierId.MND] = 115;

            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.HP]  = 109;
            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.MP]  =  40;
            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.STR] = 112;
            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.DEX] = 108;
            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.INT] =  60;
            jobModifierTable[(int)EJobId.SAM, (int)EJobModifierId.MND] =  50;

            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.MP]  = 120;
            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.STR] =  55;
            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.DEX] = 105;
            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.INT] = 115;
            jobModifierTable[(int)EJobId.RDM, (int)EJobModifierId.MND] = 110;

            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.MP]  = 120;
            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.STR] =  70;
            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.DEX] = 110;
            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.INT] = 115;
            jobModifierTable[(int)EJobId.BLU, (int)EJobModifierId.MND] = 105;

            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.HP]  = 120;
            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.MP]  =  59;
            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.STR] = 100;
            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.VIT] = 110;
            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.DEX] =  95;
            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.INT] =  60;
            jobModifierTable[(int)EJobId.GNB, (int)EJobModifierId.MND] = 100;

            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.MP]  =  79;
            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.STR] =  90;
            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.DEX] = 115;
            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.INT] =  85;
            jobModifierTable[(int)EJobId.DNC, (int)EJobModifierId.MND] =  80;

            //TODO: EW Patch Stuff
        }

        public static int Get(EJobId jobId, EJobModifierId modId) {
            return jobModifierTable[(int)jobId, (int)modId];
        }

        /// <summary>
        /// Returns the "main stat" of each job. Note that this is not the same as
        /// main *damage* stat, since tanks return vitality from this function.
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static EJobModifierId GetMainStat(EJobId job) {
            switch (job) {
                case EJobId.PLD:
                case EJobId.WAR:
                case EJobId.DRK:
                case EJobId.GNB:
                    return EJobModifierId.VIT;
                case EJobId.WHM:
                case EJobId.AST:
                case EJobId.SCH:
                    //case EJobId.SGE: TODO: EW Patch Stuff
                    return EJobModifierId.MND;
                case EJobId.NIN:
                case EJobId.BRD:
                case EJobId.MCH:
                case EJobId.DNC:
                    return EJobModifierId.DEX;
                case EJobId.SMN:
                case EJobId.BLM:
                case EJobId.RDM:
                case EJobId.BLU:
                    return EJobModifierId.INT;
                default:
                    return EJobModifierId.STR;
            }
        }
    }
}
