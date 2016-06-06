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
        #region Fields    
    
        /// <summary>
        /// The table that determines the result of a particular ContactType and a particular PhysicalAttribute.
        /// <para>The default value is a Success, meaning the ContactType will deal damage. A Failure indicates a backfire (Ex. Mario
        /// jumping on a spiked enemy)</para>
        /// </summary>
        private static Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResult>> ContactTable = null;

        /// <summary>
        /// The table that determines the damage modifier each Element deals to each PhysicalAttribute.
        /// <para>Ex. For Water to deal 2x damage to Burning, Water needs a modifier of 2.
        /// For Fire to heal Burning, it needs a modifier of -1.</para>
        /// </summary>
        private static Dictionary<Enumerations.Elements, Dictionary<Enumerations.PhysicalAttributes, float>> DamageTable = null;

        #endregion

        static Interactions()
        {
            InitalizeContactTable();

            //1. The element being used
            //2. The physical attribute to test against
            //3. The modifier against the attribute
            //Ex. Water has a 2f modifier against Burning, so it does (damage * 2f) to the enemy

            //The default modifier is the value of the const, DefaultElementModifier, and is used when no modifiers are found
            //For cases where elements heal certain attributes (Ex. Fire moves healing Burning enemies), the modifier is negative
            InitializeDamageTable();
        }

        #region Contact Table Initialization Methods

        private static void InitalizeContactTable()
        {
            ContactTable = new Dictionary<ContactTypes, Dictionary<PhysicalAttributes, ContactResult>>();

            InitializeNoneContactTable();
            InitializeJumpContactTable();
            InitializeHammerContactTable();
        }

        private static void InitializeNoneContactTable()
        {

        }

        private static void InitializeJumpContactTable()
        {
            ContactTable.Add(ContactTypes.JumpContact, new Dictionary<PhysicalAttributes, ContactResult>()
            {
                { PhysicalAttributes.Spiked, ContactResult.Failure },
                { PhysicalAttributes.Electrified, ContactResult.Failure },
                { PhysicalAttributes.Burning, ContactResult.Failure }
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
        /// <param name="contactType">The ContactType performed</param>
        /// <param name="physAttributes">The set of PhysicalAttributes to test against</param>
        /// <returns>A ContactResult of the interaction</returns>
        public static ContactResult GetContactResult(ContactTypes contactType, params PhysicalAttributes[] physAttributes)
        {
            //Return the default value
            if (ContactTable.ContainsKey(contactType) == false || physAttributes == null)
            {
                Debug.LogWarning($"{nameof(physAttributes)} array is null or {nameof(ContactTable)} does not contain the ContactType {contactType}!");
                return ContactResult.Success;
            }

            //Look through the attributes and see if there are any Failures. A single Failure overrides all Successes
            for (int i = 0; i < physAttributes.Length; i++)
            {
                Dictionary<PhysicalAttributes, ContactResult> tableForContact = ContactTable[contactType];
                PhysicalAttributes attribute = physAttributes[i];
                if (tableForContact.ContainsKey(attribute) == true)
                {
                    ContactResult contactResult = tableForContact[attribute];
                    if (contactResult == ContactResult.Failure) return contactResult;
                }
            }

            return ContactResult.Success;
        }

        #endregion

        #region Damage Table Initialization Methods

        /// <summary>
        /// Initializes the damage table. This is called in the static constructor
        /// </summary>
        private static void InitializeDamageTable()
        {
            DamageTable = new Dictionary<Enumerations.Elements, Dictionary<Enumerations.PhysicalAttributes, float>>();

            InitializeSharpTable();
            InitializeWaterTable();
            InitializeFireTable();
            InitializeElectricTable();
            InitializeIceTable();
            InitializePoisonTable();
            InitializeExplosionTable();
        }

        private static void InitializeSharpTable()
        {

        }

        private static void InitializeWaterTable()
        {
            DamageTable.Add(Enumerations.Elements.Water, new Dictionary<Enumerations.PhysicalAttributes, float>()
                    {
                        { Enumerations.PhysicalAttributes.Burning, 2f }
                    });
        }

        private static void InitializeFireTable()
        {
            DamageTable.Add(Enumerations.Elements.Fire, new Dictionary<Enumerations.PhysicalAttributes, float>()
                    {
                        { Enumerations.PhysicalAttributes.Burning, -1f }
                    });
        }

        private static void InitializeElectricTable()
        {

        }

        private static void InitializeIceTable()
        {

        }

        private static void InitializePoisonTable()
        {

        }

        private static void InitializeExplosionTable()
        {

        }

        #endregion

        #region Damage Table Methods

        /// <summary>
        /// Gets the total damage modifier of a particular elemental against a set of physical attributes.
        /// Negative values heal the one being attacked
        /// <para>NOTE: Since I can't find any cases in the first two Paper Mario games where moves have more than one element
        /// or enemies weak to more than one element, this logic is up in the air. I'm going with the Pokémon approach
        /// of factoring in all the modifiers. Like Pokémon, this will be done multiplicatively rather than additively.
        /// Undesirable values may result based on the individual modifiers, but it works perfectly fine with only one
        /// modifier like in the actual games</para>
        /// </summary>
        /// <param name="element">The element being used</param>
        /// <param name="physAttributes">The physical attributes to test against</param>
        /// <returns>A float with the total damage modifier</returns>
        public static float GetDamageModifier(Elements element, params PhysicalAttributes[] physAttributes)
        {
            //Return the default value
            if (DamageTable.ContainsKey(element) == false || physAttributes == null)
            {
                Debug.LogWarning($"{nameof(physAttributes)} array is null or {nameof(DamageTable)} does not contain the element {element}!");
                return BattleGlobals.DefaultElementModifier;
            }

            //Start out at null and assign it to the first one we find
            //This ensures we multiply them correctly and prevents problems with the modifiers ending up at 0
            float? totalModifier = null;

            //Look through the physical attributes this element affects, and check if the attributes in question match
            for (int i = 0; i < physAttributes.Length; i++)
            {
                Dictionary<PhysicalAttributes, float> tableForElement = DamageTable[element];
                PhysicalAttributes attribute = physAttributes[i];
                if (tableForElement.ContainsKey(attribute) == true)
                {
                    float modifier = tableForElement[attribute];

                    if (totalModifier.HasValue == false) totalModifier = modifier;
                    else totalModifier *= modifier;
                }
            }

            //If no matches were found return the default modifier, otherwise return the result
            return totalModifier.HasValue == false ? BattleGlobals.DefaultElementModifier : totalModifier.Value;
        }

        #endregion
    }
}
