using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.StatusGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Defines all types of interactions that exist among BattleEntities.
    /// <para>This includes Element-PhysicalAttribute interactions, ContactType-PhysicalAttribute interactions, and more.</para>
    /// </summary>
    public static class Interactions
    {
        #region Structs
        
        /// <summary>
        /// Holds the result and damage value of an damage interaction involving an Element
        /// </summary>
        private struct ElementDamageResultHolder
        {
            public ElementInteractionResult InteractionResult;
            public int Damage;

            public ElementDamageResultHolder(ElementInteractionResult interactionResult, int damage)
            {
                InteractionResult = interactionResult;
                Damage = damage;
            }
        }

        #endregion

        #region Fields    

        /// <summary>
        /// The table that determines the result of a particular ContactType and a particular PhysicalAttribute.
        /// <para>The default value is a Success, meaning the ContactType will deal damage.
        /// A Failure indicates a backfire (Ex. Mario jumping on a spiked enemy).
        /// A PartialSuccess indicates that the ContactType will both deal damage and backfire (Ex. Mario jumping on an Electrified enemy).
        /// </para>
        /// </summary>
        private static Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResultInfo>> ContactTable = null;

        /// <summary>
        /// The list of steps for the Damage Calculation formula.
        /// </summary>
        private static List<DamageCalcStep> DamageCalculationSteps = null;

        #endregion

        static Interactions()
        {
            InitalizeContactTable();
            InitializeDamageFormulaSteps();
        }

        #region Contact Table Initialization Methods

        private static void InitalizeContactTable()
        {
            ContactTable = new Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResultInfo>>();

            InitializeTopDirectContactTable();
            InitializeSideDirectContactTable();
        }

        //NOTE: In the actual games, if you have the Payback status, it takes priority over any PhysicalAttributes when being dealt damage
        //For example, if you have both Return Postage and Zap Tap equipped, sucking enemies like Fuzzies will be able to touch you
        //However, normal properties apply when attacking enemies (you'll be able to jump on Electrified enemies)

        private static void InitializeTopDirectContactTable()
        {
            ContactTable.Add(ContactTypes.TopDirect, new Dictionary<PhysicalAttributes, ContactResultInfo>()
            {
                { PhysicalAttributes.TopSpiked, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Sharp, 1), ContactResult.Failure, false) },
                { PhysicalAttributes.Electrified, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Electric, 1), ContactResult.PartialSuccess, true) },
                { PhysicalAttributes.Fiery, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Fire, 1), ContactResult.Failure, false) }
            });
        }

        private static void InitializeSideDirectContactTable()
        {
            ContactTable.Add(ContactTypes.SideDirect, new Dictionary<PhysicalAttributes, ContactResultInfo>()
            {
                { PhysicalAttributes.SideSpiked, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Sharp, 1), ContactResult.Failure, false) },
                { PhysicalAttributes.Electrified, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Electric, 1), ContactResult.PartialSuccess, true) },
                { PhysicalAttributes.Fiery, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Fire, 1), ContactResult.Failure, false) }
            });
        }

        #endregion

        #region Contact Table Methods

        /// <summary>
        /// Gets the result of a ContactType on a set of PhysicalAttributes
        /// </summary>
        /// <param name="attacker">The BattleEntity performing the attack</param>
        /// <param name="contactType">The ContactType performed</param>
        /// <param name="physAttributes">The set of PhysicalAttributes to test against</param>
        /// <param name="attributesToIgnore">A set of PhysicalAttributes to ignore</param>
        /// <returns>A ContactResultInfo of the interaction</returns>
        public static ContactResultInfo GetContactResult(BattleEntity attacker, ContactTypes contactType, PhysicalAttributes[] physAttributes, params PhysicalAttributes[] attributesToIgnore)
        {
            //Return the default value
            if (ContactTable.ContainsKey(contactType) == false || physAttributes == null)
            {
                Debug.LogWarning($"{nameof(physAttributes)} array is null or {nameof(ContactTable)} does not contain the ContactType {contactType}!");
                return ContactResultInfo.Default;
            }

            //Look through the attributes and find the first match
            for (int i = 0; i < physAttributes.Length; i++)
            {
                Dictionary<PhysicalAttributes, ContactResultInfo> tableForContact = ContactTable[contactType];
                PhysicalAttributes attribute = physAttributes[i];

                //If this attribute is ignored, move onto the next
                if (attributesToIgnore?.Contains(attribute) == true)
                    continue;

                if (tableForContact.ContainsKey(attribute) == true)
                {
                    ContactResultInfo contactResult = tableForContact[attribute];
                    //If the ContactResult is a Success if the entity has the same PhysicalAttribute as the one tested, set its result to Success
                    if (contactResult.SuccessIfSameAttr == true && attacker.EntityProperties.HasPhysAttributes(true, attribute) == true)
                        contactResult.ContactResult = ContactResult.Success;
                    return contactResult;
                }
            }

            return ContactResultInfo.Default;
        }

        #endregion

        #region Damage Calculation Initialization

        /* Damage Calculation Order:
         * Victim:
         * -------
         * 1. Base damage
         * 2. Get the contact result (Success, PartialSuccess, Failure)
         * 3. Check Element Overrides to change the attacker's Element damage based on the PhysicalAttributes of the victim (Ex. Ice Power Badge)
         * 4. Calculate the Victim's Weaknesses/Resistances to the Element
         * 5. Subtract or add to the damage based on the # of P-Up, D-Down and P-Down, D-Up Badges equipped
         * 6. If Guarded, subtract 1 from the damage and add the # of Damage Dodge Badges to the victim's Defense. If Superguarded, damage = 0
         * 7. If the damage dealt is not Piercing, subtract the victim's Defense from the damage
         * 8. Multiply by: (number of Double Pains equipped + 1)
         * 9. If in Danger or Peril, divide by: (number of Last Stands equipped + 1) (ceiling the damage if it's > 0)
         * 
         * 10. Clamp the damage: Min = 0, Max = 99
         * 
         * Attacker:
         * ---------
         * 1. Start with the total unscaled damage dealt to the Victim - Double Pain and Last Stand are not factored in
         * 2. Get the Payback from the Contact Result
         * 3. If the Victim performed a Superguard, override the Payback with the the Superguard's Payback
         * 4. Calculate the Attacker's Weaknesses/Resistances to the Payback Element
         * 5. Calculate the Payback damage based off the PaybackType
         * 6. If the PaybackType is Constant, set the damage dealt to the Payback's damage
         * 
         * 7. Clamp the damage: Min = 0, Max = 99
         */

        private static void InitializeDamageFormulaSteps()
        {
            DamageCalculationSteps = new List<DamageCalcStep>();

            //Victim steps
            DamageCalculationSteps.Add(new InitStep());
            DamageCalculationSteps.Add(new ContactResultStep());
            DamageCalculationSteps.Add(new ElementOverrideStep());
            DamageCalculationSteps.Add(new VictimAttackerStrengthStep());
            DamageCalculationSteps.Add(new VictimElementDamageStep());
            DamageCalculationSteps.Add(new VictimDamageReductionStep());
            DamageCalculationSteps.Add(new VictimCheckHitStep());
            DamageCalculationSteps.Add(new VictimDefensiveStep());
            DamageCalculationSteps.Add(new VictimDamageEffectStep());
            DamageCalculationSteps.Add(new VictimDoublePainStep());
            DamageCalculationSteps.Add(new VictimLastStandStep());
            DamageCalculationSteps.Add(new ClampVictimDamageStep());
            DamageCalculationSteps.Add(new VictimFilteredStatusStep());
            DamageCalculationSteps.Add(new VictimCheckInvincibleStep());
            DamageCalculationSteps.Add(new VictimCheckContactResultStep());

            //Attacker steps
            DamageCalculationSteps.Add(new AttackerPaybackDamageStep());
            DamageCalculationSteps.Add(new ClampAttackerDamageStep());
            DamageCalculationSteps.Add(new AttackerFilteredStatusStep());
            DamageCalculationSteps.Add(new AttackerCheckInvincibleStep());
            DamageCalculationSteps.Add(new AttackerCheckContactResultStep());
        }

        #endregion

        #region Interaction Methods

        /// <summary>
        /// Calculates the result of elemental damage on a BattleEntity, based on its weaknesses and resistances to that Element.
        /// </summary>
        /// <param name="victim">The BattleEntity being damaged.</param>
        /// <param name="element">The element the entity is attacked with.</param>
        /// <param name="damage">The initial damage of the attack.</param>
        /// <returns>An ElementDamageHolder stating the result and final damage dealt to this entity.</returns>
        private static ElementDamageResultHolder GetElementalDamage(BattleEntity victim, Elements element, int damage)
        {
            ElementDamageResultHolder elementDamageResult = new ElementDamageResultHolder(ElementInteractionResult.Damage, damage);

            //NOTE: If an entity is both resistant and weak to a particular element, they cancel out.
            //I decided to go with this approach because it's the simplest for this situation, which
            //doesn't seem desirable to begin with but could be interesting in its application
            WeaknessHolder weakness = victim.EntityProperties.GetWeakness(element);
            ResistanceHolder resistance = victim.EntityProperties.GetResistance(element);

            //If there's both a weakness and resistance, return
            if (weakness.WeaknessType != WeaknessTypes.None && resistance.ResistanceType != ResistanceTypes.None)
                return elementDamageResult;

            //Handle weaknesses
            if (weakness.WeaknessType == WeaknessTypes.PlusDamage)
            {
                elementDamageResult.Damage += weakness.Value;
            }
            else if (weakness.WeaknessType == WeaknessTypes.KO)
            {
                elementDamageResult.InteractionResult = ElementInteractionResult.KO;
            }

            //Handle resistances
            if (resistance.ResistanceType == ResistanceTypes.MinusDamage)
            {
                elementDamageResult.Damage -= resistance.Value;
            }
            else if (resistance.ResistanceType == ResistanceTypes.NoDamage)
            {
                elementDamageResult.Damage = BattleGlobals.MinDamage;
            }
            else if (resistance.ResistanceType == ResistanceTypes.Heal)
            {
                elementDamageResult.InteractionResult = ElementInteractionResult.Heal;
            }

            return elementDamageResult;
        }

        /// <summary>
        /// Filters an array of StatusEffects depending on whether they will be inflicted on a BattleEntity
        /// depending on the entity's status percentages and the chance of inflicting the StatusEffect.
        /// </summary>
        /// <param name="entity">The BattleEntity to attempt to afflict the StatusEffects with</param>
        /// <param name="statusesToInflict">The original array of StatusChanceHolders.</param>
        /// <returns>An array of StatusEffects that has succeeded in being inflicted on the entity</returns>
        private static StatusChanceHolder[] GetFilteredInflictedStatuses(BattleEntity entity, StatusChanceHolder[] statusesToInflict)
        {
            //Handle null
            if (statusesToInflict == null) return null;

            //Construct a new list
            List<StatusChanceHolder> filteredStatuses = new List<StatusChanceHolder>(statusesToInflict);

            //Look through the list and remove any StatusEffects that fail to be afflicted onto the entity
            for (int i = 0; i < filteredStatuses.Count; i++)
            {
                StatusChanceHolder statusChance = filteredStatuses[i];
                if (entity.EntityProperties.TryAfflictStatus(statusChance.Percentage, statusChance.Status) == false)
                {
                    Debug.Log($"Failed to inflict {statusChance.Status.StatusType} on {entity.Name} with a {statusChance.Percentage}% chance");
                    filteredStatuses.RemoveAt(i);
                    i--;
                }
            }

            return filteredStatuses.ToArray();
        }

        /// <summary>
        /// Calculates and returns the entire damage interaction between two BattleEntities.
        /// <para>This returns all the necessary information for both BattleEntities, including the total amount of damage dealt,
        /// the type of Elemental damage to deal, the Status Effects to inflict, and whether the attack successfully hit or not.</para>
        /// </summary>
        /// <param name="interactionParam">An InteractionParamHolder containing the BattleEntities interacting and data about their interaction.</param>
        /// <returns>An InteractionResult containing InteractionHolders for both the victim and the attacker.</returns>
        public static InteractionResult GetDamageInteraction(InteractionParamHolder interactionParam)
        {
            InteractionResult finalInteraction = new InteractionResult();
            ContactResultInfo contactResultInfo = new ContactResultInfo();

            for (int i = 0; i < DamageCalculationSteps.Count; i++)
            {
                finalInteraction = DamageCalculationSteps[i].Calculate(interactionParam, finalInteraction, contactResultInfo);
                contactResultInfo = DamageCalculationSteps[i].StepContactResultInfo;
            }

            return finalInteraction;
        }

        #endregion

        #region Damage Formula Step Classes

        /// <summary>
        /// The base class for steps in the total damage calculation.
        /// </summary>
        private abstract class DamageCalcStep
        {
            /// <summary>
            /// The current InteractionResult at each step.
            /// This is copied and modified each step, so all the values after each calculation are preserved.
            /// <para>This also allows us to preserve the last interaction performed.</para>
            /// </summary>
            protected InteractionResult StepResult = null;

            public ContactResultInfo StepContactResultInfo = new ContactResultInfo();

            public InteractionResult Calculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StepResult = new InteractionResult(curResult);
                StepContactResultInfo = curContactResult;
                OnCalculate(damageInfo, curResult, curContactResult);
                return StepResult;
            }
        
            protected abstract void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult);
        }

        private sealed class InitStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StepResult.AttackerResult.Entity = damageInfo.Attacker;
                StepResult.VictimResult.Entity = damageInfo.Victim;
                StepResult.VictimResult.ContactType = damageInfo.ContactType;
                StepResult.VictimResult.DamageElement = damageInfo.DamagingElement;
                StepResult.VictimResult.StatusesInflicted = damageInfo.Statuses;
                StepResult.VictimResult.TotalDamage = damageInfo.Damage;
                StepResult.VictimResult.Piercing = damageInfo.Piercing;
                StepResult.VictimResult.DamageEffect = damageInfo.DamageEffect;
            }
        }

        private sealed class ContactResultStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                BattleEntity victim = StepResult.VictimResult.Entity;
                BattleEntity attacker = StepResult.AttackerResult.Entity;

                //Get contact results
                StepContactResultInfo = victim.EntityProperties.GetContactResult(attacker, StepResult.VictimResult.ContactType);
            }
        }

        private sealed class ElementOverrideStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                Elements element = StepResult.VictimResult.DamageElement;
                BattleEntity victim = StepResult.VictimResult.Entity;

                //Retrieve an overridden type of Elemental damage to inflict based on the Victim's PhysicalAttributes
                //(Ex. The Ice Power Badge only deals Ice damage to Fiery entities)
                ElementOverrideHolder newElement = StepResult.AttackerResult.Entity.EntityProperties.GetTotalElementOverride(victim);
                if (newElement.Element != Elements.Invalid)
                {
                    /*NOTE: Idea for stacking weaknesses
                      (Ex. 1 Ice Power is 1 weakness, Ice Power + Ice Smash = 2 weaknesses)
                      
                      Ex. damage = 2
                      player inflicts ice 2 times: ice power & ice smash

                      ice_inflicted_times = 2
                      if (enemy weak to ice)
                        damage += ice_inflicted_times
                      end

                      #damage = 4
                    */

                    //Add the number of element overrides to the damage if the element used already exists as an override and the victim has a Weakness
                    //to the Element. This allows Badges such as Ice Power to deal more damage if used in conjunction with attacks
                    //that deal the same type of damage (Ex. Ice Power and Ice Smash deal 2 additional damage total rather than 1).
                    //If any new knowledge is discovered to improve this, this will be changed
                    //Ice Power is the only Badge of its kind across the first two PM games that does anything like this
                    if (element == newElement.Element && victim.EntityProperties.HasWeakness(element) == true)
                    {
                        StepResult.VictimResult.TotalDamage += newElement.OverrideCount;
                    }

                    StepResult.VictimResult.DamageElement = newElement.Element;
                }
            }
        }

        /// <summary>
        /// Factors in the Attacker's total <see cref="StrengthHolder"/> on the Victim.
        /// <para>This is done outside <see cref="GetElementalDamage"/> because it is adds directly to the damage and
        /// should not be applied again during the <see cref="AttackerPaybackDamageStep"/>.</para>
        /// </summary>
        private sealed class VictimAttackerStrengthStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StrengthHolder totalStrength = StepResult.AttackerResult.Entity.EntityProperties.GetTotalStrength(StepResult.VictimResult.Entity);
                StepResult.VictimResult.TotalDamage += totalStrength.Value;
            }
        }

        private sealed class VictimElementDamageStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                ElementDamageResultHolder victimElementDamage = GetElementalDamage(StepResult.VictimResult.Entity,
                    StepResult.VictimResult.DamageElement, StepResult.VictimResult.TotalDamage);

                StepResult.VictimResult.ElementResult = victimElementDamage.InteractionResult;
                StepResult.VictimResult.TotalDamage = victimElementDamage.Damage;
            }
        }

        private sealed class VictimDamageReductionStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StepResult.VictimResult.TotalDamage -= StepResult.VictimResult.Entity.BattleStats.DamageReduction;
            }
        }

        private sealed class VictimCheckHitStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //If the move cannot miss, hit is set to true
                if (damageInfo.CantMiss == true) StepResult.VictimResult.Hit = true;
                else StepResult.VictimResult.Hit = StepResult.AttackerResult.Entity.AttemptHitEntity(StepResult.VictimResult.Entity);
            }
        }

        /// <summary>
        /// The Victim's final UNSCALED damage is calculated in this step.
        /// </summary>
        private sealed class VictimDefensiveStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //Defense added from Damage Dodge Badges upon a successful Guard
                int damageDodgeDefense = 0;

                //Defensive actions take priority. If the attack didn't hit, don't check for defensive actions
                BattleGlobals.DefensiveActionHolder? victimDefenseData = null;
                if (StepResult.VictimResult.Hit == true)
                {
                    victimDefenseData = StepResult.VictimResult.Entity.GetDefensiveActionResult(StepResult.VictimResult.TotalDamage,
                        StepResult.VictimResult.StatusesInflicted, StepResult.VictimResult.DamageEffect, damageInfo.DefensiveOverride);
                }

                //A Defensive Action has been performed
                if (victimDefenseData.HasValue == true)
                {
                    StepResult.VictimResult.TotalDamage = victimDefenseData.Value.Damage;
                    StepResult.VictimResult.StatusesInflicted = victimDefenseData.Value.Statuses;
                    StepResult.VictimResult.DamageEffect = victimDefenseData.Value.DamageEffect;

                    //Store the damage dealt to the attacker, if any
                    if (victimDefenseData.Value.ElementHolder.HasValue == true)
                    {
                        ElementDamageHolder elementHolder = victimDefenseData.Value.ElementHolder.Value;

                        //If the Defensive action dealt damage and the contact was direct
                        //the Defensive action has caused a Failure for the Attacker (Ex. Superguarding)
                        if (StepResult.VictimResult.ContactType == ContactTypes.TopDirect
                            || StepResult.VictimResult.ContactType == ContactTypes.SideDirect)
                        {
                            StepContactResultInfo.ContactResult = ContactResult.Failure;

                            //Use the damage from the Defensive Action
                            StepResult.AttackerResult.TotalDamage = elementHolder.Damage;

                            //Update the Paybackholder to use the Payback data from the Defensive Action
                            StepContactResultInfo.Paybackholder = new PaybackHolder(PaybackTypes.Constant, elementHolder.Element, elementHolder.Damage);
                        }

                        StepResult.AttackerResult.DamageElement = victimDefenseData.Value.ElementHolder.Value.Element;
                    }

                    //Factor in the additional Guard defense for all DefensiveActions (for now, at least)
                    //If it's not Piercing, this will be subtracted, in addition to the Victim's Defense, from the damage dealt to the Victim
                    damageDodgeDefense = StepResult.VictimResult.Entity.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DamageDodge);
                }

                //Subtract Defense on non-piercing damage
                if (StepResult.VictimResult.Piercing == false)
                {
                    int totalDefense = StepResult.VictimResult.Entity.BattleStats.TotalDefense + damageDodgeDefense;
                    StepResult.VictimResult.TotalDamage -= totalDefense;
                }

                //Store the final unscaled damage in the Attacker's result if there was no Defensive Action payback, as it will use it later
                if (victimDefenseData.HasValue == false || victimDefenseData.Value.ElementHolder.HasValue == false)
                {
                    StepResult.AttackerResult.TotalDamage = StepResult.VictimResult.TotalDamage;
                }
            }
        }

        /// <summary>
        /// Filters out DamageEffects caused by the move depending on whether the BattleEntity is vulnerable to them or not.
        /// </summary>
        private sealed class VictimDamageEffectStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //If the attack didn't hit, don't factor in DamageEffects
                if (StepResult.VictimResult.Hit == false)
                {
                    StepResult.VictimResult.DamageEffect = DamageEffects.None;
                }
        
                //If the current result has no DamageEffects (whether the move didn't have any or a Defensive Action removed them)
                //or if the BattleEntity isn't vulnerable to any DamageEffects, then don't bother doing anything else
                if (StepResult.VictimResult.DamageEffect == DamageEffects.None
                    || StepResult.VictimResult.Entity.EntityProperties.HasDamageEffectVulnerabilities() == false)
                {
                    return;
                }

                //The DamageEffects stored in the result
                DamageEffects resultEffects = DamageEffects.None;

                //Get all the DamageEffects
                DamageEffects[] damageEffects = UtilityGlobals.GetEnumValues<DamageEffects>();

                //Start at index 1, as 0 is the value of None indicating no DamageEffects
                for (int i = 1; i < damageEffects.Length; i++)
                {
                    DamageEffects curEffect = damageEffects[i];

                    //If the move has the DamageEffect and the entity is affected by it, add it to the result
                    //This approach is easier and more readable than removing effects
                    if (UtilityGlobals.DamageEffectHasFlag(StepResult.VictimResult.DamageEffect, curEffect) == true
                        && StepResult.VictimResult.Entity.EntityProperties.IsVulnerableToDamageEffect(curEffect) == true)
                    {
                        resultEffects |= curEffect;
                    }
                }

                //Set the result
                StepResult.VictimResult.DamageEffect = resultEffects;
            }
        }

        private sealed class VictimDoublePainStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //Factor in Double Pain for the Victim
                int doublePainCount = StepResult.VictimResult.Entity.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DoublePain);

                StepResult.VictimResult.TotalDamage *= (1 + doublePainCount);
            }
        }

        private sealed class VictimLastStandStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //Factor in Last Stand for the Victim, if the Victim is in Danger or Peril
                if (StepResult.VictimResult.Entity.IsInDanger == true)
                {
                    //PM rounds down, whereas TTYD rounds up. We're going with the latter
                    //TTYD always ceilings the value (Ex. 3.2 turns to 4)
                    int lastStandCount = StepResult.VictimResult.Entity.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.LastStand);
                    
                    int lastStandDivider = (1 + lastStandCount);
                    StepResult.VictimResult.TotalDamage = (int)Math.Ceiling(StepResult.VictimResult.TotalDamage / (float)lastStandDivider);
                }
            }
        }

        private sealed class ClampVictimDamageStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //Clamp Victim damage
                StepResult.VictimResult.TotalDamage =
                    UtilityGlobals.Clamp(StepResult.VictimResult.TotalDamage, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }
        }

        private sealed class VictimFilteredStatusStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StepResult.VictimResult.StatusesInflicted =
                    GetFilteredInflictedStatuses(StepResult.VictimResult.Entity, StepResult.VictimResult.StatusesInflicted);
            }
        }

        private sealed class VictimCheckInvincibleStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //Check if the Victim is Invincible. If so, ignore all damage
                //If Invincible entities want to be immune to Status Effects, they should add immunities when turning Invincible
                if (StepResult.VictimResult.Entity.IsInvincible() == true)
                {
                    StepResult.VictimResult.TotalDamage = 0;
                    StepResult.VictimResult.ElementResult = ElementInteractionResult.Damage;
                }
            }
        }

        /// <summary>
        /// Final Victim step - ALL Victim damage information is known after this.
        /// </summary>
        private sealed class VictimCheckContactResultStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                if (StepContactResultInfo.ContactResult == ContactResult.Failure)
                {
                    //If the Attacker failed to attack, mark that the Victim shouldn't take damage
                    //This prevents the code afterwards from dealing damage to the Victim
                    StepResult.VictimResult.ShouldDamageEntity = false;
                }
            }
        }

        private sealed class AttackerPaybackDamageStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //The unscaled damage the Attacker dealt to the Victim
                //This will be the Payback damage dealt from a Defensive Action if one that deals damage has been performed
                int unscaledAttackerDamage = StepResult.AttackerResult.TotalDamage;

                int damageDealt = unscaledAttackerDamage;
                PaybackHolder paybackHolder = StepContactResultInfo.Paybackholder;

                //Get the damage done to the Attacker, factoring in Weaknesses/Resistances
                ElementDamageResultHolder attackerElementDamage = GetElementalDamage(StepResult.AttackerResult.Entity,
                    paybackHolder.Element, damageDealt);

                //Get Payback damage - Payback damage is calculated after everything else
                //However, it does NOT factor in Double Pain or Last Stand, hence why we use the unscaled Victim damage
                int paybackDamage = paybackHolder.GetPaybackDamage(attackerElementDamage.Damage);

                //If Constant Payback, the constant damage value will be returned as the Payback damage
                //Therefore, update the final Payback damage value to factor in Weaknesses/Resistances using the constant Payback damage
                //Ex. This causes an enemy with a +1 Weakness to Fire to be dealt 2 damage instead of 1 for a Constant 1 Fire Payback
                if (paybackHolder.PaybackType == PaybackTypes.Constant)
                {
                    paybackDamage = GetElementalDamage(StepResult.AttackerResult.Entity, paybackHolder.Element, paybackDamage).Damage;
                }

                //Fill out the rest of the Attacker information since we have it
                //Payback damage is always direct, piercing, and guaranteed to hit
                StepResult.AttackerResult.TotalDamage = paybackDamage;
                StepResult.AttackerResult.DamageElement = paybackHolder.Element;
                StepResult.AttackerResult.ElementResult = attackerElementDamage.InteractionResult;
                StepResult.AttackerResult.ContactType = ContactTypes.TopDirect;
                StepResult.AttackerResult.Piercing = true;
                StepResult.AttackerResult.StatusesInflicted = paybackHolder.StatusesInflicted;
                StepResult.AttackerResult.Hit = true;
                StepResult.AttackerResult.IsPaybackDamage = true;
            }
        }

        private class ClampAttackerDamageStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StepResult.AttackerResult.TotalDamage = 
                    UtilityGlobals.Clamp(StepResult.AttackerResult.TotalDamage, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }
        }

        private class AttackerFilteredStatusStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                StepResult.AttackerResult.StatusesInflicted =
                    GetFilteredInflictedStatuses(StepResult.AttackerResult.Entity, StepResult.AttackerResult.StatusesInflicted);
            }
        }

        private class AttackerCheckInvincibleStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                //Check if the Attacker is Invincible. If so, ignore all damage
                //If Invincible entities want to be immune to Status Effects, they should add immunities when turning Invincible
                if (StepResult.AttackerResult.Entity.IsInvincible() == true)
                {
                    StepResult.AttackerResult.TotalDamage = 0;
                    StepResult.AttackerResult.ElementResult = ElementInteractionResult.Damage;
                }
            }
        }

        /// <summary>
        /// Final Attacker step - ALL Attacker damage information is known after this.
        /// </summary>
        private sealed class AttackerCheckContactResultStep : DamageCalcStep
        {
            protected override void OnCalculate(InteractionParamHolder damageInfo, InteractionResult curResult, ContactResultInfo curContactResult)
            {
                if (StepContactResultInfo.ContactResult == ContactResult.Success)
                {
                    //If the Attacker succeeded to attack, mark that the Attacker shouldn't take damage
                    //This prevents the code afterwards from dealing damage to the Attacker
                    StepResult.AttackerResult.ShouldDamageEntity = false;
                }
            }
        }

        #endregion
    }
}
