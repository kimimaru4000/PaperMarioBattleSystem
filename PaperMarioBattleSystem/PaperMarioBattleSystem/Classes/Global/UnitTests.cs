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
        public static void RunInteractionUnitTests()
        {
            //Interaction tests
            InteractionUnitTests.ElementOverrideInteractionUT1();
            InteractionUnitTests.PaybackInteractionUT1();
            InteractionUnitTests.PaybackInteractionUT2();
            InteractionUnitTests.PaybackInteractionUT3();
        }

        public static void RunStatusUnitTests()
        {
            StatusUnitTests.NoSkillsTestDoubleDisable();
        }

        public static void RunBadgeUnitTests()
        {
            BadgeUnitTests.TestPAndNoPEquipCount(new BattleMario(new MarioStats(0, 0, 0, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal)));
            BadgeUnitTests.TestPAndNoPEquipCount(new Goombario());
            BadgeUnitTests.TestPAndNoPEquipCount2(new BattleMario(new MarioStats(0, 0, 0, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal)));
            BadgeUnitTests.TestPAndNoPEquipCount2(new Goombario());
            BadgeUnitTests.TestBothEquipCount();
            BadgeUnitTests.TestMixedEquipCount();
        }

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

        public static class StatusUnitTests
        {
            public static void NoSkillsTestDoubleDisable()
            {
                Goomba goomba = new Goomba();
                HammermanBadge hmBadge = new HammermanBadge();

                goomba.EntityProperties.AfflictStatus(new NoSkillsStatus(Enumerations.MoveCategories.Jump, 3), false);
                Debug.Assert(goomba.EntityProperties.IsMoveCategoryDisabled(Enumerations.MoveCategories.Jump));

                hmBadge.Equip(goomba);
                Debug.Assert(goomba.EntityProperties.IsMoveCategoryDisabled(Enumerations.MoveCategories.Jump));

                goomba.EntityProperties.RemoveStatus(Enumerations.StatusTypes.NoSkills, false);
                Debug.Assert(goomba.EntityProperties.IsMoveCategoryDisabled(Enumerations.MoveCategories.Jump));

                hmBadge.UnEquip();
                Debug.Assert(goomba.EntityProperties.IsMoveCategoryDisabled(Enumerations.MoveCategories.Jump) == false);
            }
        }

        public static class BadgeUnitTests
        {
            public static void TestPAndNoPEquipCount(BattlePlayer player)
            {
                CloseCallBadge CC = new CloseCallBadge();
                CloseCallBadge CC2 = new CloseCallBadge();
                CloseCallBadge CC3 = new CloseCallBadge();
                CloseCallBadge CC4 = new CloseCallBadge();
                CloseCallPBadge CCP = new CloseCallPBadge();

                CC.Equip(player);
                CC2.Equip(player);
                CC3.Equip(player);
                CC4.Equip(player);
                CCP.Equip(player);

                Debug.Assert(player.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.CloseCall) == player.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.CloseCallP));

                CC.UnEquip();
                CC2.UnEquip();
                CC3.UnEquip();
                CC4.UnEquip();
                CCP.UnEquip();

                Debug.Log("\n");
            }

            public static void TestPAndNoPEquipCount2(BattlePlayer player)
            {
                CloseCallPBadge CCP1 = new CloseCallPBadge();
                CloseCallPBadge CCP2 = new CloseCallPBadge();
                CloseCallPBadge CCP3 = new CloseCallPBadge();
                CloseCallPBadge CCP4 = new CloseCallPBadge();
                CloseCallBadge CC = new CloseCallBadge();

                CCP1.Equip(player);
                CCP2.Equip(player);
                CCP3.Equip(player);
                CCP4.Equip(player);
                CC.Equip(player);

                Debug.Assert(player.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.CloseCall) == player.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.CloseCallP));

                CCP1.UnEquip();
                CCP2.UnEquip();
                CCP3.UnEquip();
                CCP4.UnEquip();
                CC.UnEquip();

                Debug.Log("\n");
            }

            public static void TestBothEquipCount()
            {
                BattleMario mario = new BattleMario(new MarioStats(0, 0, 0, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goombario goombario = new Goombario();

                QuickChangeBadge qc = new QuickChangeBadge();
                TimingTutorBadge tt = new TimingTutorBadge();
                qc.Equip(mario);
                tt.Equip(goombario);

                Debug.Assert(mario.GetEquippedBadgeCount(qc.BadgeType) == goombario.GetEquippedBadgeCount(qc.BadgeType));
                Debug.Assert(mario.GetEquippedBadgeCount(tt.BadgeType) == goombario.GetEquippedBadgeCount(tt.BadgeType));

                qc.UnEquip();
                tt.UnEquip();

                Debug.Log("\n");

                qc.Equip(goombario);
                tt.Equip(mario);

                Debug.Assert(mario.GetEquippedBadgeCount(qc.BadgeType) == goombario.GetEquippedBadgeCount(qc.BadgeType));
                Debug.Assert(mario.GetEquippedBadgeCount(tt.BadgeType) == goombario.GetEquippedBadgeCount(tt.BadgeType));

                qc.UnEquip();
                tt.UnEquip();

                Debug.Log("\n");
            }

            public static void TestMixedEquipCount()
            {
                BattleMario mario = new BattleMario(new MarioStats(0, 0, 0, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goombario goombario = new Goombario();

                QuickChangeBadge qc = new QuickChangeBadge();
                RightOnBadge ro = new RightOnBadge();
                qc.Equip(mario);
                ro.Equip(goombario);

                Debug.Assert(mario.GetEquippedBadgeCount(qc.BadgeType) == goombario.GetEquippedBadgeCount(qc.BadgeType));
                Debug.Assert(mario.GetEquippedBadgeCount(ro.BadgeType) != goombario.GetEquippedBadgeCount(ro.BadgeType));

                qc.UnEquip();
                ro.UnEquip();

                Debug.Log("\n");
            }
        }
    }
}
