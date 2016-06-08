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

        #endregion

        static Interactions()
        {
            InitalizeContactTable();
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
                { PhysicalAttributes.Fiery, ContactResult.Failure }
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
    }
}
