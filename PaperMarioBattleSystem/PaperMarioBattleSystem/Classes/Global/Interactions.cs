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
        /// <para>The default value is a Success, meaning the ContactType will deal damage. A Failure indicates a backfire (Ex. Mario
        /// jumping on a spiked enemy)</para>
        /// </summary>
        private static Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResultInfo>> ContactTable = null;

        #endregion

        static Interactions()
        {
            InitalizeContactTable();
        }

        #region Contact Table Initialization Methods

        private static void InitalizeContactTable()
        {
            ContactTable = new Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResultInfo>>();

            InitializeNoneContactTable();
            InitializeJumpContactTable();
            InitializeHammerContactTable();
        }

        private static void InitializeNoneContactTable()
        {

        }

        //NOTE: In the actual games, if you have the Payback status, it takes priority over any PhysicalAttributes when being dealt damage
        //For example, if you have both Return Postage and Zap Tap equipped, sucking enemies like Fuzzies will be able to touch you
        //However, normal properties apply when attacking enemies (you'll be able to jump on Electrified enemies)

        private static void InitializeJumpContactTable()
        {
            ContactTable.Add(ContactTypes.JumpContact, new Dictionary<PhysicalAttributes, ContactResultInfo>()
            {
                { PhysicalAttributes.Spiked, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Sharp, 1), ContactResult.Failure, false) },
                { PhysicalAttributes.Electrified, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Electric, 1), ContactResult.PartialSuccess, true) },
                { PhysicalAttributes.Fiery, new ContactResultInfo(new PaybackHolder(PaybackTypes.Constant, Elements.Fire, 1), ContactResult.Failure, false) }
            });
        }

        private static void InitializeHammerContactTable()
        {

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

        #region Interaction Methods

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
         * 9. If in Danger or Peril, divide by: (number of Last Stands equipped + 1) (round up the damage if it's > 0 and < 1)
         * 
         * 10. Clamp the damage: Min = 0, Max = 99
         * 
         * Attacker:
         * ---------
         * 1. Start with the total damage dealt to the Victim
         * 2. Get the Payback from the Contact Result
         * 3. If the Victim performed a Superguard, override the Payback with the the Superguard's Payback
         * 4. Calculate the Attacker's Weaknesses/Resistances to the Payback Element
         * 5. Calculate the Payback damage based off the PaybackType
         * 6. If the PaybackType is Constant, set the damage dealt to the Payback's damage
         * 
         * 7. Clamp the damage: Min = 0, Max = 99
         */

        /// <summary>
        /// Calculates and returns the entire damage interaction between two BattleEntities.
        /// <para>This returns all the necessary information for both BattleEntities, including the total amount of damage dealt,
        /// the type of Elemental damage to deal, the Status Effects to inflict, and whether the attack successfully hit or not.</para>
        /// </summary>
        /// <param name="interactionParam">An InteractionParamHolder containing the BattleEntities interacting and data about their interaction</param>
        /// <returns>An InteractionResult containing InteractionHolders for both the victim and the attacker</returns>
        public static InteractionResult GetDamageInteraction(InteractionParamHolder interactionParam)
        {
            InteractionResult finalInteractionResult = new InteractionResult();

            BattleEntity attacker = interactionParam.Attacker;
            BattleEntity victim = interactionParam.Victim;
            ContactTypes contactType = interactionParam.ContactType;
            Elements element = interactionParam.DamagingElement;
            StatusEffect[] statuses = interactionParam.Statuses;
            int damage = interactionParam.Damage;
            bool piercing = interactionParam.Piercing;

            //Get contact results
            ContactResultInfo contactResultInfo = victim.EntityProperties.GetContactResult(attacker, contactType);
            ContactResult contactResult = contactResultInfo.ContactResult;

            //Retrieve an overridden type of Elemental damage to inflict based on the Victim's PhysicalAttributes
            //(Ex. The Ice Power Badge only deals Ice damage to Fiery entities)
            Elements newElement = attacker.EntityProperties.GetTotalElementOverride(victim);
            if (newElement != Elements.Invalid)
            {
                //Add 1 to the damage if the element used already exists as an override and the victim has a Weakness to the Element.
                //This allows Badges such as Ice Power to deal more damage if used in conjunction with attacks
                //that deal the same type of damage (Ex. Ice Power and Ice Smash deal 2 additional damage total rather than 1).
                //If any new knowledge is discovered to improve this, this will be changed. At the moment, it seems unlikely because
                //Ice Power is the only Badge of its kind across the first two PM games that does anything like this
                if (element == newElement && victim.EntityProperties.HasWeakness(element) == true)
                {
                    damage += 1;
                }

                element = newElement;
            }

            /*Get the total damage dealt to the Victim. The amount of Full or Half Payback damage dealt to the Attacker
              uses the resulting damage value from this because Payback uses the total damage that would be dealt to the Victim.
              This occurs before factoring in elemental resistances/weaknesses from the Attacker*/
            ElementDamageResultHolder victimElementDamage = GetElementalDamage(victim, element, damage);//GetElementalDamageWithStrengths(attacker, victim, element, damage);

            int unscaledVictimDamage = victimElementDamage.Damage;

            //Subtract damage reduction (P-Up, D-Down and P-Down, D-Up Badges)
            unscaledVictimDamage -= victim.BattleStats.DamageReduction;

            //Defense added from Damage Dodge Badges upon a successful Guard
            int damageDodgeDefense = 0;

            //Defensive actions take priority
            BattleGlobals.DefensiveActionHolder? victimDefenseData = victim.GetDefensiveActionResult(victimElementDamage.Damage, statuses);
            if (victimDefenseData.HasValue == true)
            {
                unscaledVictimDamage = victimDefenseData.Value.Damage;
                statuses = victimDefenseData.Value.Statuses;
                //If the Defensive action dealt damage and the contact was direct
                //the Defensive action has caused a Failure for the Attacker (Ex. Superguarding)
                if (contactType == ContactTypes.JumpContact && victimDefenseData.Value.ElementHolder.HasValue == true)
                {
                    contactResult = ContactResult.Failure;
                }

                //Factor in the additional Guard defense for all DefensiveActions (for now, at least)
                damageDodgeDefense = victim.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DamageDodge);
            }

            //Subtract Defense on non-piercing damage
            if (piercing == false)
            {
                int totalDefense = victim.BattleStats.TotalDefense + damageDodgeDefense;
                unscaledVictimDamage -= totalDefense;
            }

            int scaledVictimDamage = unscaledVictimDamage;

            //Factor in Double Pain for the Victim
            scaledVictimDamage *= (1 + victim.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.DoublePain));

            //Factor in Last Stand for the Victim, if the Victim is in Danger or Peril
            if (victim.IsInDanger == true)
            {
                //NOTE: PM rounds down, whereas TTYD rounds up. We're going with the latter
                //I'm not sure if TTYD always ceilings the value (Ex. 3.2 turns to 4) or does standard rounding, so I'll stick with standard rounding for now
                int lastStandDivider = (1 + victim.GetEquippedBadgeCount(BadgeGlobals.BadgeTypes.LastStand));
                scaledVictimDamage = (int)Math.Round(scaledVictimDamage / (float)lastStandDivider);
            }

            //Clamp Victim damage
            scaledVictimDamage = UtilityGlobals.Clamp(scaledVictimDamage, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

            #region Victim Damage Dealt

            //Calculating damage dealt to the Victim
            if (contactResult == ContactResult.Success || contactResult == ContactResult.PartialSuccess)
            {
                //Get the Status Effects to inflict on the Victim
                StatusEffect[] victimInflictedStatuses = GetFilteredInflictedStatuses(victim, statuses);
                bool hit = attacker.AttemptHitEntity(victim);

                finalInteractionResult.VictimResult = new InteractionHolder(victim, scaledVictimDamage, element, 
                    victimElementDamage.InteractionResult, contactType, piercing, victimInflictedStatuses, hit);
            }

            #endregion

            #region Attacker Damage Dealt

            //Calculating damage dealt to the Attacker
            if (contactResult == ContactResult.Failure || contactResult == ContactResult.PartialSuccess)
            {
                //The damage the Attacker dealt to the Victim
                int damageDealt = unscaledVictimDamage;
                PaybackHolder paybackHolder = contactResultInfo.Paybackholder;
                
                //Override the PaybackHolder with a Defensive Action's results, if any
                if (victimDefenseData.HasValue == true && victimDefenseData.Value.ElementHolder.HasValue == true)
                {
                    damageDealt = victimDefenseData.Value.ElementHolder.Value.Damage;
                    paybackHolder = new PaybackHolder(PaybackTypes.Constant, victimDefenseData.Value.ElementHolder.Value.Element, damageDealt);
                }

                //Get the damage done to the Attacker, factoring in Weaknesses/Resistances
                ElementDamageResultHolder attackerElementDamage = GetElementalDamage(attacker, paybackHolder.Element, damageDealt);

                //Get Payback damage - Payback damage is calculated after everything else, including Constant Payback.
                //However, it does NOT factor in Double Pain or any sort of Defense modifiers.
                int paybackDamage = paybackHolder.GetPaybackDamage(attackerElementDamage.Damage);

                //If Constant Payback, update the damage value to use the element
                if (paybackHolder.PaybackType == PaybackTypes.Constant)
                {
                    paybackDamage = GetElementalDamage(attacker, paybackHolder.Element, paybackDamage).Damage;
                }

                //Clamp Attacker damage
                attackerElementDamage.Damage = UtilityGlobals.Clamp(paybackDamage, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

                //Get the Status Effects to inflict
                StatusEffect[] attackerInflictedStatuses = GetFilteredInflictedStatuses(attacker, paybackHolder.StatusesInflicted);
                    
                finalInteractionResult.AttackerResult = new InteractionHolder(attacker, attackerElementDamage.Damage, paybackHolder.Element,
                    attackerElementDamage.InteractionResult, ContactTypes.None, true, attackerInflictedStatuses, true);
            }

            #endregion

            return finalInteractionResult;
        }

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

        /*/// <summary>
        /// Calculates the result of elemental damage on a BattleEntity, based on its weaknesses and resistances to that Element.
        /// This also factors in the strengths of the attacker based on the BattleEntity's PhysicalAttributes.
        /// </summary>
        /// <param name="attacker">The BattleEntity dealing damage.</param>
        /// <param name="victim">The BattleEntity being damaged.</param>
        /// <param name="element">The element the entity is attacked with.</param>
        /// <param name="damage">The initial damage of the attack.</param>
        /// <returns>An ElementDamageHolder stating the result and final damage dealt to this entity.</returns>
        private static ElementDamageResultHolder GetElementalDamageWithStrengths(BattleEntity attacker, BattleEntity victim, Elements element, int damage)
        {
            ElementDamageResultHolder elementDamageResult = GetElementalDamage(victim, element, damage);

            //Factor in strengths
            StrengthHolder strength = attacker.EntityProperties.GetTotalStrength(victim);
            elementDamageResult.Damage += strength.Value;

            return elementDamageResult;
        }*/

        /// <summary>
        /// Filters an array of StatusEffects depending on whether they will be inflicted on a BattleEntity
        /// depending on the entity's status percentages
        /// </summary>
        /// <param name="entity">The BattleEntity to attempt to afflict the StatusEffects with</param>
        /// <param name="statusesToInflict">The original array of StatusEffects</param>
        /// <returns>An array of StatusEffects that has succeeded in being inflicted on the entity</returns>
        private static StatusEffect[] GetFilteredInflictedStatuses(BattleEntity entity, StatusEffect[] statusesToInflict)
        {
            //Handle null
            if (statusesToInflict == null) return statusesToInflict;

            //Construct a list with the original elements
            List<StatusEffect> filteredStatuses = new List<StatusEffect>(statusesToInflict);

            //Look through the list and remove any StatusEffects that fail to be afflicted onto the entity
            for (int i = 0; i < filteredStatuses.Count; i++)
            {
                StatusEffect status = filteredStatuses[i];
                if (entity.EntityProperties.TryAfflictStatus(status) == false)
                {
                    Debug.Log($"Failed to inflict {status.StatusType} on {entity.Name}");
                    filteredStatuses.RemoveAt(i);
                    i--;
                }
            }

            return filteredStatuses.ToArray();
        }

        #endregion
    }
}
