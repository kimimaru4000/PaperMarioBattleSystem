using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

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
        private struct ElementDamageHolder
        {
            public ElementInteractionResult InteractionResult;
            public int Damage;

            public ElementDamageHolder(ElementInteractionResult interactionResult, int damage)
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
                { PhysicalAttributes.Spiked, new ContactResultInfo(Elements.Sharp, ContactResult.Failure, false) },
                { PhysicalAttributes.Electrified, new ContactResultInfo(Elements.Electric, ContactResult.PartialSuccess, true) },
                { PhysicalAttributes.Fiery, new ContactResultInfo(Elements.Fire, ContactResult.Failure, false) }
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
                    if (contactResult.SuccessIfSameAttr == true && attacker.HasPhysAttributes(true, attribute) == true)
                        contactResult.ContactResult = ContactResult.Success;
                    return contactResult;
                }
            }

            return ContactResultInfo.Default;
        }

        #endregion

        #region Interaction Methods

        public static InteractionResult GetDamageInteraction(BattleEntity attacker, BattleEntity victim, int damage, Elements element,
            ContactTypes contactType, StatusEffect[] statuses)
        {
            InteractionResult finalInteractionResult = new InteractionResult();

            ContactResultInfo contactResultInfo = victim.GetContactResult(attacker, contactType);
            ContactResult contactResult = contactResultInfo.ContactResult;

            //Calculating damage dealt to the Victim
            if (contactResult == ContactResult.Success || contactResult == ContactResult.PartialSuccess)
            {
                //Calculate modifier damage
                int modifierDamage = victim.GetTotalDamageMod(attacker);
                int newDamage = damage + modifierDamage;

                ElementDamageHolder victimElementDamage = GetElementalDamage(victim, element, newDamage);
                StatusEffect[] inflictedStatuses = GetFilteredInflictedStatuses(victim, statuses);
                bool hit = attacker.AttemptHitEntity(victim);

                //NOTE: Statuses are always afflicted for now until entities get percentages of being affected by them
                finalInteractionResult.VictimResult = new InteractionHolder(victim, victimElementDamage.Damage, element, 
                    victimElementDamage.InteractionResult, contactType, false, inflictedStatuses, hit);
            }

            //Calculating damage dealt to the Attacker
            if (contactResult == ContactResult.Failure || contactResult == ContactResult.PartialSuccess)
            {
                ElementDamageHolder attackerElementDamage = GetElementalDamage(attacker, contactResultInfo.Element, 1);

                //NOTE: Statuses are not afflicted on the attacker unless contact with the victim causes it.
                //This isn't in place yet so don't pass anything in for now
                finalInteractionResult.AttackerResult = new InteractionHolder(attacker, attackerElementDamage.Damage, contactResultInfo.Element,
                    attackerElementDamage.InteractionResult, ContactTypes.None, true, null, true);
            }

            return finalInteractionResult;
        }

        /// <summary>
        /// Calculates the result of elemental damage on a BattleEntity, based on its weaknesses and resistances to that Element
        /// </summary>
        /// <param name="entity">The BattleEntity being damaged</param>
        /// <param name="element">The element the entity is attacked with</param>
        /// <param name="damage">The initial damage of the attack</param>
        /// <returns>An ElementDamageHolder stating the result and final damage dealt to this entity</returns>
        private static ElementDamageHolder GetElementalDamage(BattleEntity entity, Elements element, int damage)
        {
            ElementDamageHolder elementDamageResult = new ElementDamageHolder(ElementInteractionResult.Damage, damage);

            //NOTE: If an entity is both resistant and weak to a particular element, they cancel out.
            //I decided to go with this approach because it's the simplest for this situation, which
            //doesn't seem desirable to begin with but could be interesting in its application
            WeaknessHolder weakness = entity.GetWeakness(element);
            ResistanceHolder resistance = entity.GetResistance(element);
            
            //If there's both a weakness and resistance, return
            if (weakness.WeaknessType == WeaknessTypes.None && resistance.ResistanceType == ResistanceTypes.None)
                return elementDamageResult;

            //Handle weaknesses
            if (weakness.WeaknessType == WeaknessTypes.PlusDamage)
            {
                elementDamageResult.Damage = UtilityGlobals.Clamp(elementDamageResult.Damage + weakness.Value, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            }
            else if (weakness.WeaknessType == WeaknessTypes.KO)
            {
                elementDamageResult.InteractionResult = ElementInteractionResult.KO;
            }

            //Handle resistances
            if (resistance.ResistanceType == ResistanceTypes.MinusDamage)
            {
                elementDamageResult.Damage = UtilityGlobals.Clamp(elementDamageResult.Damage - resistance.Value, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
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
                if (entity.TryAfflictStatus(status) == false)
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
