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
        /// Key: Class abbreviation string (e.g. SMN), or role name with first letter capitalized (Caster, Healer, Tank, Melee, Ranged).
        /// Value: List of all actions defined for that class/role.
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

            #region Items

            ActionDef Item_Grade4IntTincture = new ActionDef()
            {
                UniqueID = "Item_Grade4IntTincture",
                IsGCD = false,
                DisplayName = "HQ Grade 4 Tincture of Intelligence",//TODO: Change icon
                Recast = 27000,
                AnimationLockOverride = 150,
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["Item_Grade4IntTincture"],
                        Duration=3000,
                    }
                }
            };
            actions.Add("Item_Grade4IntTincture",Item_Grade4IntTincture);
            ActionDef Item_Grade5IntTincture = new ActionDef()
            {
                UniqueID = "Item_Grade5IntTincture",
                IsGCD = false,
                DisplayName = "HQ Grade 5 Tincture of Intelligence",//TODO: Change icon
                Recast = 27000,
                AnimationLockOverride = 150,
                CooldownID = "Item_Grade4IntTincture",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["Item_Grade5IntTincture"],
                        Duration=3000,
                    }
                }
            };
            actions.Add("Item_Grade5IntTincture", Item_Grade5IntTincture);
            #endregion

            #region Role
            List<ActionDef> casterActions = new List<ActionDef>();
            actionSets.Add("Caster", casterActions);
            List<ActionDef> meleeActions = new List<ActionDef>();
            actionSets.Add("Melee", meleeActions);
            List<ActionDef> rangedActions = new List<ActionDef>();
            actionSets.Add("Ranged", rangedActions);
            List<ActionDef> healerActions = new List<ActionDef>();
            actionSets.Add("Healer", healerActions);
            List<ActionDef> tankActions = new List<ActionDef>();
            actionSets.Add("Tank", tankActions);

            ActionDef Role_Swiftcast = new ActionDef()
            {
                UniqueID = "Role_Swiftcast",
                IsGCD = false,
                DisplayName = "Swiftcast",
                IconName = "role_swiftcast.png",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["Role_Swiftcast"],
                        Duration = 1000
                    }
                }
            };
            casterActions.Add(Role_Swiftcast);
            healerActions.Add(Role_Swiftcast);
            actions.Add(Role_Swiftcast.UniqueID, Role_Swiftcast);


            casterActions.Add(Item_Grade4IntTincture);
            casterActions.Add(Item_Grade5IntTincture);
            #endregion

            #region Summoner
            List<ActionDef> summonerActionSet = new List<ActionDef>();
            actionSets.Add("SMN", summonerActionSet);
            summonerActionSet.AddRange(casterActions);

            #region Non-Summons
            //TODO: Ruin 1/2 upgrade path

            ActionDef SMN_Ruin3 = new ActionDef()
            {
                UniqueID = "SMN_Ruin3",
                IsGCD = true,
                IsSpell = true,
                CastTime = GCD_150,
                RecastGCD = BASE_GCD,
                Potency = 310,
                DisplayName = "Ruin 3",
                IconName = "smn_ruin3.png",
                RequiredAbsentEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    },
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixSummoned"]
                    }
                }
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
                IconName = "smn_ruin4.png",
                RequiredEffects = new List<EffectRequirement>
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_FurtherRuin"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_FurtherRuin", 1)
                }
            };
            summonerActionSet.Add(SMN_Ruin4);
            actions.Add(SMN_Ruin4.UniqueID, SMN_Ruin4);

            //TODO Outburst/Tridisaster

            //TODO: Aethercharge into Trance (into bahamut) upgrade path

            ActionDef SMN_EnergyDrain = new ActionDef()
            {
                UniqueID = "SMN_EnergyDrain",
                IsGCD = false,
                Potency = 200,
                DisplayName = "Energy Drain",
                IconName = "smn_energydrain.png",
                Recast = 6000,
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_FurtherRuin"],
                        Duration=6000,
                    }
                }
            };
            summonerActionSet.Add(SMN_EnergyDrain);
            actions.Add(SMN_EnergyDrain.UniqueID, SMN_EnergyDrain);

            ActionDef SMN_EnergySiphon = new ActionDef()
            {
                UniqueID = "SMN_EnergySiphon",
                IsGCD = false,
                Potency = 100,
                DisplayName = "Energy Siphon",
                IconName = "smn_energysiphon.png",
                Recast = 6000,
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_FurtherRuin"],
                        Duration=6000,
                    }
                }
            };
            summonerActionSet.Add(SMN_EnergySiphon);
            actions.Add(SMN_EnergySiphon.UniqueID, SMN_EnergySiphon);

            ActionDef SMN_Fester = new ActionDef()
            {
                UniqueID = "SMN_Fester",
                IsGCD = false,
                Potency = 300,
                DisplayName = "Fester",
                IconName = "smn_fester.png",
                Recast = 100,
            };
            summonerActionSet.Add(SMN_Fester);
            actions.Add(SMN_Fester.UniqueID, SMN_Fester);

            ActionDef SMN_Painflare = new ActionDef()
            {
                UniqueID = "SMN_Painflare",
                IsGCD = false,
                Potency = 150,
                DisplayName = "Painflare",
                IconName = "smn_painflare.png",
                Recast = 100,
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_FurtherRuin"],
                        Duration=6000,
                    }
                }
            };
            summonerActionSet.Add(SMN_Painflare);
            actions.Add(SMN_Painflare.UniqueID, SMN_Painflare);

            #endregion

            #region Bahamut
            ActionDef SMN_Pet_AkhMorn = new ActionDef()
            {
                UniqueID = "SMN_Pet_AkhMorn",
                IsGCD = false,
                Potency = 1300,
                DisplayName = "Akh Morn",
                IconName = "smn_enkindlebahamut.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                }
            };
            actions.Add(SMN_Pet_AkhMorn.UniqueID, SMN_Pet_AkhMorn);

            ActionDef SMN_Pet_Wyrmwave = new ActionDef()
            {
                UniqueID = "SMN_Pet_Wyrmwave",
                IsGCD = true,
                RecastGCD = 300,
                Potency = 150,
                DisplayName = "Wyrmwave",
                IconName = "smn_enkindlebahamut.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                }
            };
            actions.Add(SMN_Pet_Wyrmwave.UniqueID, SMN_Pet_Wyrmwave);

            RotationStep WyrmwaveStep = new RotationStep()
            {
                Type = ERotationStepType.Action,
                Parameters = new RotationStep.RotationStepParameters()
                {
                    { "action", "SMN_Pet_Wyrmwave" }
                }
            };
            List<RotationStep> bahamutRotation = new List<RotationStep>()
            {
                new RotationStep()
                {
                    Type = ERotationStepType.Wait,
                    Parameters = new RotationStep.RotationStepParameters()
                    {
                        { "time", "200" } 
                    }
                },
                WyrmwaveStep,
                WyrmwaveStep,
                WyrmwaveStep,
                WyrmwaveStep,
            };
            
            ActionDef SMN_SummonBahamut = new ActionDef()
            {
                UniqueID = "SMN_SummonBahamut",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 6000,
                RecastScalesWithSps = true,
                RecastScalesWithHaste = true,
                DisplayName = "Summon Bahamut",
                IconName = "smn_summonbahamut.png",
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
                        effect = EffectsBank.effects["SMN_FireArcanum"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_WindArcanum"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_EarthArcanum"],
                        Duration = int.MaxValue,
                    }
                },
                RequiredAbsentEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixAvailable"],
                    }
                },
                SummonedPet = new Tuple<string, List<RotationStep>>("Bahamut", bahamutRotation),
                PetExpiration = 1500
            };
            summonerActionSet.Add(SMN_SummonBahamut);
            actions.Add(SMN_SummonBahamut.UniqueID, SMN_SummonBahamut);

            ActionDef SMN_AstralImpulse = new ActionDef()
            {
                UniqueID = "SMN_AstralImpulse",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 430,
                DisplayName = "Astral Impulse",
                IconName = "smn_astralimpulse.png",
                RequiredEffects = new List<EffectRequirement>() {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                }
            };
            summonerActionSet.Add(SMN_AstralImpulse);
            actions.Add(SMN_AstralImpulse.UniqueID, SMN_AstralImpulse);

            ActionDef SMN_BahamutEnkindle = new ActionDef()
            {
                UniqueID = "SMN_BahamutEnkindle",
                IsGCD = false,
                DisplayName = "Enkindle Bahamut",
                IconName = "smn_enkindlebahamut.png",
                Recast = 2000,
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_BahamutSummoned"]
                    }
                },
                TriggersPetAction = SMN_Pet_AkhMorn
            };
            summonerActionSet.Add(SMN_BahamutEnkindle);
            actions.Add(SMN_BahamutEnkindle.UniqueID, SMN_BahamutEnkindle);

            ActionDef SMN_Deathflare = new ActionDef()
            {
                UniqueID = "SMN_Deathflare",
                IsGCD = false,
                Potency = 500,
                DisplayName = "Deathflare",
                IconName = "smn_deathflare.png",
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

            #endregion

            #region Phoenix
            ActionDef SMN_Pet_Revelation = new ActionDef()
            {
                UniqueID = "SMN_Pet_Revelation",
                IsGCD = false,
                Potency = 1300,
                DisplayName = "Revelation",
                IconName = "smn_enkindlebahamut.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixSummoned"]
                    }
                }
            };
            actions.Add(SMN_Pet_Revelation.UniqueID, SMN_Pet_Revelation);

            ActionDef SMN_Pet_ScarletFlame = new ActionDef()
            {
                UniqueID = "SMN_Pet_ScarletFlame",
                IsGCD = true,
                RecastGCD = 300,
                Potency = 150,
                DisplayName = "Scarlet Flame",
                IconName = "smn_enkindlebahamut.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixSummoned"]
                    }
                }
            };
            actions.Add(SMN_Pet_ScarletFlame.UniqueID, SMN_Pet_ScarletFlame);

            RotationStep ScarletFlameStep = new RotationStep()
            {
                Type = ERotationStepType.Action,
                Parameters = new RotationStep.RotationStepParameters()
                {
                    { "action", "SMN_Pet_ScarletFlame" }
                }
            };
            List<RotationStep> phoenixRotation = new List<RotationStep>()
            {
                new RotationStep()
                {
                    Type = ERotationStepType.Wait,
                    Parameters = new RotationStep.RotationStepParameters()
                    {
                        { "time", "200" }
                    }
                },
                ScarletFlameStep,
                ScarletFlameStep,
                ScarletFlameStep,
                ScarletFlameStep,
            };

            ActionDef SMN_SummonPhoenix = new ActionDef()
            {
                UniqueID = "SMN_SummonPhoenix",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 6000,
                RecastScalesWithSps = true,
                RecastScalesWithHaste = true,
                DisplayName = "Summon Phoenix",
                IconName = "smn_summonphoenix.png",
                CooldownID = "SMN_SummonBahamut",
                AppliedEffects = new List<EffectApplication>
                {
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_PhoenixSummoned"],
                        Duration = 1500,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_FireArcanum"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_WindArcanum"],
                        Duration = int.MaxValue,
                    },
                    new EffectApplication
                    {
                        effect = EffectsBank.effects["SMN_EarthArcanum"],
                        Duration = int.MaxValue,
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixAvailable"],
                    }
                },
                RemoveEffectStacks = new List<Tuple<string,int>>()
                {
                    new Tuple<string, int>("SMN_PhoenixAvailable", 0)
                },
                SummonedPet = new Tuple<string, List<RotationStep>>("Phoenix", phoenixRotation),
                PetExpiration = 1500
            };
            summonerActionSet.Add(SMN_SummonPhoenix);
            actions.Add(SMN_SummonPhoenix.UniqueID, SMN_SummonPhoenix);

            ActionDef SMN_FountainOfFire = new ActionDef()
            {
                UniqueID = "SMN_FountainOfFire",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 430,
                DisplayName = "Fountain of Fire",
                IconName = "smn_fountainoffire.png",
                RequiredEffects = new List<EffectRequirement>() {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixSummoned"]
                    }
                }
            };
            summonerActionSet.Add(SMN_FountainOfFire);
            actions.Add(SMN_FountainOfFire.UniqueID, SMN_FountainOfFire);

            ActionDef SMN_PhoenixEnkindle = new ActionDef()
            {
                UniqueID = "SMN_PhoenixEnkindle",
                IsGCD = false,
                DisplayName = "Enkindle Phoenix",
                IconName = "smn_enkindlephoenix.png",
                Recast = 2000,
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_PhoenixSummoned"]
                    }
                },
                TriggersPetAction = SMN_Pet_Revelation
            };
            summonerActionSet.Add(SMN_PhoenixEnkindle);
            actions.Add(SMN_PhoenixEnkindle.UniqueID, SMN_PhoenixEnkindle);

            #endregion

            #region Ifrit
            //TODO: Ruby carbuncle/ summon I upgrade path

            ActionDef SMN_SummonIfrit = new ActionDef()
            {
                UniqueID = "SMN_SummonIfrit",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 700,
                DisplayName = "Summon Ifrit II",
                IconName = "smn_summonifrit2.png",
                AppliedEffects = new List<EffectApplication>() { 
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_FireAttunement"],
                        Duration = 3000,
                        Stacks = 2
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_IfritsFavor"],
                        Duration = 3000,
                        Stacks = 1
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_FireArcanum"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_FireArcanum", 1)
                }
            };
            summonerActionSet.Add(SMN_SummonIfrit);
            actions.Add(SMN_SummonIfrit.UniqueID, SMN_SummonIfrit);

            ActionDef SMN_RubyRite = new ActionDef()
            {
                UniqueID = "SMN_RubyRite",
                IsGCD = true,
                IsSpell = true,
                CastTime = 280,
                RecastGCD = BASE_GCD,
                Potency = 430,
                DisplayName = "Ruby Rite",
                IconName = "smn_rubyrite.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_FireAttunement"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_FireAttunement", 1)
                }
            };
            summonerActionSet.Add(SMN_RubyRite);
            actions.Add(SMN_RubyRite.UniqueID, SMN_RubyRite);

            ActionDef SMN_CrimsonCyclone = new ActionDef()
            {
                UniqueID = "SMN_CrimsonCyclone",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 430,
                DisplayName = "Crimson Cyclone",
                IconName = "smn_crimsoncyclone.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_CrimsonStrikeReady"],
                        Duration = 300
                    }
                },
                RequiredAbsentEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_CrimsonStrikeReady"]
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_IfritsFavor"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_IfritsFavor", 1)
                }
            };
            summonerActionSet.Add(SMN_CrimsonCyclone);
            actions.Add(SMN_CrimsonCyclone.UniqueID, SMN_CrimsonCyclone);

            ActionDef SMN_CrimsonStrike = new ActionDef()
            {
                UniqueID = "SMN_CrimsonStrike",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 430,
                DisplayName = "Crimson Strike",
                IconName = "smn_crimsonstrike.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_CrimsonStrikeReady"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_CrimsonStrikeReady", 1)
                }
            };
            summonerActionSet.Add(SMN_CrimsonStrike);
            actions.Add(SMN_CrimsonStrike.UniqueID, SMN_CrimsonStrike);
            #endregion

            #region Titan
            //TODO: Topaz Carbuncle / Summon I upgrade path
            ActionDef SMN_SummonTitan = new ActionDef()
            {
                UniqueID = "SMN_SummonTitan",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 700,
                DisplayName = "Summon Titan II",
                IconName = "smn_summontitan2.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_EarthAttunement"],
                        Duration = 3000,
                        Stacks = 4
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_EarthArcanum"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_EarthArcanum", 1)
                }
            };
            summonerActionSet.Add(SMN_SummonTitan);
            actions.Add(SMN_SummonTitan.UniqueID, SMN_SummonTitan);

            ActionDef SMN_TopazRite = new ActionDef()
            {
                UniqueID = "SMN_TopazRite",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 330,
                DisplayName = "Topaz Rite",
                IconName = "smn_topazrite.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_TitansFavor"],
                        Duration = 3000,
                        Stacks = 1
                    }
                }
            };
            summonerActionSet.Add(SMN_TopazRite);
            actions.Add(SMN_TopazRite.UniqueID, SMN_TopazRite);

            ActionDef SMN_MountainBuster = new ActionDef()
            {
                UniqueID = "SMN_MountainBuster",
                IsGCD = false,
                Potency = 150,
                DisplayName = "Mountain Buster",
                IconName = "smn_mountainbuster.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_TitansFavor"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_TitansFavor", 1)
                }
            };
            summonerActionSet.Add(SMN_MountainBuster);
            actions.Add(SMN_MountainBuster.UniqueID, SMN_MountainBuster);
            #endregion

            #region Garuda
            ActionDef SMN_SummonGaruda = new ActionDef()
            {
                UniqueID = "SMN_SummonGaruda",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 700,
                DisplayName = "Summon Garuda II",
                IconName = "smn_summongaruda2.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_WindAttunement"],
                        Duration = 3000,
                        Stacks = 4
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_GarudasFavor"],
                        Duration = 3000,
                        Stacks = 1
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_WindArcanum"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_WindArcanum", 1)
                }
            };
            summonerActionSet.Add(SMN_SummonGaruda);
            actions.Add(SMN_SummonGaruda.UniqueID, SMN_SummonGaruda);

            ActionDef SMN_EmeraldRite = new ActionDef()
            {
                UniqueID = "SMN_EmeraldRite",
                IsGCD = true,
                IsSpell = true,
                CastTime = 0,
                RecastGCD = GCD_150,
                Potency = 230,
                DisplayName = "Emerald Rite",
                IconName = "smn_emeraldrite.png",
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_WindAttunement"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_WindAttunement", 1)
                }
            };
            summonerActionSet.Add(SMN_EmeraldRite);
            actions.Add(SMN_EmeraldRite.UniqueID, SMN_EmeraldRite);

            ActionDef SMN_Slipstream = new ActionDef()
            {
                UniqueID = "SMN_Slipstream",
                IsGCD = true,
                IsSpell = true,
                CastTime = 300,
                RecastGCD = 350,
                Potency = 430,
                DisplayName = "Slipstream",
                IconName = "smn_slipstream.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_Slipstream"],
                        Duration = 1500
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["SMN_GarudasFavor"]
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("SMN_GarudasFavor", 1)
                }
            };
            summonerActionSet.Add(SMN_Slipstream);
            actions.Add(SMN_Slipstream.UniqueID, SMN_Slipstream);
            #endregion

            ActionDef SMN_SearingLight = new ActionDef()
            {
                UniqueID = "SMN_SearingLight",
                IsGCD = false,
                DisplayName = "Searing Light",
                IconName = "smn_searinglight.png",
                Recast = 12000,
                AppliedEffects = new List<EffectApplication>() 
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["SMN_SearingLight"],
                        Duration = 3000
                    }
                }
            };
            summonerActionSet.Add(SMN_SearingLight);
            actions.Add(SMN_SearingLight.UniqueID, SMN_SearingLight);

            #endregion

            #region Machinist
            List<ActionDef> machinistActionSet = new List<ActionDef>();
            actionSets.Add("MCH", machinistActionSet);
            machinistActionSet.AddRange(rangedActions);

            //NOTES: Things that upgrades as you level.
            //    Split Shot -> Heated Split Shot (180 potency before upgrade)
            //    Slug Shot -> Heated Slug Shot (260 potency before upgrade, 100 uncombo'd)
            //    Clean Shot -> Heated Clean Shot (340 potency before upgrade, 100 uncombo'd)
            //    Hot Shot -> Air Anchor (300 potency before upgrade)
            //    Wildfire (150 potency per GCD to 200)
            //    Rook Autoturret -> Automaton Queen (80 potency autos and 400p finisher, to 150p autos and 800p finisher)
            //      Battery also doesn't start shoting up until you get Rook, but all moves that generate battery immediately go to their full generation.
            //      Heat doesn't generate until level 30 (hypercharge). All moves generate their full heat value from that point.
            //    MCH has two level-based traits that give % damage bonus.
            // Main challenges for simming MCH
            //    * pet potency
            //    * variable turret/queen length, and their AA modeling,
            //    * wildfire potency (which also requires modeling animation delay)
            //    * AA modeling in general (though this is the least impactful thing to skip)
            //    * flamethrower (ground effect tick modeling, i.e. fast server tick)

            //NOTE: MCH is still very much just placeholder'd here. Just a few actions so I can test selection of job in the rotations UI.
            ActionDef MCH_SplitShot = new ActionDef()
            {
                UniqueID = "MCH_SplitShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 180,
                DisplayName = "Split Shot",
                IconName = "mch_splitshot.png",
                LevelBasedUpgrade = new Tuple<int, string>(54,"MCH_HeatedSplitShot"),
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_SlugShotReady"],
                        Duration = 1500
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks=5,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("MCH_CleanShotReady", 0)
                }
            };
            machinistActionSet.Add(MCH_SplitShot);
            actions.Add(MCH_SplitShot.UniqueID, MCH_SplitShot);

            ActionDef MCH_HeatedSplitShot = new ActionDef()
            {
                UniqueID = "MCH_HeatedSplitShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 220,
                DisplayName = "Heated Split Shot",
                IconName = "mch_heatedsplitshot.png",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_SlugShotReady"],
                        Duration = 1500
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks=5,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("MCH_CleanShotReady", 0)
                }
            };
            machinistActionSet.Add(MCH_HeatedSplitShot);
            actions.Add(MCH_HeatedSplitShot.UniqueID, MCH_HeatedSplitShot);

            ActionDef MCH_SlugShot = new ActionDef()
            {
                UniqueID = "MCH_SlugShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 260,
                UncomboedPotency = 100,
                DisplayName = "Slug Shot",
                IconName="mch_slugshot.png",
                LevelBasedUpgrade = new Tuple<int, string>(60, "MCH_HeatedSlugShot"),
                ComboEffectId = "MCH_SlugShotReady",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_CleanShotReady"],
                        Duration = 1500
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks=5,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                }
            };
            machinistActionSet.Add(MCH_SlugShot);
            actions.Add(MCH_SlugShot.UniqueID, MCH_SlugShot);

            ActionDef MCH_HeatedSlugShot = new ActionDef()
            {
                UniqueID = "MCH_HeatedSlugShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 330,
                UncomboedPotency = 100,
                DisplayName = "Heated Slug Shot",
                IconName = "mch_heatedslugshot.png",
                ComboEffectId = "MCH_SlugShotReady",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_CleanShotReady"],
                        Duration = 1500
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks=5,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                }
            };
            machinistActionSet.Add(MCH_HeatedSlugShot);
            actions.Add(MCH_HeatedSlugShot.UniqueID, MCH_HeatedSlugShot);

            ActionDef MCH_CleanShot = new ActionDef()
            {
                UniqueID = "MCH_CleanShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 340,
                UncomboedPotency = 100,
                DisplayName = "Clean Shot",
                IconName = "mch_cleanshot.png",
                LevelBasedUpgrade = new Tuple<int, string>(64, "MCH_HeatedCleanShot"),
                ComboEffectId = "MCH_CleanShotReady",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks=5,
                        IsAdditiveStacks=true,
                        StackMax=100
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Battery"],
                        Stacks=10,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("MCH_SlugShotReady", 0)
                }
            };
            machinistActionSet.Add(MCH_CleanShot);
            actions.Add(MCH_CleanShot.UniqueID, MCH_CleanShot);

            ActionDef MCH_HeatedCleanShot = new ActionDef()
            {
                UniqueID = "MCH_HeatedCleanShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Potency = 440,
                UncomboedPotency = 100,
                DisplayName = "Heated Clean Shot",
                IconName = "mch_heatedcleanshot.png",
                ComboEffectId = "MCH_CleanShotReady",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks=5,
                        IsAdditiveStacks=true,
                        StackMax=100
                    },
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Battery"],
                        Stacks=10,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                },
                RemoveEffectStacks = new List<Tuple<string, int>>()
                {
                    new Tuple<string, int>("MCH_SlugShotReady", 0)
                }
            };
            machinistActionSet.Add(MCH_HeatedCleanShot);
            actions.Add(MCH_HeatedCleanShot.UniqueID, MCH_HeatedCleanShot);

            ActionDef MCH_Drill = new ActionDef()
            {
                UniqueID = "MCH_Drill",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 2000,
                RecastScalesWithSks = true,
                RecastScalesWithHaste = true,
                Potency = 700,
                DisplayName = "Drill",
                IconName = "mch_drill.png"
            };
            machinistActionSet.Add(MCH_Drill);
            actions.Add(MCH_Drill.UniqueID, MCH_Drill);

            ActionDef MCH_Bioblaster = new ActionDef()
            {
                UniqueID = "MCH_Bioblaster",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 2000,
                RecastScalesWithSks = true,
                RecastScalesWithHaste = true,
                Potency = 60,
                DisplayName = "Bioblaster",
                IconName = "mch_bioblaster.png",
                CooldownID = "MCH_Drill",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        Duration = 1500,
                        effect = EffectsBank.effects["MCH_Bioblaster"]
                    }
                }
            };
            machinistActionSet.Add(MCH_Bioblaster);
            actions.Add(MCH_Bioblaster.UniqueID, MCH_Bioblaster);

            ActionDef MCH_HotShot = new ActionDef()
            {
                UniqueID = "MCH_HotShot",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 40,
                RecastScalesWithSks = true,
                RecastScalesWithHaste = true,
                Potency = 300,
                DisplayName = "Hot Shot",
                IconName = "mch_hotshot.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Battery"],
                        Stacks=20,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                },
                LevelBasedUpgrade = new Tuple<int, string>(76, "MCH_AirAnchor")
            };
            machinistActionSet.Add(MCH_HotShot);
            actions.Add(MCH_HotShot.UniqueID, MCH_HotShot);

            ActionDef MCH_AirAnchor = new ActionDef()
            {
                UniqueID = "MCH_AirAnchor",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = BASE_GCD,
                Recast = 40,
                RecastScalesWithSks = true,
                RecastScalesWithHaste = true,
                Potency = 700,
                DisplayName = "Air Anchor",
                IconName = "mch_airanchor.png",
                AppliedEffects = new List<EffectApplication>() {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Battery"],
                        Stacks=20,
                        IsAdditiveStacks=true,
                        StackMax=100
                    }
                },
            };
            machinistActionSet.Add(MCH_AirAnchor);
            actions.Add(MCH_AirAnchor.UniqueID, MCH_AirAnchor);

            ActionDef MCH_HeatBlast = new ActionDef()
            {
                UniqueID = "MCH_HeatBlast",
                IsGCD = true,
                IsWeaponskill = true,
                CastTime = 0,
                RecastGCD = GCD_150,
                Potency = 220,
                DisplayName = "Heat Blast",
                IconName = "mch_heatblast.png",
                CooldownReset = new List<Tuple<string,int>> ()
                {
                    new Tuple<string, int>("MCH_Ricochet", 1500),
                    new Tuple<string, int>("MCH_GaussRound", 1500)
                },
                RequiredEffects = new List<EffectRequirement> () 
                { 
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["MCH_Hypercharge"]
                    } 
                }
            };
            machinistActionSet.Add(MCH_HeatBlast);
            actions.Add(MCH_HeatBlast.UniqueID, MCH_HeatBlast);

            ActionDef MCH_Reassemble = new ActionDef()
            {
                UniqueID = "MCH_Reassemble",
                IsGCD = false,
                Recast = 5500,
                DisplayName = "Reassemble",
                IconName = "mch_reassemble.png",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Reassemble"],
                        Duration = 500
                    }
                }
            };
            machinistActionSet.Add(MCH_Reassemble);
            actions.Add(MCH_Reassemble.UniqueID, MCH_Reassemble);

            ActionDef MCH_Ricochet = new ActionDef()
            {
                UniqueID = "MCH_Ricochet",
                IsGCD = false,
                Recast = 3000,
                Charges = 3,
                DisplayName = "Ricochet",
                IconName = "mch_ricochet.png",
                Potency = 150,
            };
            machinistActionSet.Add(MCH_Ricochet);
            actions.Add(MCH_Ricochet.UniqueID, MCH_Ricochet);

            ActionDef MCH_GaussRound = new ActionDef()
            {
                UniqueID = "MCH_GaussRound",
                IsGCD = false,
                Recast = 3000,
                Charges = 3,
                DisplayName = "Guass Round",
                IconName = "mch_gaussround.png",
                Potency = 150,
            };
            machinistActionSet.Add(MCH_GaussRound);
            actions.Add(MCH_GaussRound.UniqueID, MCH_GaussRound);

            ActionDef MCH_BarrelStabilizer = new ActionDef()
            {
                UniqueID = "MCH_BarrelStabilizer",
                IsGCD = false,
                Recast = 12000,
                DisplayName = "Barrel Stabilizer",
                IconName = "mch_barrelstabilizer.png",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks = 50,
                        StackMax = 100,
                        IsAdditiveStacks = true,
                    }
                }
            };
            machinistActionSet.Add(MCH_BarrelStabilizer);
            actions.Add(MCH_BarrelStabilizer.UniqueID, MCH_BarrelStabilizer);

            ActionDef MCH_Hypercharge = new ActionDef()
            {
                UniqueID = "MCH_Hypercharge",
                IsGCD = false,
                Recast = 1000,
                DisplayName = "Hypercharge",
                IconName = "mch_hypercharge.png",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Hypercharge"],
                        Duration = 800
                    }
                },
                RequiredEffects = new List<EffectRequirement>()
                {
                    new EffectRequirement()
                    {
                        effect = EffectsBank.effects["MCH_Heat"],
                        Stacks = 50
                    }
                },
                RemoveEffectStacks = new List<Tuple<string,int>>()
                {
                    new Tuple<string,int>("MCH_Heat", 50)
                }
            };
            machinistActionSet.Add(MCH_Hypercharge);
            actions.Add(MCH_Hypercharge.UniqueID, MCH_Hypercharge);

            ActionDef MCH_Wildfire = new ActionDef()
            {
                UniqueID = "MCH_Wildfire",
                IsGCD = false,
                Recast = 1000,
                DisplayName = "Wildfire",
                IconName = "mch_wildfire.png",
                AppliedEffects = new List<EffectApplication>()
                {
                    new EffectApplication()
                    {
                        effect = EffectsBank.effects["MCH_Wildfire"],
                        Duration = 1000
                    }
                }
            };
            machinistActionSet.Add(MCH_Wildfire);
            actions.Add(MCH_Wildfire.UniqueID, MCH_Wildfire);

            #endregion

        }
    }
}
