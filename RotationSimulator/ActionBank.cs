using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationSimulator
{
    public enum States
    {
        SMN_PhoenixAvailable,
        SMN_Ruination,
        SMN_IfritGem,
        SMN_TitanGem,
        SMN_GarudaGem,
        SMN_Aetherflow
    }

    public static class ActionBank
    {
        public static int GCD_150 = 150;
        public static int BASE_GCD = 248;

        public static readonly RotationStep SMN_SummonBahamut;
        public static readonly RotationStep SMN_BahamutFiller;
        public static readonly RotationStep SMN_BahamutEnkindle;
        public static readonly RotationStep SMN_Ruin3;
        public static readonly RotationStep SMN_Ruin4;
        public static readonly RotationStep SMN_EnergyDrain;
        public static readonly RotationStep SMN_Fester;
        public static readonly RotationStep SMN_Painflare;
        public static readonly RotationStep SMN_Deathflare;
        public static readonly RotationStep SMN_SummonIfrit;
        public static readonly RotationStep SMN_IfritEA1;
        public static readonly RotationStep SMN_IfritEA2;
        public static readonly RotationStep SMN_SummonGaruda;
        public static readonly RotationStep SMN_GarudaEA1;
        public static readonly RotationStep SMN_GarudaEA2;
        public static readonly RotationStep SMN_SummonTitan;
        public static readonly RotationStep SMN_TitanEA1;
        public static readonly RotationStep SMN_TitanEA2;
        public static readonly RotationStep SMN_Devotion;

        public static readonly Dictionary<string, List<RotationStep>> actionSets;

        /// <summary>
        /// Wait for the listed number of centiseconds, doing nothing. Only needed if deliberately inserting downtime (e.g. when setting up an opener).
        /// 
        /// Keep in mind that this wait time does not start until all active cast times and animation locks have ended.
        /// </summary>
        /// <param name="wait"></param>
        /// <returns></returns>
        public static RotationStep CreateWaitAction(int wait) {
            return new RotationStep()
            {
                castTime = wait,
                recastGCD = 0,
                recast = 0,
                DisplayName = "(player has chosen to wait)",

            };
        }

        static ActionBank()
        {
            actionSets = new Dictionary<string, List<RotationStep>>();

            #region Summoner
            List<RotationStep> summonerActionSet = new List<RotationStep>();
            actionSets.Add("SMN", summonerActionSet);

            SMN_SummonBahamut = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 700,
                DisplayName = "Summon Bahamut",
                IconName = "smn_summon_phoenix.png"
            };
            EffectApplication summonBahamutEffectApplication = new EffectApplication
            {
                type = EActiveEffect.SMN_BahamutSummoned,
                Duration = 1500,
                Stacks = 1,
                DisplayName = "Bahamut Summoned"
            };
            EffectApplication phoenixAvailableApplication = new EffectApplication()
            {
                type = EActiveEffect.SMN_PhoenixAvailable,
                Duration = int.MaxValue,
                Stacks = 1,
                DisplayName = "(Phoenix Available)"
            };
            SMN_SummonBahamut.appliedEffects.Add(summonBahamutEffectApplication);
            summonerActionSet.Add(SMN_SummonBahamut);

            SMN_BahamutFiller = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 400,
                DisplayName = "(Bahamut Filler)"
            };
            ActiveEffect bahamutSummonedActiveEffect = new ActiveEffect()
            {
                type = EActiveEffect.SMN_BahamutSummoned,
                Stacks = 0,
            };
            SMN_BahamutFiller.requiredEffects.Add(bahamutSummonedActiveEffect);
            summonerActionSet.Add(SMN_BahamutFiller);

            SMN_Ruin3 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = GCD_150,
                recastGCD = BASE_GCD,
                potency = 300,
                DisplayName = "Ruin 3"
            };
            summonerActionSet.Add(SMN_Ruin3);

            SMN_Ruin4 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 430,
                DisplayName = "Ruin 4"
            };
            ActiveEffect ruinationActiveEffect = new ActiveEffect()
            {
                type = EActiveEffect.SMN_Ruination
            };
            SMN_Ruin4.requiredEffects.Add(ruinationActiveEffect);
            SMN_Ruin4.removeEffectStacks.Add(new Tuple<EActiveEffect, int>(EActiveEffect.SMN_Ruination, 1));
            summonerActionSet.Add(SMN_Ruin4);

            SMN_BahamutEnkindle = new RotationStep()
            {
                IsGCD = false,
                potency = 600,
                DisplayName = "Enkindle (Akh Morn)",
                recast = 800
            };
            SMN_BahamutEnkindle.requiredEffects.Add(bahamutSummonedActiveEffect);
            summonerActionSet.Add(SMN_BahamutEnkindle);

            SMN_EnergyDrain = new RotationStep()
            {
                IsGCD = false,
                potency = 100,
                DisplayName = "Energy Drain",
                recast = 6000
            };
            EffectApplication ruinationApplication = new EffectApplication()
            {
                DisplayName = "Ruination",
                Duration = 6000,
                Stacks = 1,
                type = EActiveEffect.SMN_Ruination
            };
            SMN_EnergyDrain.appliedEffects.Add(ruinationApplication);
            summonerActionSet.Add(SMN_EnergyDrain);

            SMN_Fester = new RotationStep()
            {
                IsGCD = false,
                potency = 300,
                DisplayName = "Fester",
                recast = 300,
            };
            summonerActionSet.Add(SMN_Fester);

            SMN_Painflare = new RotationStep()
            {
                IsGCD = false,
                potency = 130,
                DisplayName = "Painflare",
                recast = 300,
            };
            SMN_Painflare.appliedEffects.Add(ruinationApplication);
            summonerActionSet.Add(SMN_Painflare);

            SMN_Deathflare = new RotationStep()
            {
                IsGCD = false,
                potency = 500,
                DisplayName = "Deathflare",
                recast = 2000
            };
            SMN_Deathflare.requiredEffects.Add(bahamutSummonedActiveEffect);
            summonerActionSet.Add(SMN_Deathflare);

            SMN_SummonIfrit = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 700,
                DisplayName = "Summon Ifrit"
            };
            summonerActionSet.Add(SMN_SummonIfrit);

            SMN_IfritEA1 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = BASE_GCD,
                recastGCD = BASE_GCD,
                potency = 400,
                DisplayName = "(Ifrit EA1)"
            };
            summonerActionSet.Add(SMN_IfritEA1);

            SMN_IfritEA2 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 400,
                DisplayName = "(Ifrit EA2)"
            };
            summonerActionSet.Add(SMN_IfritEA2);

            SMN_SummonGaruda = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 700,
                DisplayName = "Summon Garuda"
            };
            summonerActionSet.Add(SMN_SummonGaruda);

            SMN_GarudaEA1 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = GCD_150,
                potency = 300,
                DisplayName = "(Garuda EA1)"
            };
            summonerActionSet.Add(SMN_GarudaEA1);

            SMN_GarudaEA2 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = BASE_GCD,
                recastGCD = BASE_GCD,
                potency = 400,
                DisplayName = "(Garuda EA2)"
            };
            summonerActionSet.Add(SMN_GarudaEA2);

            SMN_SummonTitan = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 700,
                DisplayName = "Summon Titan"
            };
            summonerActionSet.Add(SMN_SummonTitan);

            SMN_TitanEA1 = new RotationStep()
            {
                IsGCD = true,
                IsSpell = true,
                castTime = 0,
                recastGCD = BASE_GCD,
                potency = 330,
                DisplayName = "(Titan EA1)"
            };
            summonerActionSet.Add(SMN_TitanEA1);

            SMN_TitanEA2 = new RotationStep()
            {
                IsGCD = false,
                potency = 150,
                DisplayName = "(Titan EA2)"
            };
            summonerActionSet.Add(SMN_TitanEA2);

            SMN_Devotion = new RotationStep()
            {
                IsGCD = false,
                DisplayName = "(Devotion)",
                recast = 12000
            };
            EffectApplication devotionEffectApplication = new EffectApplication()
            {
                type = EActiveEffect.SMN_Devotion,
                Duration = 3000,
                DisplayName = "(Devotion)"
            };
            SMN_Devotion.appliedEffects.Add(devotionEffectApplication);
            summonerActionSet.Add(SMN_Devotion);

            #endregion
        }

        
    }
}
