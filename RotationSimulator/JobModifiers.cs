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

            //TODO: Add WHM, BLM, ACN here

            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.HP]  = 105;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.MP]  = 111;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.STR] =  90;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.VIT] = 100;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.DEX] = 100;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.INT] = 115;
            jobModifierTable[(int)EJobId.SMN, (int)EJobModifierId.MND] =  80;

            //TODO: Add all the others
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
