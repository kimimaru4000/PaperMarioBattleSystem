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
                    Enumerations.ContactTypes.TopDirect, null, Enumerations.DamageEffects.None, false, Enumerations.DefensiveActionTypes.None);
                InteractionResult interaction = Interactions.GetDamageInteraction(param);

                Debug.Assert(interaction.VictimResult.TotalDamage == 4);

                PrintInteractionResult(interaction);

                icePower.UnEquip();
                icePower2.UnEquip();

                ElementOverrideHolder overrideHolder2 = mario.EntityProperties.GetTotalElementOverride(goomba);
                Debug.Assert(overrideHolder2.Element == Enumerations.Elements.Invalid);
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
