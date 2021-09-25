using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public static class ActionBank
    {
        public static int GCD_150 = 150;
        public static int BASE_GCD = 250;

        /// <summary>
        /// Key: Class abbreviation string.
        /// Value: List of all actions defined for that class.
        /// </summary>
        public static readonly Dictionary<string, List<ActionDef>> actionSets;

        /// <summary>
        /// Key: UniqueID for an action.
        /// Value: The full ActionDef.
        /// </summary>
        public static readonly Dictionary<string, ActionDef> actions;

        static ActionBank()
        {
            actionSets = new Dictionary<string, List<ActionDef>>();
            actions = new Dictionary<string, ActionDef>();

            #region Summoner
            List<ActionDef> summonerActionSet = new List<ActionDef>();
            actionSets.Add("SMN", summonerActionSet);

            ActionDef SMN_SummonBahamut = new ActionDef()
            {
                UniqueID = "SMN_SummonBahamut",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 6000,
                RecastScales = true,
                Potency = 700,
                DisplayName = "Summon Bahamut",
                IconName = "smn_summon_phoenix.png",
                AppliedEffects = new List<EffectApplication>
                {
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"],
                        Duration = 1500,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_PhoenixAvailable"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_IfritGem"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_GarudaGem"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_TitanGem"],
                        Duration = int.MaxValue,
                    }
                },
                RequiredAbsentEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixAvailable"],
                    }
                }
            };
            summonerActionSet.Add(SMN_SummonBahamut);
            actions.Add(SMN_SummonBahamut.UniqueID, SMN_SummonBahamut);

            ActionDef SMN_BahamutFiller = new ActionDef()
            {
                UniqueID = "SMN_BahamutFiller",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 400,
                DisplayName = "(Bahamut Filler)",
                RequiredEffects = new List<EffectRequirement>() {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                }
            };
            summonerActionSet.Add(SMN_BahamutFiller);
            actions.Add(SMN_BahamutFiller.UniqueID, SMN_BahamutFiller);

            ActionDef SMN_Ruin3 = new ActionDef()
            {
                UniqueID = "SMN_Ruin3",
                IsGCD = true,
                IsSpell = true,
                CastTime = GCD_150,
                RecastGCD = BASE_GCD,
                Potency = 300,
                DisplayName = "Ruin 3"
            };
            summonerActionSet.Add(SMN_Ruin3);
            actions.Add(SMN_Ruin3.UniqueID, SMN_Ruin3);

            ActionDef SMN_Ruin4 = new ActionDef()
            {
                UniqueID = "SMN_Ruin4",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 430,
                DisplayName = "Ruin 4",
                RequiredEffects = new List<EffectRequirement>
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_Ruination"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_Ruination", 1)
                }
            };
            summonerActionSet.Add(SMN_Ruin4);
            actions.Add(SMN_Ruin4.UniqueID, SMN_Ruin4);

            ActionDef SMN_BahamutEnkindle = new ActionDef()
            {
                UniqueID = "SMN_BahamutEnkindle",
                IsGCD = false,
                Potency = 600,
                DisplayName = "Enkindle (Akh Morn)",
                Recast = 800,
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                }
            };
            summonerActionSet.Add(SMN_BahamutEnkindle);
            actions.Add(SMN_BahamutEnkindle.UniqueID, SMN_BahamutEnkindle);

            ActionDef SMN_EnergyDrain = new ActionDef()
            {
                UniqueID = "SMN_EnergyDrain",
                IsGCD = false,
                Potency = 100,
                DisplayName = "Energy Drain",
                Recast = 6000,
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_Ruination"],
                        Duration=6000,
                    }
                }
            };
            summonerActionSet.Add(SMN_EnergyDrain);
            actions.Add(SMN_EnergyDrain.UniqueID, SMN_EnergyDrain);

            ActionDef SMN_Fester = new ActionDef()
            {
                UniqueID = "SMN_Fester",
                IsGCD = false,
                Potency = 300,
                DisplayName = "Fester",
                Recast = 300,
            };
            summonerActionSet.Add(SMN_Fester);
            actions.Add(SMN_Fester.UniqueID, SMN_Fester);

            ActionDef SMN_Painflare = new ActionDef()
            {
                UniqueID = "SMN_Painflare",
                IsGCD = false,
                Potency = 130,
                DisplayName = "Painflare",
                Recast = 300,
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_Ruination"],
                        Duration=6000,
                    }
                }
            };
            summonerActionSet.Add(SMN_Painflare);
            actions.Add(SMN_Painflare.UniqueID, SMN_Painflare);

            ActionDef SMN_Deathflare = new ActionDef()
            {
                UniqueID = "SMN_Deathflare",
                IsGCD = false,
                Potency = 500,
                DisplayName = "Deathflare",
                Recast = 2000,
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                }
            };
            summonerActionSet.Add(SMN_Deathflare);
            actions.Add(SMN_Deathflare.UniqueID, SMN_Deathflare);

            ActionDef SMN_SummonIfrit = new ActionDef()
            {
                UniqueID = "SMN_SummonIfrit",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 700,
                DisplayName = "Summon Ifrit"
            };
            summonerActionSet.Add(SMN_SummonIfrit);
            actions.Add(SMN_SummonIfrit.UniqueID, SMN_SummonIfrit);

            ActionDef SMN_IfritEA1 = new ActionDef()
            {
                UniqueID = "SMN_IfritEA1",
                IsGCD = true,
                IsSpell = true,
                CastTime = BASE_GCD,
                RecastGCD = BASE_GCD,
                Potency = 400,
                DisplayName = "(Ifrit EA1)"
            };
            summonerActionSet.Add(SMN_IfritEA1);
            actions.Add(SMN_IfritEA1.UniqueID, SMN_IfritEA1);

            ActionDef SMN_IfritEA2 = new ActionDef()
            {
                UniqueID = "SMN_IfritEA2",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 400,
                DisplayName = "(Ifrit EA2)"
            };
            summonerActionSet.Add(SMN_IfritEA2);
            actions.Add(SMN_IfritEA2.UniqueID, SMN_IfritEA2);

            ActionDef SMN_SummonGaruda = new ActionDef()
            {
                UniqueID = "SMN_SummonGaruda",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 700,
                DisplayName = "Summon Garuda"
            };
            summonerActionSet.Add(SMN_SummonGaruda);
            actions.Add(SMN_SummonGaruda.UniqueID, SMN_SummonGaruda);

            ActionDef SMN_GarudaEA1 = new ActionDef()
            {
                UniqueID = "SMN_GarudaEA1",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = GCD_150,
                Potency = 300,
                DisplayName = "(Garuda EA1)"
            };
            summonerActionSet.Add(SMN_GarudaEA1);
            actions.Add(SMN_GarudaEA1.UniqueID, SMN_GarudaEA1);

            ActionDef SMN_GarudaEA2 = new ActionDef()
            {
                UniqueID = "SMN_GarudaEA2",
                IsGCD = true,
                IsSpell = true,
                CastTime = BASE_GCD,
                RecastGCD = BASE_GCD,
                Potency = 400,
                DisplayName = "(Garuda EA2)"
            };
            summonerActionSet.Add(SMN_GarudaEA2);
            actions.Add(SMN_GarudaEA2.UniqueID, SMN_GarudaEA2);

            ActionDef SMN_SummonTitan = new ActionDef()
            {
                UniqueID = "SMN_SummonTitan",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 700,
                DisplayName = "Summon Titan"
            };
            summonerActionSet.Add(SMN_SummonTitan);
            actions.Add(SMN_SummonTitan.UniqueID, SMN_SummonTitan);

            ActionDef SMN_TitanEA1 = new ActionDef()
            {
                UniqueID = "SMN_TitanEA1",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 330,
                DisplayName = "(Titan EA1)"
            };
            summonerActionSet.Add(SMN_TitanEA1);
            actions.Add(SMN_TitanEA1.UniqueID, SMN_TitanEA1);

            ActionDef SMN_TitanEA2 = new ActionDef()
            {
                UniqueID = "SMN_TitanEA2",
                IsGCD = false,
                Potency = 150,
                DisplayName = "(Titan EA2)"
            };
            summonerActionSet.Add(SMN_TitanEA2);
            actions.Add(SMN_TitanEA2.UniqueID, SMN_TitanEA2);

            ActionDef SMN_Devotion = new ActionDef()
            {
                UniqueID = "SMN_Devotion",
                IsGCD = false,
                DisplayName = "(Devotion)",
                Recast = 12000,
                AppliedEffects = new List<EffectApplication>() 
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_Devotion"],
                        Duration = 3000
                    }
                }
            };
            summonerActionSet.Add(SMN_Devotion);
            actions.Add(SMN_Devotion.UniqueID, SMN_Devotion);

            #endregion
        }

        
    }
}
