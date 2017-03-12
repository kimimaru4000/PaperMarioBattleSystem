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
            public static void NewInteractionUT1()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 5, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);
            }

            public static void NewInteractionUT2()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                SpikedGoomba spikedGoomba = new SpikedGoomba();

                InteractionParamHolder param = new InteractionParamHolder(mario, spikedGoomba, 5, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);
            }

            public static void NewInteractionUT3()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                SpikedGoomba spikedGoomba = new SpikedGoomba();
                spikedGoomba.EntityProperties.AfflictStatus(new ElectrifiedStatus(5));

                InteractionParamHolder param = new InteractionParamHolder(mario, spikedGoomba, 5, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);
            }

            public static void NewInteractionUT4()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();
                goomba.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
                goomba.EntityProperties.AfflictStatus(new ElectrifiedStatus(5));

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 5, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);
            }

            public static void NewInteractionUT5()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();
                goomba.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
                goomba.EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Half, Enumerations.Elements.Poison, new PoisonStatus(5)));

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 10, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);
            }

            public static void NewInteractionUT6()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();
                mario.EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Full, Enumerations.Elements.Star));
                goomba.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
                goomba.EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Half, Enumerations.Elements.Poison, new PoisonStatus(5)));

                Badge dd1 = Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DamageDodge, BadgeGlobals.BadgeFilterType.UnEquipped);
                dd1?.Equip(mario);
                Badge dd2 = Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DamageDodge, BadgeGlobals.BadgeFilterType.UnEquipped);
                dd2?.Equip(mario);

                //For defensive actions; add flags in their code to make them always succeed
                //We'll need to implement the debug badge that automatically completes action commands as well as
                //make it easier to start input for action commands for debugging
                mario.OnTurnEnd();
                mario.Update();

                InteractionParamHolder param = new InteractionParamHolder(goomba, mario, 10, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);

                if (BattleManager.Instance.EntityTurn.PreviousAction?.MoveSequence.InSequence == false)
                    BattleManager.Instance.EntityTurn.OnTurnStart();

                dd1?.UnEquip();
                dd2?.UnEquip();
            }

            public static void NewInteractionUT7()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 50, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();
                goomba.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Fiery);
                goomba.EntityProperties.AddWeakness(Enumerations.Elements.Ice, new WeaknessHolder(WeaknessTypes.PlusDamage, 2));

                Badge badge = Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.IcePower, BadgeGlobals.BadgeFilterType.UnEquipped);
                badge?.Equip(mario);

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 3, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);

                badge?.UnEquip();
            }

            public static void NewInteractionUT8()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();
                mario.EntityProperties.AddWeakness(Enumerations.Elements.Normal, new WeaknessHolder(WeaknessTypes.KO, 4));
                goomba.EntityProperties.AfflictStatus(new PaybackStatus(5));

                Badge badge = Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.DoublePain, BadgeGlobals.BadgeFilterType.UnEquipped);
                goomba.SetHeldCollectible(badge);
                goomba.OnBattleStart();

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 4, Enumerations.Elements.Water, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);

                badge?.UnEquip();
                goomba.SetHeldCollectible(null);
            }

            public static void NewInteractionUT9()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();

                //Update HealthState for Last Stand to kick in on Danger
                mario.TakeDamage(Enumerations.Elements.Normal, 0, false);

                Badge badge = Inventory.Instance.GetBadge(BadgeGlobals.BadgeTypes.LastStand, BadgeGlobals.BadgeFilterType.UnEquipped);
                badge?.Equip(mario);

                InteractionParamHolder param = new InteractionParamHolder(goomba, mario, 80, Enumerations.Elements.Water, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);

                badge?.UnEquip();
            }

            public static void NewInteractionUT10()
            {
                BattleMario mario = new BattleMario(new MarioStats(1, 5, 50, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal));
                Goomba goomba = new Goomba();

                mario.EntityProperties.AddResistance(Enumerations.Elements.Electric, new ResistanceHolder(ResistanceTypes.Heal, 0));

                goomba.EntityProperties.AddPayback(new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Half, Enumerations.Elements.Electric));
                goomba.EntityProperties.AfflictStatus(new ElectrifiedStatus(5));

                InteractionParamHolder param = new InteractionParamHolder(mario, goomba, 90, Enumerations.Elements.Normal, false,
                    Enumerations.ContactTypes.Direct, null);

                InteractionResult oldInteraction = Interactions.GetDamageInteraction(param);
                InteractionResult newInteraction = Interactions.GetDamageInteractionNew(param);

                Debug.Log("Old: ");
                PrintInteractionResult(oldInteraction);
                Debug.Log("New: ");
                PrintInteractionResult(newInteraction);
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
                        statuses += interactionHolder.StatusesInflicted[i].StatusType.ToString() + " ";
                }

                Debug.Log($"{startString}: {interactionHolder.Entity?.Name}\n" +
                          $"{startString} Damage: {interactionHolder.TotalDamage}\n" +
                          $"{startString} Element: {interactionHolder.DamageElement}\n" +
                          $"{startString} Element Result: {interactionHolder.ElementResult}\n" +
                          $"{startString} Piercing: {interactionHolder.Piercing}\n" +
                          $"{startString} Statuses: {statuses}\n" +
                          $"{startString} Hit: {interactionHolder.Hit}");
            }
        }
    }
}
