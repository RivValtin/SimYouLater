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

            #region Summoner
            effects.Add("SMN_Devotion", new EffectDef()
            {
                UniqueID = "SMN_Devotion",
                DisplayName = "Devotion",
            });
            effects.Add("SMN_BahamutSummoned", new EffectDef()
            {
                UniqueID = "SMN_BahamutSummoned",
                DisplayName = "Bahamut Summoned",
            });
            effects.Add("SMN_PhoenixAvailable", new EffectDef()
            {
                UniqueID = "SMN_PhoenixAvailable",
                DisplayName = "Phoenix Available",
            });
            effects.Add("SMN_IfritGem", new EffectDef()
            {
                UniqueID = "SMN_IfritGem",
                DisplayName = "Ifrit Gem Available",
            });
            effects.Add("SMN_GarudaGem", new EffectDef()
            {
                UniqueID = "SMN_GarudaGem",
                DisplayName = "Garuda Gem Available",
            });
            effects.Add("SMN_TitanGem", new EffectDef()
            {
                UniqueID = "SMN_TitanGem",
                DisplayName = "Titan Gem Available",
                Type = EEffectType.Hidden,
            });
            effects.Add("SMN_IfritCharges", new EffectDef()
            {
                UniqueID = "SMN_IfritCharges",
                DisplayName = "Ifrit Aether Charged",
                UsesStacks = true
            });
            effects.Add("SMN_GarudaCharges", new EffectDef()
            {
                UniqueID = "SMN_GarudaCharges",
                DisplayName = "Garuda Aether Charged",
                UsesStacks = true
            });
            effects.Add("SMN_TitanCharges", new EffectDef()
            {
                UniqueID = "SMN_TitanCharges",
                DisplayName = "Titan Aether Charged",
                UsesStacks = true
            });
            effects.Add("SMN_Ruination", new EffectDef()
            {
                UniqueID = "SMN_Ruination",
                DisplayName = "Ruination",
            });
            effects.Add("SMN_Aetherflow", new EffectDef()
            {
                UniqueID = "SMN_Aetherflow",
                DisplayName = "Aetherflow",
                UsesStacks = true
            }); ;
            #endregion

            #region Ninja
            effects.Add("NIN_TrickAttack", new EffectDef()
            {
                UniqueID = "NIN_TrickAttack",
                DisplayName = "Trick Attack",
                Type = EEffectType.Debuff,
            });
            #endregion

            #region Machinist
            effects.Add("MCH_SlugShotReady", new EffectDef()
            {
                UniqueID= "MCH_SlugShotReady",
                DisplayName="(Slug Shot Ready)",
                Type=EEffectType.Combo
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
                Type = EEffectType.Combo,
                IconName = "mch_wildfire.png"
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
                DisplayName = "Heat",
                Type = EEffectType.Resource,
            });
            effects.Add("MCH_Reassemble", new EffectDef()
            {
                UniqueID = "MCH_Reassemble",
                DisplayName = "Reassemble",
                Type = EEffectType.Buff,
            });
            #endregion
        }
    }
}
