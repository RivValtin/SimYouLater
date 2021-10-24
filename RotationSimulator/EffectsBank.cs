using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public static class EffectsBank
    {
        public static readonly Dictionary<string, EffectDef> effects = new Dictionary<string, EffectDef>();
        
        static EffectsBank() {
            #region Role
            effects.Add("Role_Swiftcast", new EffectDef()
            {
                UniqueID = "Role_Swiftcast",
                DisplayName = "Swiftcast",
            });
            #endregion

            #region Astrologian
            effects.Add("AST_Divination", new EffectDef()
            {
                UniqueID = "AST_Divination",
                DisplayName = "Divination"
            });
            #endregion

            #region Bard
            effects.Add("BRD_BattleVoice", new EffectDef()
            {
                UniqueID = "BRD_BattleVoice",
                DisplayName = "Battle Voice"
            });
            effects.Add("BRD_MagesBallad_Party", new EffectDef()
            {
                UniqueID = "BRD_MagesBallad_Party",
                DisplayName = "Mage's Ballad"
            });
            effects.Add("BRD_ArmysPaeon_Party", new EffectDef()
            {
                UniqueID = "BRD_ArmysPaeon_Party",
                DisplayName = "Army's Paeon"
            });
            effects.Add("BRD_WanderersMinuet_Party", new EffectDef()
            {
                UniqueID = "BRD_WanderersMinuet_Party",
                DisplayName = "Wanderer's Minuet"
            });
            #endregion

            #region Dancer
            effects.Add("DNC_TechnicalFinish", new EffectDef()
            {
                UniqueID = "DNC_TechnicalFinish",
                DisplayName = "Technical Finish"
            }); 
            effects.Add("DNC_StandardFinish", new EffectDef()
            {
                UniqueID = "DNC_StandardFinish",
                DisplayName = "Standard Finish"
            });
            effects.Add("DNC_Devilment", new EffectDef()
            {
                UniqueID = "DNC_Devilment",
                DisplayName = "Devilment"
            });
            #endregion

            #region Dragoon
            effects.Add("DRG_BattleLitany", new EffectDef()
            {
                UniqueID="DRG_BattleLitany",
                DisplayName="Battle Litany"
            });
            //the buff for the dragoon
            effects.Add("DRG_RightEye", new EffectDef()
            {
                UniqueID = "DRG_RightEye",
                DisplayName = "Right Eye"
            });
            //the buff for the other person
            effects.Add("DRG_LeftEye", new EffectDef()
            {
                UniqueID = "DRG_LeftEye",
                DisplayName = "Left Eye"
            });
            #endregion

            #region Machinist
            effects.Add("MCH_SlugShotReady", new EffectDef()
            {
                UniqueID = "MCH_SlugShotReady",
                DisplayName = "(Slug Shot Ready)",
                Type = EEffectType.Combo
            });
            effects.Add("MCH_CleanShotReady", new EffectDef()
            {
                UniqueID = "MCH_CleanShotReady",
                DisplayName = "(Clean Shot Ready)",
                Type = EEffectType.Combo
            });
            effects.Add("MCH_Wildfire", new EffectDef()
            {
                UniqueID = "MCH_Wildfire",
                DisplayName = "Wildfire",
                Type = EEffectType.Debuff,
                IconName = "mch_wildfire.png",
            });
            effects.Add("MCH_Heat", new EffectDef()
            {
                UniqueID = "MCH_Heat",
                DisplayName = "Heat",
                Type = EEffectType.Resource,
            });
            effects.Add("MCH_Battery", new EffectDef()
            {
                UniqueID = "MCH_Battery",
                DisplayName = "Battery",
                Type = EEffectType.Resource,
            });
            effects.Add("MCH_Reassemble", new EffectDef()
            {
                UniqueID = "MCH_Reassemble",
                DisplayName = "Reassemble",
                Type = EEffectType.Buff,
                IconName = "mch_reassemble.png"
            });
            effects.Add("MCH_Hypercharge", new EffectDef()
            {
                UniqueID = "MCH_Hypercharge",
                DisplayName = "Hypercharge",
                Type = EEffectType.Buff,
                IconName = "mch_hypercharge.png"
            });
            effects.Add("MCH_Bioblaster", new EffectDef()
            {
                UniqueID = "MCH_Bioblaster",
                DisplayName = "Bioblaster",
                Type = EEffectType.Debuff,
                IconName = "mch_bioblaster.png",
                Potency = 60
            });
            #endregion

            #region Monk

            effects.Add("MNK_Brotherhood", new EffectDef()
            {
                UniqueID = "MNK_Brotherhood",
                DisplayName = "Brotherhood"
            });
            #endregion

            #region Ninja
            effects.Add("NIN_TrickAttack", new EffectDef()
            {
                UniqueID = "NIN_TrickAttack",
                DisplayName = "Trick Attack",
                Type = EEffectType.Debuff,
            });
            #endregion

            #region Red Mage
            effects.Add("RDM_Embolden_Party", new EffectDef()
            {
                UniqueID = "RDM_Embolden_Party",
                DisplayName = "Embolden",
                Type = EEffectType.Debuff
            });
            #endregion

            #region Scholar
            effects.Add("SCH_ChainStratagem", new EffectDef()
            {
                UniqueID = "SCH_ChainStratagem",
                DisplayName = "Chain Stratagem",
                Type = EEffectType.Debuff
            });
            #endregion

            #region Summoner
            effects.Add("SMN_SearingLight", new EffectDef()
            {
                UniqueID = "SMN_SearingLight",
                DisplayName = "Searing Light",
            });
            effects.Add("SMN_BahamutSummoned", new EffectDef()
            {
                UniqueID = "SMN_BahamutSummoned",
                DisplayName = "Bahamut Summoned",
                Type = EEffectType.Hidden
            });
            effects.Add("SMN_PhoenixAvailable", new EffectDef()
            {
                UniqueID = "SMN_PhoenixAvailable",
                DisplayName = "Phoenix Available",
                Type = EEffectType.Hidden
            });
            effects.Add("SMN_PhoenixSummoned", new EffectDef()
            {
                UniqueID = "SMN_PhoenixSummoned",
                DisplayName = "Phoenix Summoned",
                Type = EEffectType.Hidden
            });
            effects.Add("SMN_FireArcanum", new EffectDef()
            {
                UniqueID = "SMN_FireArcanum",
                DisplayName = "Fire Arcanum",
                Type = EEffectType.Hidden
            });
            effects.Add("SMN_WindArcanum", new EffectDef()
            {
                UniqueID = "SMN_WindArcanum",
                DisplayName = "Wind Arcanum",
                Type = EEffectType.Hidden
            });
            effects.Add("SMN_EarthArcanum", new EffectDef()
            {
                UniqueID = "SMN_EarthArcanum",
                DisplayName = "Earth Arcanum",
                Type = EEffectType.Hidden,
            });
            effects.Add("SMN_FireAttunement", new EffectDef()
            {
                UniqueID = "SMN_FireAttunement",
                DisplayName = "Fire Attunement",
                UsesStacks = true,
                Type = EEffectType.Resource,
            });
            effects.Add("SMN_IfritsFavor", new EffectDef()
            {
                UniqueID = "SMN_IfritsFavor",
                DisplayName = "Ifrit's Favor",
                Type = EEffectType.Hidden,
            });
            effects.Add("SMN_CrimsonStrikeReady", new EffectDef()
            {
                UniqueID = "SMN_CrimsonStrikeReady",
                DisplayName = "Crimson Strike Ready",
                UsesStacks = true,
                Type = EEffectType.Combo,
            });
            effects.Add("SMN_WindAttunement", new EffectDef()
            {
                UniqueID = "SMN_WindAttunement",
                DisplayName = "Wind Attunement",
                UsesStacks = true,
                Type = EEffectType.Resource,
            });
            effects.Add("SMN_GarudasFavor", new EffectDef()
            {
                UniqueID = "SMN_GarudasFavor",
                DisplayName = "Garuda's Favor",
                Type = EEffectType.Hidden,
            });
            effects.Add("SMN_Slipstream", new EffectDef()
            {
                UniqueID = "SMN_Slipstream",
                DisplayName = "Slipstream",
                Type = EEffectType.Ground,
                Potency = 30,
                Snapshots = false
            });
            effects.Add("SMN_EarthAttunement", new EffectDef()
            {
                UniqueID = "SMN_EarthAttunement",
                DisplayName = "Earth Attunement",
                UsesStacks = true,
                Type = EEffectType.Resource,
            });
            effects.Add("SMN_TitansFavor", new EffectDef()
            {
                UniqueID = "SMN_TitansFavor",
                DisplayName = "Titan's Favor",
                Type = EEffectType.Hidden,
            });
            effects.Add("SMN_FurtherRuin", new EffectDef()
            {
                UniqueID = "SMN_FurtherRuin",
                DisplayName = "Further Ruin",
            });
            effects.Add("SMN_Aetherflow", new EffectDef()
            {
                UniqueID = "SMN_Aetherflow",
                DisplayName = "Aetherflow",
                UsesStacks = true
            }); ;
            #endregion


        }
    }
}
