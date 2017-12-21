using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A class for conducting unit tests.
    /// </summary>
    public static class UnitTests
    {
        public static class InteractionUnitTests
        {
            public static void ElementOverrideInteractionUT1()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();

                IcePowerBadge icePower = new IcePowerBadge();
                icePower.Equip(mario);
                IcePowerBadge icePower2 = new IcePowerBadge();
                icePower2.Equip(mario);

                goomba.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Fiery);
                goomba.EntityProperties.AddWeakness(Enumerations.Elements.Ice, new WeaknessHolder(WeaknessTypes.PlusDamage, 1));

                Debug.Assert(goomba.EntityProperties.HasPhysAttributes(true, Enumerations.PhysicalAttributes.Fiery));
                Debug.Assert(goomba.EntityProperties.HasWeakness(Enumerations.Elements.Ice));

                ElementOverrideHolder overrideHolder = mario.EntityProperties.GetTotalElementOverride(goomba);

                Debug.Assert(overrideHolder.Element == Enumerations.Elements.Ice);
                Debug.Assert(overrideHolder.OverrideCount == 2);

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 1, Enumerations.Elements.Ice, true,
                    Enumerations.ContactTypes.TopDirect, Enumerations.ContactProperties.None, null, Enumerations.DamageEffects.None, false, Enumerations.DefensiveActionTypes.None);
                InteractionResult interaction = Interactions.GetDamageInteraction(param);

                Debug.Assert(interaction.VictimResult.TotalDamage == 4);

                PrintInteractionResult(interaction);

                icePower.UnEquip();
                icePower2.UnEquip();

                ElementOverrideHolder overrideHolder2 = mario.EntityProperties.GetTotalElementOverride(goomba);
                Debug.Assert(overrideHolder2.Element == Enumerations.Elements.Invalid);
            }

            public static void PaybackInteractionUT1()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();
                
                mario.EntityProperties.AfflictStatus(new HoldFastStatus(3), false);

                Debug.Assert(mario.EntityProperties.HasPayback());

                InteractionParamHolder paramHolder = new InteractionParamHolder(goomba, mario, 0, Enumerations.Elements.Normal, true,
                    Enumerations.ContactTypes.Latch, Enumerations.ContactProperties.None, null, Enumerations.DamageEffects.None, false,
                    Enumerations.DefensiveActionTypes.Guard | Enumerations.DefensiveActionTypes.Superguard);

                InteractionResult interaction = Interactions.GetDamageInteraction(paramHolder);

                PrintInteractionResult(interaction);

                Debug.Assert(interaction.AttackerResult.TotalDamage == 1);
                Debug.Assert(interaction.VictimResult.TotalDamage == 0);
            }

            public static void PaybackInteractionUT2()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Super, EquipmentGlobals.HammerLevels.Normal));
                KoopaTroopa koopa = new KoopaTroopa();
                koopa.RaiseAttack(2);

                ReturnPostageBadge returnPostage = new ReturnPostageBadge();
                ZapTapBadge zapTap = new ZapTapBadge();
                returnPostage.Equip(mario);
                zapTap.Equip(mario);

                Debug.Assert(mario.EntityProperties.HasPayback());
                Debug.Assert(mario.EntityProperties.HasPhysAttributes(true, Enumerations.PhysicalAttributes.Electrified));

                int damage = new ShellToss().DamageProperties.Damage + koopa.BattleStats.TotalAttack;

                InteractionParamHolder paramHolder = new InteractionParamHolder(koopa, mario, damage, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.SideDirect, Enumerations.ContactProperties.Protected, null, Enumerations.DamageEffects.None, false,
                    Enumerations.DefensiveActionTypes.None);

                InteractionResult interaction = Interactions.GetDamageInteraction(paramHolder);

                PrintInteractionResult(interaction);

                returnPostage.UnEquip();
                zapTap.UnEquip();

                Debug.Assert(interaction.VictimResult.TotalDamage == 4);
                Debug.Assert(interaction.AttackerResult.TotalDamage == 2);
            }

            public static void PaybackInteractionUT3()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Super, EquipmentGlobals.HammerLevels.Normal));
                Pokey pokey = new Pokey();

                Debug.Assert(pokey.EntityProperties.HasPhysAttributes(true, Enumerations.PhysicalAttributes.Spiked));
                Debug.Assert(pokey.EntityProperties.HasPayback());

                InteractionParamHolder paramHolder = new InteractionParamHolder(mario, pokey, 3, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.SideDirect, Enumerations.ContactProperties.None, null, Enumerations.DamageEffects.None, false,
                    Enumerations.DefensiveActionTypes.None);

                InteractionResult interaction = Interactions.GetDamageInteraction(paramHolder);

                PrintInteractionResult(interaction);

                Debug.Assert(interaction.VictimResult.DontDamageEntity == true);
                Debug.Assert(interaction.AttackerResult.IsPaybackDamage == true);
            }

            private static void PrintInteractionResult(InteractionResult interactionResult)
            {
                InteractionHolder attackResult = interactionResult.AttackerResult;
                InteractionHolder victimResult = interactionResult.VictimResult;

                PrintInteractionHolder(victimResult, false);
                PrintInteractionHolder(attackResult, true);
            }

            private static void PrintInteractionHolder(InteractionHolder interactionHolder, bool attacker)
            {
                string startString = attacker == true ? "Attacker" : "Victim";

                string statuses = string.Empty;
                if (interactionHolder.StatusesInflicted != null)
                {
                    for (int i = 0; i < interactionHolder.StatusesInflicted.Length; i++)
                    {
                        StatusChanceHolder statusHolder = interactionHolder.StatusesInflicted[i];
                        statuses += $"({statusHolder.Percentage}%){statusHolder.Status.StatusType.ToString()} ";
                    }
                }
                
                Debug.Log($"{startString}: {interactionHolder.Entity?.Name}\n" +
                          $"{startString} Damage: {interactionHolder.TotalDamage}\n" +
                          $"{startString} Element: {interactionHolder.DamageElement}\n" +
                          $"{startString} Element Result: {interactionHolder.ElementResult}\n" +
                          $"{startString} Contact Type: {interactionHolder.ContactType}\n" +
                          $"{startString} Contact Property: {interactionHolder.ContactProperty}\n" +
                          $"{startString} Piercing: {interactionHolder.Piercing}\n" +
                          $"{startString} Statuses: {statuses}\n" +
                          $"{startString} Hit: {interactionHolder.Hit}\n" +
                          $"{startString} Damage Effect(s): {interactionHolder.DamageEffect}\n" +
                          $"{startString} IsPaybackDamage: {interactionHolder.IsPaybackDamage}\n" +
                          $"{startString} Don't Damage: {interactionHolder.DontDamageEntity}\n");
            }
        }
    }
}
