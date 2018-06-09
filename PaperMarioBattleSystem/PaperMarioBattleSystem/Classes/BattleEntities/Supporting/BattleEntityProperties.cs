using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.StatusGlobals;
using PaperMarioBattleSystem.Utilities;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles numerous BattleEntity properties like weaknesses/resistances to certain Elements, and more.
    /// </summary>
    public class BattleEntityProperties : ICleanup
    {
        #region Delegates and Events

        public delegate void StatusInflicted(StatusEffect statusEffect);
        /// <summary>
        /// The event invoked when inflicting a BattleEntity with a Status Effect.
        /// This is invoked after the Status Effect is actually inflicted.
        /// </summary>
        public event StatusInflicted StatusInflictedEvent = null;

        public delegate void StatusRemoved(StatusEffect statusEffect);
        /// <summary>
        /// The event invoked when a Status Effect is removed from a BattleEntity.
        /// This is invoked after the Status Effect is actually removed.
        /// </summary>
        public event StatusRemoved StatusRemovedEvent = null;

        #endregion

        /// <summary>
        /// The BattleEntity these properties are for
        /// </summary>
        protected readonly BattleEntity Entity = null;

        /// <summary>
        /// The physical attributes the entity possesses. When added, they are sorted in decreasing order.
        /// </summary>
        protected readonly SortedDictionary<PhysicalAttributes, int> PhysAttributes = new SortedDictionary<PhysicalAttributes, int>(new PhysAttributeGlobals.PhysAttributeComparer());

        /// <summary>
        /// The exceptions this entity has for certain types of contact against certain PhysicalAttributes.
        /// <para>This is used for Badges such as Spike Shield and Ice Power.</para>
        /// </summary>
        protected readonly Dictionary<ContactTypes, List<PhysicalAttributes>> ContactExceptions = new Dictionary<ContactTypes, List<PhysicalAttributes>>();

        /// <summary>
        /// The Weaknesses the entity has.
        /// <para>A List is used for the value so additional Weaknesses can be added via moves or equipment.</para>
        /// </summary>
        protected readonly Dictionary<Elements, List<WeaknessHolder>> Weaknesses = new Dictionary<Elements, List<WeaknessHolder>>();

        /// <summary>
        /// The Resistances the entity has.
        /// <para>A List is used for the value so additional Resistances can be added via moves or equipment.</para>
        /// </summary>
        protected readonly Dictionary<Elements, List<ResistanceHolder>> Resistances = new Dictionary<Elements, List<ResistanceHolder>>();

        /// <summary>
        /// The Strengths the entity has against entities with PhysicalAttributes.
        /// </summary>
        protected readonly Dictionary<PhysicalAttributes, List<StrengthHolder>> Strengths = new Dictionary<PhysicalAttributes, List<StrengthHolder>>();

        /// <summary>
        /// The types of Elemental damage to use when dealing damage to BattleEntities with particular PhysicalAttributes
        /// </summary>
        protected readonly Dictionary<PhysicalAttributes, SortedDictionary<Elements, int>> ElementOverrides = new Dictionary<PhysicalAttributes, SortedDictionary<Elements, int>>();
        
        /// <summary>
        /// The StatusEffects the entity is afflicted with. When added, they are sorted by their priorities in the StatusOrder table.
        /// <para></para>
        /// <see cref="StatusGlobals.StatusOrder"/>
        /// </summary>
        protected readonly SortedDictionary<StatusTypes, StatusEffect> Statuses = new SortedDictionary<StatusTypes, StatusEffect>(new StatusGlobals.StatusTypeComparer());

        /// <summary>
        /// Properties relating to how the entity is affected by each StatusEffect.
        /// Examples include the likelihood of being inflicted by a StatusEffect-inducing attack and
        /// how many additional turns the entity is affected by the StatusEffect
        /// </summary>
        protected readonly Dictionary<StatusTypes, StatusPropertyHolder> StatusProperties = new Dictionary<StatusTypes, StatusPropertyHolder>();

        /// <summary>
        /// The DamageEffects the BattleEntity is vulnerable to.
        /// </summary>
        protected DamageEffects VulnerableDamageEffects = DamageEffects.None;

        /// <summary>
        /// Additional properties of the entity
        /// </summary>
        protected readonly Dictionary<AdditionalProperty, object> AdditionalProperties = new Dictionary<AdditionalProperty, object>();

        /// <summary>
        /// The Payback data of the entity
        /// </summary>
        protected readonly List<PaybackHolder> Paybacks = new List<PaybackHolder>();

        /// <summary>
        /// The move categories the entity cannot perform because they are disabled
        /// </summary>
        protected readonly Dictionary<MoveCategories, int> DisabledMoveCategories = new Dictionary<MoveCategories, int>();

        /// <summary>
        /// A delegate describing a custom targeting method.
        /// If true, the BattleEntity should remain in the target set, and if false, the BattleEntity should be excluded from the target set.
        /// </summary>
        /// <param name="moveAction">The MoveAction targeting this BattleEntity.</param>
        /// <returns>true if the BattleEntity should remain in the target set and false if the BattleEntity should be excluded from the target set.</returns>
        public delegate bool CustomTarget(in MoveAction moveAction);

        /// <summary>
        /// The custom targeting method to use. It defaults to null.
        /// </summary>
        public CustomTarget CustomTargeting { get; private set; } = null;

        #region Badge Fields

        /// <summary>
        /// The Badges equipped to this BattleEntity.
        /// </summary>
        protected readonly List<Badge> EquippedBadges = new List<Badge>();

        /// <summary>
        /// The counts for each type of Badge equipped to the BattleEntity.
        /// </summary>
        protected readonly Dictionary<BadgeGlobals.BadgeTypes, int> EquippedBadgeCounts = new Dictionary<BadgeGlobals.BadgeTypes, int>();

        #endregion

        #region Constructor

        public BattleEntityProperties(BattleEntity entity)
        {
            Entity = entity;
        }

        #endregion

        public void CleanUp()
        {
            RemoveAllStatuses();

            AdditionalProperties.Clear();
            PhysAttributes.Clear();
            ContactExceptions.Clear();

            Weaknesses.Clear();
            Resistances.Clear();
            Strengths.Clear();

            ElementOverrides.Clear();
            StatusProperties.Clear();

            AdditionalProperties.Clear();

            Paybacks.Clear();
            DisabledMoveCategories.Clear();

            //Unequip all Badges from the BattleEntity
            for (int i = EquippedBadges.Count - 1; i >= 0; i--)
            {
                EquippedBadges[i].UnEquip();
            }

            EquippedBadgeCounts.Clear();

            CustomTargeting = null;

            StatusInflictedEvent = null;
            StatusRemovedEvent = null;
        }

        #region Physical Attribute Methods

        /// <summary>
        /// Adds a physical attribute to the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to add</param>
        public void AddPhysAttribute(PhysicalAttributes physicalAttribute)
        {
            if (PhysAttributes.ContainsKey(physicalAttribute) == true)
            {
                PhysAttributes[physicalAttribute]++;
                Debug.Log($"Incremented the physical attribute {physicalAttribute} for {Entity.Name}!");
                return;
            }
            else
            {
                Debug.Log($"Added the physical attribute {physicalAttribute} to {Entity.Name}'s existing attributes!");
                PhysAttributes.Add(physicalAttribute, 1);
            }
        }

        /// <summary>
        /// Removes a physical attribute from the entity
        /// </summary>
        /// <param name="physicalAttribute">The physical attribute to remove</param>
        public void RemovePhysAttribute(PhysicalAttributes physicalAttribute)
        {
            int attributeNum = -1;
            if (PhysAttributes.TryGetValue(physicalAttribute, out attributeNum) == false)
            {
                Debug.LogWarning($"Cannot remove physical attribute {physicalAttribute} because {Entity.Name} does not have it!");
                return;
            }

            attributeNum--;

            if (attributeNum <= 0)
            {
                PhysAttributes.Remove(physicalAttribute);
                Debug.Log($"Removed the physical attribute {physicalAttribute} from {Entity.Name}'s existing attributes!");
            }
            else
            {
                PhysAttributes[physicalAttribute] = attributeNum;
                Debug.Log($"Decremented the physical attribute {physicalAttribute} for {Entity.Name}!");
            }
        }

        /// <summary>
        /// Tells whether the entity has a set of physical attributes or not
        /// </summary>
        /// <param name="checkAny">If true, checks the entity has any of the physical attributes rather than all</param>
        /// <param name="attributes">The set of physical attributes to check the entity has</param>
        /// <returns>true if the entity has any or all, based on the value of checkAny, of the physical attributes in the set, otherwise false</returns>
        public bool HasPhysAttributes(bool checkAny, params PhysicalAttributes[] attributes)
        {
            if (attributes == null) return false;

            //Loop through and look at each attribute
            //If we're looking for all attributes, return false if one is not found
            //If we're looking for any attribute, return true if one is found
            for (int i = 0; i < attributes.Length; i++)
            {
                if (PhysAttributes.ContainsKey(attributes[i]) == checkAny) return checkAny;
            }

            return !checkAny;
        }

        /// <summary>
        /// Determines the result of contact, based on the type of contact made, when it's made with this entity.
        /// <para>Contacts that aren't a Success are prioritized over any Payback.
        /// If a ContactResult of Success is found, then the Payback for this entity is added if it exists
        /// and the ContactResult becomes a PartialSuccess.</para>
        /// </summary>
        /// <param name="attacker">The entity attacking this one</param>
        /// <param name="contactType">The type of contact made with this entity</param>
        /// <param name="contactProperty">The ContactProperty of the contact.</param>
        /// <returns>A ContactResultInfo containing the result of the interaction</returns>
        public ContactResultInfo GetContactResult(BattleEntity attacker, ContactTypes contactType, ContactProperties contactProperty)
        {
            ContactResultInfo contactResultInfo = Interactions.GetContactResult(attacker.EntityProperties.GetAllPhysAttributes(), contactType, contactProperty, GetAllPaybacks(), attacker.EntityProperties.GetContactExceptions(contactType));

            return contactResultInfo;
        }

        /// <summary>
        /// Returns all PhysicalAttributes the BattleEntity has.
        /// </summary>
        /// <returns>An array of all PhysicalAttributes the BattleEntity has, with higher PhysicalAttribute values first.</returns>
        public PhysicalAttributes[] GetAllPhysAttributes()
        {
            if (PhysAttributes.Keys.Count == 0) return Array.Empty<PhysicalAttributes>();

            return PhysAttributes.Keys.ToArray();
        }

        /// <summary>
        /// Adds all PhysicalAttributes the BattleEntity has into a supplied list.
        /// </summary>
        /// <param name="physAttrList">The list to add the BattleEntity's PhysicalAttributes to.</param>
        public void GetAllPhysAttributes(List<PhysicalAttributes> physAttrList)
        {
            foreach (PhysicalAttributes physAttribute in PhysAttributes.Keys)
            {
                physAttrList.Add(physAttribute);
            }
        }

        #endregion

        #region Element Override Methods

        /// <summary>
        /// Adds an Element Override of an Element to this BattleEntity for a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the Element Override</param>
        /// <param name="element">The Element to add for this PhysicalAttribute</param>
        public void AddElementOverride(PhysicalAttributes attribute, Elements element)
        {
            SortedDictionary<Elements, int> elementOverride = GetElementOverride(attribute);

            //Add a new entry if one doesn't exist
            if (elementOverride == null)
            {
                elementOverride = new SortedDictionary<Elements, int>(new ElementGlobals.ElementComparer());
                ElementOverrides.Add(attribute, elementOverride);
            }

            //If we don't have an override for this PhysicalAttribute with this Element, add one
            if (elementOverride.ContainsKey(element) == false)
            {
                elementOverride.Add(element, 1);
            }
            //Increment the count otherwise
            else
            {
                elementOverride[element] += 1;
            }

            Debug.Log($"Added a(n) {element} override to {Entity.Name} for the {attribute} PhysicalAttribute!");
        }

        /// <summary>
        /// Removes an Element Override of an Element this BattleEntity has for a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the Element Override</param>
        /// <param name="element">The Element to remove for the Element Override</param>
        public void RemoveElementOverride(PhysicalAttributes attribute, Elements element)
        {
            SortedDictionary<Elements, int> elementOverride = GetElementOverride(attribute);
            int elementOverrideVal = -1;

            if (elementOverride == null || elementOverride.TryGetValue(element, out elementOverrideVal) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not contain an element override for the {attribute} PhysicalAttribute and thus cannot remove one!");
                return;
            }

            //Decrement the count for the Element on this PhysicalAttribute
            elementOverrideVal--;
            Debug.Log($"Decremented a(n) {element} override from {Entity.Name} for the {attribute} PhysicalAttribute!");

            if (elementOverrideVal <= 0)
            {
                elementOverride.Remove(element);
                Debug.Log($"Removed the element {element} for the {attribute} PhysicalAttribute from {Entity.Name}'s Element Overrides!");

                //If no Elements are remaining for this PhysicalAttribute, remove the Element Override
                if (elementOverride.Count <= 0)
                {
                    ElementOverrides.Remove(attribute);
                    Debug.Log($"Removed element override for the {attribute} PhysicalAttribute on {Entity.Name}");
                }
            }
            else
            {
                elementOverride[element] = elementOverrideVal;
            }
        }

        /// <summary>
        /// Tells if the BattleEntity has an Element Override for a particular PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the Element Override</param>
        /// <returns>true if the Element Override exists for the PhysicalAttribute, otherwise false</returns>
        public bool HasElementOverride(PhysicalAttributes attribute)
        {
            return ElementOverrides.ContainsKey(attribute);
        }

        /// <summary>
        /// Tells if the BattleEntity has an Element Override of a particular Element for a particular PhysicalAttribute.
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the Element Override.</param>
        /// <param name="element">The Element of the Element Override.</param>
        /// <returns>true if an Element Override of the Element exists for the PhysicalAttribute, otherwise false.</returns>
        public bool HasElementOverride(PhysicalAttributes attribute, Elements element)
        {
            SortedDictionary<Elements, int> elementOverride = GetElementOverride(attribute);
            return (elementOverride != null && elementOverride.ContainsKey(element) == true);
        }

        /// <summary>
        /// Retrieves the Element Override this BattleEntity has for the first PhysicalAttribute found on a victim
        /// </summary>
        /// <param name="attacker">The BattleEntity this one is attacking</param>
        /// <returns>An ElementOverrideHolder with the type of Element damage this BattleEntity will do and how many overrides of that Element exist.</returns>
        public ElementOverrideHolder GetTotalElementOverride(BattleEntity victim)
        {
            PhysicalAttributes[] victimAttributes = victim.EntityProperties.GetAllPhysAttributes();

            for (int i = 0; i < victimAttributes.Length; i++)
            {
                PhysicalAttributes physAttribute = victimAttributes[i];

                SortedDictionary<Elements, int> elementOverride = GetElementOverride(physAttribute);
                if (elementOverride != null)
                {
                    //NOTE: I'm not happy with the overall performance of this, but it's definitely better than
                    //not allowing more Elements or their counts for each override

                    //Return the first one since they're sorted
                    Elements[] elementsForOverride = GetElementsForOverride(elementOverride, physAttribute);
                    Elements element = elementsForOverride[0];

                    return new ElementOverrideHolder(element, elementOverride[element]);
                }
            }

            return ElementOverrideHolder.Default;
        }

        /// <summary>
        /// Returns all Elements of an Element Override the BattleEntity has for a particular PhysicalAttribute.
        /// </summary>
        /// <param name="physAttribute">The PhysicalAttribute to get the Element Overrides for.</param>
        /// <returns>An array of all Elements of the Element Override the BattleEntity has for the PhysicalAttribute, with higher Element values first.</returns>
        protected Elements[] GetElementsForOverride(PhysicalAttributes physAttribute)
        {
            return GetElementsForOverride(GetElementOverride(physAttribute), physAttribute);
        }

        /// <summary>
        /// Returns all Elements of an Element Override the BattleEntity has for a particular PhysicalAttribute.
        /// </summary>
        /// <param name="elementOverride">The Element Override dictionary to look with.</param>
        /// <param name="physAttribute">The PhysicalAttribute to get the Element Overrides for.</param>
        /// <returns>An array of all Elements of the Element Override the BattleEntity has for the PhysicalAttribute, with higher Element values first.</returns>
        protected Elements[] GetElementsForOverride(SortedDictionary<Elements, int> elementOverride, PhysicalAttributes physAttribute)
        {
            if (elementOverride == null || elementOverride.Keys.Count == 0)
            {
                return Array.Empty<Elements>();
            }

            return elementOverride.Keys.ToArray();
        }

        /// <summary>
        /// Gets the Element Override dictionary associated with a particular PhysicalAttribute. This method is internal.
        /// </summary>
        /// <param name="physAttribute">The PhysicalAttribute to get the Element Overrides for.</param>
        /// <returns>A SortedDictionary containing the Element Overrides.</returns>
        private SortedDictionary<Elements, int> GetElementOverride(PhysicalAttributes physAttribute)
        {
            SortedDictionary<Elements, int> elementOverride = null;
            ElementOverrides.TryGetValue(physAttribute, out elementOverride);

            return elementOverride;
        }

        #endregion

        #region Contact Exception Methods

        /// <summary>
        /// Adds a contact exception to the entity
        /// </summary>
        /// <param name="contactType"></param>
        /// <param name="physAttribute"></param>
        public void AddContactException(ContactTypes contactType, PhysicalAttributes physAttribute)
        {
            List<PhysicalAttributes> physAttrList = null;

            //Add a new key if one doesn't exist
            if (ContactExceptions.TryGetValue(contactType, out physAttrList) == false)
            {
                physAttrList = new List<PhysicalAttributes>();
                ContactExceptions.Add(contactType, physAttrList);
            }

            //Add to the list
            physAttrList.Add(physAttribute);

            Debug.Log($"Added contact exception on {Entity.Name} for the {physAttribute} PhysicalAttribute during {contactType} contact!");
        }

        public void RemoveContactException(ContactTypes contactType, PhysicalAttributes physAttribute)
        {
            List<PhysicalAttributes> physAttrList = null;

            if (ContactExceptions.TryGetValue(contactType, out physAttrList) == false)
            {
                Debug.LogError($"Cannot remove {physAttribute} from the exception list on {Entity.Name} for {contactType} because no list exists!");
                return;
            }

            bool removed = physAttrList.Remove(physAttribute);
            if (removed == true)
            {
                Debug.Log($"Removed {physAttribute} attribute exception on {Entity.Name} for {contactType} contact!");
            }

            //If there are no PhysicalAttributes in the exceptions list for this ContactType, remove the key
            if (physAttrList.Count == 0)
            {
                ContactExceptions.Remove(contactType);
            }
        }

        /// <summary>
        /// Returns a set of PhysicalAttributes to ignore when the BattleEntity makes contact.
        /// </summary>
        /// <param name="contactType">The type of contact this BattleEntity made.</param>
        /// <returns>An array of PhysicalAttributes this BattleEntity can ignore when making contact, otherwise an empty array.</returns>
        public PhysicalAttributes[] GetContactExceptions(ContactTypes contactType)
        {
            List<PhysicalAttributes> physAttrList = null;

            //Return an empty array if no exceptions exist for this type of contact
            if (ContactExceptions.TryGetValue(contactType, out physAttrList) == false || physAttrList.Count == 0)
            {
                return Array.Empty<PhysicalAttributes>();
            }

            return physAttrList.ToArray();
        }

        /// <summary>
        /// Adds a set of PhysicalAttributes to ignore when the BattleEntity makes contact into a supplied list.
        /// </summary>
        /// <param name="physAttributeList">The list to add the PhysicalAttributes to.</param>
        /// <param name="contactType">The type of contact this BattleEntity made.</param>
        public void GetContactExceptions(List<PhysicalAttributes> physAttributeList, ContactTypes contactType)
        {
            List<PhysicalAttributes> physAttrList = null;
            if (ContactExceptions.TryGetValue(contactType, out physAttrList) == true)
            {
                physAttributeList.CopyFromList(physAttrList);
            }
        }

        #endregion

        #region Weakness Methods

        /// <summary>
        /// Adds a Weakness on the BattleEntity
        /// </summary>
        /// <param name="element">The Element the BattleEntity is weak to</param>
        /// <param name="weaknessHolder">The data for the Weakness</param>
        public void AddWeakness(Elements element, in WeaknessHolder weaknessHolder)
        {
            List<WeaknessHolder> weaknessList = null;
            if (Weaknesses.TryGetValue(element, out weaknessList) == false)
            {
                weaknessList = new List<WeaknessHolder>();
                Weaknesses.Add(element, weaknessList);
            }

            weaknessList.Add(weaknessHolder);
            Debug.Log($"Added {weaknessHolder.WeaknessType} Weakness to {Entity.Name} for the {element} Element!");
        }

        /// <summary>
        /// Removes a Weakness on the BattleEntity
        /// </summary>
        /// <param name="element">The Element the BattleEntity is weak to</param>
        public void RemoveWeakness(Elements element, in WeaknessHolder weakness)
        {
            List<WeaknessHolder> weaknessList = null;
            if (Weaknesses.TryGetValue(element, out weaknessList) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not have a weakness for {element}");
                return;
            }

            bool removed = weaknessList.Remove(weakness);
            if (weaknessList.Count == 0)
            {
                Weaknesses.Remove(element);
            }

            if (removed == true)
                Debug.Log($"Removed {weakness.WeaknessType} Weakness to the {element} Element on {Entity.Name}!");
        }

        /// <summary>
        /// Gets this entity's total weakness to a particular Element
        /// </summary>
        /// <param name="element">The Element to test a weakness for</param>
        /// <returns>A copy of the WeaknessHolder associated with the element if found, otherwise default weakness data</returns>
        public WeaknessHolder GetWeakness(Elements element)
        {
            List<WeaknessHolder> weaknesses = null;
            if (Weaknesses.TryGetValue(element, out weaknesses) == false)
            {
                //Debug.Log($"{Name} does not have a weakness for {element}");
                return WeaknessHolder.Default;
            }

            WeaknessHolder weaknessHolder = default(WeaknessHolder);

            //Get the total Weakness
            for (int i = 0; i < weaknesses.Count; i++)
            {
                weaknessHolder.Value += weaknesses[i].Value;
                //Stronger WeaknessTypes are prioritized
                if (weaknesses[i].WeaknessType > weaknessHolder.WeaknessType)
                    weaknessHolder.WeaknessType = weaknesses[i].WeaknessType;
            }

            return weaknessHolder;
        }

        /// <summary>
        /// Tells if the BattleEntity has a Weakness to a particular Element
        /// </summary>
        /// <param name="element">The Element</param>
        /// <returns>true if the BattleEntity has a Weakness to the Element, false otherwise</returns>
        public bool HasWeakness(Elements element)
        {
            return Weaknesses.ContainsKey(element);
        }

        #endregion

        #region Resistance Methods

        ///<summary>
        ///Adds a Resistance on the BattleEntity
        ///</summary>
        ///<param name="element">The element the BattleEntity is resistant to</param>
        ///<param name="resistanceHolder">The data for the Resistance</param>
        public void AddResistance(Elements element, in ResistanceHolder resistanceHolder)
        {
            List<ResistanceHolder> resistanceList = null;
            if (Resistances.TryGetValue(element, out resistanceList) == false)
            {
                resistanceList = new List<ResistanceHolder>();
                Resistances.Add(element, resistanceList);
            }

            resistanceList.Add(resistanceHolder);
            Debug.Log($"Added {resistanceHolder.ResistanceType} Resistance to {Entity.Name} for the {element} Element!");
        }

        /// <summary>
        /// Removes a Resistance on the BattleEntity
        /// </summary>
        /// <param name="element">The Element the BattleEntity is resistant to</param>
        public void RemoveResistance(Elements element, in ResistanceHolder resistanceHolder)
        {
            List<ResistanceHolder> resistanceList = null;
            if (Resistances.TryGetValue(element, out resistanceList) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not have a resistance for {element}");
                return;
            }

            bool removed = resistanceList.Remove(resistanceHolder);
            if (resistanceList.Count == 0)
            {
                Resistances.Remove(element);
            }

            if (removed == true)
                Debug.Log($"Removed {resistanceHolder.ResistanceType} Resistance to the {element} Element on {Entity.Name}!");
        }

        /// <summary>
        /// Gets this entity's total resistance to a particular Element
        /// </summary>
        /// <param name="element">The element to test a resistance towards</param>
        /// <returns>A copy of the ResistanceHolder associated with the element if found, otherwise default resistance data</returns>
        public ResistanceHolder GetResistance(Elements element)
        {
            List<ResistanceHolder> resistances = null;

            if (Resistances.TryGetValue(element, out resistances) == false)
            {
                //Debug.Log($"{Entity.Name} does not have a resistance for {element}");
                return ResistanceHolder.Default;
            }

            ResistanceHolder resistanceHolder = default(ResistanceHolder);

            //Get the total resistance
            for (int i = 0; i < resistances.Count; i++)
            {
                resistanceHolder.Value += resistances[i].Value;
                //Stronger ResistanceTypes are prioritized
                if (resistances[i].ResistanceType > resistanceHolder.ResistanceType)
                    resistanceHolder.ResistanceType = resistances[i].ResistanceType;
            }

            return resistanceHolder;
        }

        /// <summary>
        /// Tells if the BattleEntity has a Resistance to a particular Element
        /// </summary>
        /// <param name="element">The Element</param>
        /// <returns>true if the BattleEntity has a Resistance to the Element, false otherwise</returns>
        public bool HasResistance(Elements element)
        {
            return Resistances.ContainsKey(element);
        }

        #endregion

        #region Strength Methods

        ///<summary>
        ///Adds a Strength to the BattleEntity.
        ///</summary>
        ///<param name="physAttribute">The PhysicalAttribute the BattleEntity is strong against.</param>
        ///<param name="strengthHolder">The data for the Strength.</param>
        public void AddStrength(PhysicalAttributes physAttribute, in StrengthHolder strengthHolder)
        {
            List<StrengthHolder> strengthList = null;
            if (Strengths.TryGetValue(physAttribute, out strengthList) == false)
            {
                strengthList = new List<StrengthHolder>();
                Strengths.Add(physAttribute, strengthList);
            }

            strengthList.Add(strengthHolder);
            Debug.Log($"Added strength value of {strengthHolder.Value} to {Entity.Name} for the {physAttribute} PhysicalAttribute!");
        }

        /// <summary>
        /// Removes a Strength from the BattleEntity.
        /// </summary>
        /// <param name="physAttribute">The PhysicalAttribute the BattleEntity is strong against.</param>
        public void RemoveStrength(PhysicalAttributes physAttribute, in StrengthHolder strengthHolder)
        {
            List<StrengthHolder> strengthList = null;
            if (Strengths.TryGetValue(physAttribute, out strengthList) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not have a strength for {physAttribute}");
                return;
            }

            bool removed = strengthList.Remove(strengthHolder);
            if (strengthList.Count == 0)
            {
                Strengths.Remove(physAttribute);
            }

            if (removed == true)
                Debug.Log($"Removed strength value of {strengthHolder.Value} from the {physAttribute} PhysicalAttribute on {Entity.Name}!");
        }

        /// <summary>
        /// Gets this entity's total strength to a particular PhysicalAttribute.
        /// </summary>
        /// <param name="physAttribute">The PhysicalAttribute to test a strength towards.</param>
        /// <returns>A copy of the StrengthHolder associated with the element if found, otherwise default strength data.</returns>
        private StrengthHolder GetStrength(PhysicalAttributes physAttribute)
        {
            List<StrengthHolder> strengths = null;

            if (Strengths.TryGetValue(physAttribute, out strengths) == false)
            {
                //Debug.Log($"{Entity.Name} does not have a strength for {physAttribute}");
                return StrengthHolder.Default;
            }

            StrengthHolder strengthHolder = default(StrengthHolder);

            //Get the total strength
            for (int i = 0; i < strengths.Count; i++)
            {
                strengthHolder.Value += strengths[i].Value;
            }

            return strengthHolder;
        }

        /// <summary>
        /// Tells if the BattleEntity has a Strength to a particular PhysicalAttribute.
        /// </summary>
        /// <param name="physAttribute">The PhysicalAttribute.</param>
        /// <returns>true if the BattleEntity has a Strength to the PhysicalAttribute, false otherwise.</returns>
        public bool HasStrength(PhysicalAttributes physAttribute)
        {
            return Strengths.ContainsKey(physAttribute);
        }

        /// <summary>
        /// Retrieves the total Strength this BattleEntity has against all PhysicalAttributes found on a victim.
        /// </summary>
        /// <param name="attacker">The BattleEntity this one is attacking.</param>
        /// <returns>A StrengthHolder with information about how much additional damage this BattleEntity will do to the victim.</returns>
        public StrengthHolder GetTotalStrength(BattleEntity victim)
        {
            StrengthHolder totalStrength = StrengthHolder.Default;
            PhysicalAttributes[] victimAttributes = victim.EntityProperties.GetAllPhysAttributes();

            for (int i = 0; i < victimAttributes.Length; i++)
            {
                StrengthHolder attributeStrength = GetStrength(victimAttributes[i]);
                totalStrength.Value += attributeStrength.Value;
            }

            return totalStrength;
        }

        #endregion

        #region Status Effect Methods

        /// <summary>
        /// Attempts to afflict the entity with a StatusEffect based on its properties and status percentage for the StatusEffect.
        /// </summary>
        /// <param name="inflictionChance">The chance of inflicting the StatusEffect.</param>
        /// <param name="status">The StatusEffect to afflict the entity with.</param>
        /// <returns>true if the StatusEffect was successfully afflicted, false otherwise.</returns>
        public bool TryAfflictStatus(double inflictionChance, StatusEffect status)
        {
            StatusPropertyHolder statusProperty = GetStatusProperty(status.StatusType);
            //If the entity is immune to this particular StatusEffect, don't allow it to be inflicted
            if (statusProperty.IsImmune == true)
            {
                Debug.Log($"{Entity.Name} is immune to {status.StatusType}!");
                return false;
            }

            //Test the percentage
            double percentage = statusProperty.StatusPercentage;
            return UtilityGlobals.TestRandomCondition(inflictionChance, percentage);
        }

        /// <summary>
        /// Directly afflicts the entity with a StatusEffect
        /// </summary>
        /// <param name="status">The StatusEffect to afflict the entity with</param>
        public void AfflictStatus(StatusEffect status)
        {
            StatusEffect refreshedStatus = GetStatus(status.StatusType);

            //If the entity already has this StatusEffect, refresh its properties with the new properties.
            //By default, the duration is refreshed.
            //We don't remove the status then reafflict it because that would end it. With a status like Frozen,
            //it would deal damage to the entity when being removed and we don't want that
            if (refreshedStatus != null)
            {
                refreshedStatus.Refresh(status);

                //Invoke the status inflicted event, passing in the status inflicted with
                StatusInflictedEvent?.Invoke(status);

                string refreshedTurnMessage = refreshedStatus.Duration.ToString();
                if (refreshedStatus.IsInfinite == true) refreshedTurnMessage = "Infinite";
                Debug.Log($"{status.StatusType} Status on {Entity.Name} was refreshed with a duration of {refreshedTurnMessage}!");
                return;
            }

            //Copy the StatusEffect
            StatusEffect newStatus = status.Copy();

            //Add the status then afflict it
            Statuses.Add(newStatus.StatusType, newStatus);
            newStatus.SetEntity(Entity);
            newStatus.Afflict();

            //Invoke the status inflicted event, passing in the status inflicted with
            StatusInflictedEvent?.Invoke(status);

            string turnMessage = newStatus.TotalDuration.ToString();
            if (newStatus.IsInfinite == true) turnMessage = "Infinite";
            Debug.LogWarning($"Afflicted {Entity.Name} with the {newStatus.StatusType} Status for {turnMessage} turns!");
        }

        /// <summary>
        /// Ends and removes a StatusEffect on the entity
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to remove</param>
        /// <param name="showMessage">Whether to show the StatusEffect's RemovedMessage as a Battle Message when it is removed.</param>
        /// <returns>The StatusEffect removed from the BattleEntity.</returns>
        public StatusEffect RemoveStatus(StatusTypes statusType)
        {
            StatusEffect status = GetStatus(statusType);
            
            //Don't do anything if the entity doesn't have this status
            if (status == null)
            {
                Debug.Log($"{Entity.Name} is not currently afflicted with the {statusType} Status!");
                return null;
            }

            //End the status then remove it
            status.End();
            status.ClearEntity();
            Statuses.Remove(statusType);

            //Invoke the status removed event
            StatusRemovedEvent?.Invoke(status);

            string turnMessage = status.TotalDuration.ToString();
            if (status.IsInfinite == true) turnMessage = "Infinite";
            Debug.LogWarning($"Removed the {statusType} Status on {Entity.Name} which was inflicted for {turnMessage} turns!");

            return status;
        }

        /// <summary>
        /// Ends and removes all StatusEffects on the BattleEntity.
        /// </summary>
        public void RemoveAllStatuses()
        {
            StatusEffect[] statusEffects = GetStatuses();
            for (int i = 0; i < statusEffects.Length; i++)
            {
                RemoveStatus(statusEffects[i].StatusType);
            }
        }

        /// <summary>
        /// Tells if the entity is currently afflicted with a particular StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to check</param>
        /// <returns>true if the entity is afflicted with the StatusEffect, otherwise false</returns>
        public bool HasStatus(StatusTypes statusType)
        {
            return Statuses.ContainsKey(statusType);
        }

        /// <summary>
        /// Retrieves a specific StatusEffect. This method is internal.
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to get.</param>
        /// <returns>null if the entity isn't afflicted with the StatusEffect, otherwise the StatusEffect it's afflicted with.</returns>
        private StatusEffect GetStatus(StatusTypes statusType)
        {
            StatusEffect status = null;
            Statuses.TryGetValue(statusType, out status);

            return status;
        }

        /// <summary>
        /// Returns all StatusEffects the entity is afflicted with, in order of Priority.
        /// </summary>
        /// <returns>An array of StatusEffects in order of Priority. If no StatusEffects are on the entity, it'll return an empty array.</returns>
        public StatusEffect[] GetStatuses()
        {
            if (Statuses.Values.Count == 0) return Array.Empty<StatusEffect>();

            return Statuses.Values.ToArray();
        }

        /// <summary>
        /// Adds all StatusEffects the entity is afflicted with, in order of Priority, into a supplied list.
        /// </summary>
        /// <param name="statusList">The List to add the StatusEffects to.</param>
        public void GetStatuses(List<StatusEffect> statusList)
        {
            foreach (StatusEffect status in Statuses.Values)
            {
                statusList.Add(status);
            }
        }

        /// <summary>
        /// Suppresses a set of Status Effects in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the Status Effects should be suppressed.</param>
        /// <param name="statusTypes">The StatusTypes of the Status Effects to suppress.</param>
        public void SuppressStatuses(StatusSuppressionTypes statusSuppressionType, params StatusTypes[] statusTypes)
        {
            //Return if there are no statuses to suppress
            if (statusTypes == null || statusTypes.Length == 0)
            {
                Debug.LogError($"{nameof(statusTypes)} is null or empty!");
                return;
            }

            //Go through all the statuses and suppress them
            for (int i = 0; i < statusTypes.Length; i++)
            {
                StatusEffect statusEffect = GetStatus(statusTypes[i]);
                if (statusEffect != null)
                {
                    statusEffect.Suppress(statusSuppressionType);
                }
                else
                {
                    Debug.LogWarning($"Could not find Status {statusTypes[i]} on {Entity.Name} to suppress!");
                }
            }
        }

        /// <summary>
        /// Unsuppresses a set of Status Effects in a particular way.
        /// </summary>
        /// <param name="statusSuppressionType">The StatusSuppressionTypes of how the Status Effects should be unsuppressed.</param>
        /// <param name="statusTypes">The StatusTypes of the Status Effects to unsuppress.</param>
        public void UnsuppressStatuses(StatusSuppressionTypes statusSuppressionType, params StatusTypes[] statusTypes)
        {
            //Return if there are no statuses to unsuppress
            if (statusTypes == null || statusTypes.Length == 0)
            {
                Debug.LogError($"{nameof(statusTypes)} is null or empty!");
                return;
            }

            //Go through all the statuses and unsuppress them
            for (int i = 0; i < statusTypes.Length; i++)
            {
                StatusEffect statusEffect = GetStatus(statusTypes[i]);
                if (statusEffect != null)
                {
                    statusEffect.Unsuppress(statusSuppressionType);
                }
                else
                {
                    Debug.LogWarning($"Could not find Status {statusTypes[i]} on {Entity.Name} to unsuppress!");
                }
            }
        }

        #endregion

        #region Status Property Methods

        /// <summary>
        /// Adds a StatusProperty for a particular StatusEffect to the BattleEntity.
        /// If a StatusProperty already exists for a StatusEffect, it will be replaced.
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect.</param>
        /// <param name="statusProperty">The StatusPropertyHolder associated with the StatusEffect.</param>
        public void AddStatusProperty(StatusTypes statusType, in StatusPropertyHolder statusProperty)
        {
            if (HasStatusProperty(statusType) == true)
            {
                Debug.Log($"Replacing {nameof(StatusPropertyHolder)} for the {statusType} Status as {Entity.Name} already has one!");
                StatusProperties.Remove(statusType);
            }

            StatusProperties.Add(statusType, statusProperty);
        }

        /// <summary>
        /// Removes a StatusProperty for a particular StatusEffect from the BattleEntity.
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect.</param>
        public void RemoveStatusProperty(StatusTypes statusType)
        {
            if (HasStatusProperty(statusType) == false)
            {
                Debug.LogWarning($"{Entity.Name} doesn't have a {nameof(StatusPropertyHolder)} for the {statusType} Status!");
                return;
            }

            StatusProperties.Remove(statusType);
        }

        /// <summary>
        /// Tells if the BattleEntity has a StatusPropertyHolder associated with a particular StatusEffect or not
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <returns>true if a StatusPropertyHolder can be found for the specified StatusType, false otherwise</returns>
        public bool HasStatusProperty(StatusTypes statusType)
        {
            return StatusProperties.ContainsKey(statusType);
        }

        /// <summary>
        /// Retrieves the StatusPropertyHolder associated with a particular StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect</param>
        /// <returns>The StatusPropertyHolder corresponding to the specified StatusType. If there is no entry, returns a default one</returns>
        public StatusPropertyHolder GetStatusProperty(StatusTypes statusType)
        {
            StatusPropertyHolder statusProperty;
            if (StatusProperties.TryGetValue(statusType, out statusProperty) == false)
            {
                return StatusPropertyHolder.Default;
            }

            return statusProperty;
        }

        /// <summary>
        /// Copies StatusProperties from a <see cref="BattleEntityProperties"/> to this one.
        /// This removes all existing StatusProperties before copying.
        /// </summary>
        /// <param name="entityProperties">The BattleEntityProperties to copy StatusProperties from.</param>
        public void CopyStatusProperties(in BattleEntityProperties entityProperties)
        {
            //Don't do anything if null was passed in
            if (entityProperties == null)
            {
                Debug.LogError($"{nameof(entityProperties)} is null in {nameof(CopyStatusProperties)}! Fix this!!");
                return;
            }

            //Remove all Status Properties first
            RemoveAllStatusProperties();

            //Copy the data from the other StatusProperties
            StatusProperties.CopyDictionaryData(entityProperties.StatusProperties);
        }

        /// <summary>
        /// Removes all StatusProperties. This is for internal use.
        /// </summary>
        private void RemoveAllStatusProperties()
        {
            StatusProperties.Clear();
        }

        #endregion

        #region Damage Effects Methods

        /// <summary>
        /// Sets the BattleEntity's vulnerability to DamageEffects.
        /// </summary>
        /// <param name="damageEffects">The DamageEffects value to set. This is a bit field.</param>
        public void SetVulnerableDamageEffects(DamageEffects damageEffects)
        {
            VulnerableDamageEffects = damageEffects;
        }

        /// <summary>
        /// Returns the BattleEntity's vulnerability to DamageEffects.
        /// </summary>
        /// <returns>A DamageEffects value.</returns>
        public DamageEffects GetVulnerableDamageEffects()
        {
            return VulnerableDamageEffects;
        }

        /// <summary>
        /// Tells whether the BattleEntity is vulnerable to any DamageEffect in a set of DamageEffects.
        /// </summary>
        /// <param name="damageEffects">The DamageEffects to check vulnerability to.</param>
        /// <returns>true if the BattleEntity is vulnerable to any of the DamageEffects, otherwise false.</returns>
        public bool IsVulnerableToDamageEffect(DamageEffects damageEffects)
        {
            return UtilityGlobals.DamageEffectHasFlag(VulnerableDamageEffects, damageEffects);
        }

        /// <summary>
        /// Tells if the BattleEntity is vulnerable to any DamageEffects.
        /// </summary>
        /// <returns>true if the BattleEntity is vulnerable to any DamageEffect, otherwise false.</returns>
        public bool HasDamageEffectVulnerabilities() => (VulnerableDamageEffects != DamageEffects.None);

        #endregion

        #region Payback Data Methods

        /// <summary>
        /// Adds Payback to the BattleEntity, causing it to deal damage to direct attackers.
        /// </summary>
        /// <param name="paybackHolder">The PaybackHolder to add.</param>
        public void AddPayback(in PaybackHolder paybackHolder)
        {
            Debug.Log($"Added {paybackHolder.Element} Payback of type {paybackHolder.PaybackType} to {Entity.Name}!");

            Paybacks.Add(paybackHolder);
        }

        /// <summary>
        /// Removes a Payback on the BattleEntity.
        /// </summary>
        /// <param name="paybackHolder">The PaybackHolder to remove.</param>
        public void RemovePayback(in PaybackHolder paybackHolder)
        {
            bool removed = Paybacks.Remove(paybackHolder);

            if (removed == true)
            {
                Debug.Log($"Successfully removed {paybackHolder.Element} Payback of type {paybackHolder.PaybackType} on {Entity.Name}!");
            }
        }

        /// <summary>
        /// Gets all of the BattleEntity's individual Paybacks.
        /// </summary>
        /// <returns>An array of PaybackHolders with the Paybacks the BattleEntity has.</returns>
        public PaybackHolder[] GetAllPaybacks()
        {
            if (Paybacks.Count == 0) return Array.Empty<PaybackHolder>();

            return Paybacks.ToArray();
        }

        /// <summary>
        /// Adds all of the BattleEntity's individual Paybacks to a supplied list.
        /// </summary>
        /// <param name="paybackList">The list of Paybacks to add to.</param>
        public void GetAllPaybacks(List<PaybackHolder> paybackList)
        {
            paybackList.CopyFromList(Paybacks);
        }

        /// <summary>
        /// Gets the total Payback a BattleEntity has by combining all of the current Paybacks affecting the BattleEntity.
        /// </summary>
        /// <param name="additionalPaybacks">Any additional PaybackHolders to factor in.</param>
        /// <returns>A PaybackHolder with the combined properties of all the Paybacks the BattleEntity has</returns>
        public PaybackHolder GetPayback(params PaybackHolder[] additionalPaybacks)
        {
            //Gather all the entity's Paybacks in the list
            List<PaybackHolder> allPaybacks = Paybacks;

            //Add any additional Paybacks
            if (additionalPaybacks != null && additionalPaybacks.Length > 0)
            {
                allPaybacks = new List<PaybackHolder>(Paybacks);
                allPaybacks.AddRange(additionalPaybacks);
            }

            return PaybackHolder.CombinePaybacks(allPaybacks);
        }

        /// <summary>
        /// Tells if the BattleEntity has any Payback at all
        /// </summary>
        /// <returns>true if the Payback list has at least one entry, otherwise false</returns>
        public bool HasPayback()
        {
            return (Paybacks.Count > 0);
        }

        #endregion

        #region Additional Property Methods

        /// <summary>
        /// Adds an AdditionalProperty to the entity.
        /// If it already has the property, it replaces its value with the new value.
        /// </summary>
        /// <param name="property">The AdditionalProperty to add.</param>
        /// <param name="value">An object of the value corresponding to the AdditionalProperty.</param>
        public void AddAdditionalProperty(AdditionalProperty property, in object value)
        {
            //Remove if the entity already has it
            if (HasAdditionalProperty(property) == true)
            {
                RemoveAdditionalProperty(property);
            }

            AdditionalProperties.Add(property, value);
            Debug.Log($"Added the {property} property to {Entity.Name} with a value of {value}!");
        }

        /// <summary>
        /// Removes an AdditionalProperty from the entity
        /// </summary>
        /// <param name="property">The AdditionalProperty to remove.</param>
        public void RemoveAdditionalProperty(AdditionalProperty property)
        {
            bool removed = AdditionalProperties.Remove(property);

            if (removed == true)
            {
                Debug.Log($"Removed the {property} property on {Entity.Name}!");
            }
        }

        /// <summary>
        /// Checks if the entity has an AdditionalProperty.
        /// </summary>
        /// <param name="property">The AdditionalProperty to check.</param>
        /// <returns>true if the entity has the AdditionalProperty, otherwise false</returns>
        public bool HasAdditionalProperty(AdditionalProperty property)
        {
            return AdditionalProperties.ContainsKey(property);
        }

        /// <summary>
        /// Gets the value of an AdditionalProperty the entity has.
        /// </summary>
        /// <typeparam name="T">The type of property to get.</typeparam>
        /// <param name="property">The AdditionalProperty to get the value for.</param>
        /// <returns>The value corresponding to the property passed in. If no value was found, returns the default value of type T.</returns>
        public T GetAdditionalProperty<T>(AdditionalProperty property)
        {
            if (AdditionalProperties.TryGetValue(property, out object val) == true)
            {
                return (T)val;
            }

            return default(T);
        }

        #endregion

        #region Move Category Methods

        /// <summary>
        /// Disables a particular MoveCategory from being used by the entity.
        /// </summary>
        /// <param name="category">The type of moves to disable.</param>
        public void DisableMoveCategory(MoveCategories category)
        {
            if (IsMoveCategoryDisabled(category) == false)
            {
                DisabledMoveCategories.Add(category, 0);
            }

            DisabledMoveCategories[category]++;

            Debug.Log($"Disabled {category} moves from use for {Entity.Name}. Total disables: {DisabledMoveCategories[category]}");
        }

        /// <summary>
        /// Clears a particular MoveCategory from being disabled.
        /// </summary>
        /// <param name="category">The type of moves to enable.</param>
        public void EnableMoveCategory(MoveCategories category)
        {
            int newVal = -1;
            if (DisabledMoveCategories.TryGetValue(category, out newVal) == false)
            {
                Debug.LogWarning($"{category} moves are not currently disabled for {Entity.Name}!");
                return;
            }

            newVal -= 1;
            
            if (newVal <= 0)
            {
                DisabledMoveCategories.Remove(category);

                Debug.Log($"Enabled {category} moves for {Entity.Name} to use once again");
            }
            else
            {
                DisabledMoveCategories[category] = newVal;

                Debug.Log($"Enabled {category} moves for {Entity.Name}. Total disables remaining: {newVal}");
            }
        }

        /// <summary>
        /// Tells whether a particular MoveCategory is disabled for this entity.
        /// </summary>
        /// <param name="category">The type of moves to check.</param>
        /// <returns>true if the category is in the disabled dictionary, otherwise false.</returns>
        public bool IsMoveCategoryDisabled(MoveCategories category)
        {
            return DisabledMoveCategories.ContainsKey(category);
        }

        /// <summary>
        /// Gets all of the entity's currently disabled MoveCategories.
        /// </summary>
        /// <returns>An array of MoveCategories that are disabled. If none are disabled, an empty array is returned.</returns>
        public MoveCategories[] GetDisabledMoveCategories()
        {
            if (DisabledMoveCategories.Keys.Count == 0) return Array.Empty<MoveCategories>();

            return DisabledMoveCategories.Keys.ToArray();
        }

        /// <summary>
        /// Gets all of the entity's currently disabled MoveCategories.
        /// </summary>
        /// <returns>An array of MoveCategories that are disabled. If none are disabled, an empty array is returned.</returns>
        public void GetDisabledMoveCategories(List<MoveCategories> moveCategoryList)
        {
            foreach (MoveCategories moveCategory in DisabledMoveCategories.Keys)
            {
                moveCategoryList.Add(moveCategory);
            }
        }

        #endregion

        #region Badge Methods

        /// <summary>
        /// Adds a Badge to a BattleEntity. This is called from the Badge's Equip methods.
        /// </summary>
        /// <param name="badge">The Badge to equip.</param>
        public void AddEquippedBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError("Can't add a null Badge!");
                return;
            }

            //Add the Badge
            EquippedBadges.Add(badge);

            //Add a new entry if it doesn't exist
            if (EquippedBadgeCounts.ContainsKey(badge.BadgeType) == false)
            {
                EquippedBadgeCounts.Add(badge.BadgeType, 0);
            }

            //Increment the number of Badges of this type equipped
            EquippedBadgeCounts[badge.BadgeType]++;
        }

        /// <summary>
        /// Removes a Badge from a BattleEntity. This is called from the Badge's UnEquip methods.
        /// </summary>
        /// <param name="badge">The Badge to unequip.</param>
        public void RemoveEquippedBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError("Can't remove a null Badge!");
                return;
            }

            //Remove the Badge
            bool removed = EquippedBadges.Remove(badge);

            //Check if we removed the Badge from our equipped list
            if (removed == true)
            {
                //Update the Badge count by subtracting one
                int newBadgeCount = -1;
                if (EquippedBadgeCounts.TryGetValue(badge.BadgeType, out newBadgeCount) == true)
                {
                    newBadgeCount -= 1;

                    //If no more Badges of this type are equipped, remove the entry
                    if (newBadgeCount <= 0)
                    {
                        EquippedBadgeCounts.Remove(badge.BadgeType);
                    }
                    //Otherwise, simply update the entry
                    else
                    {
                        EquippedBadgeCounts[badge.BadgeType] = newBadgeCount;
                    }
                }
            }
        }

        /// <summary>
        /// Returns all equipped Badges on this BattleEntity in a new array.
        /// </summary>
        /// <returns>All Badges equipped to this BattleEntity in a new array.</returns>
        public Badge[] GetEquippedBadges()
        {
            return EquippedBadges.ToArray();
        }

        /// <summary>
        /// Adds all equipped Badges on this BattleEntity into a supplied list.
        /// </summary>
        /// <param name="badgeList">The list to add the equipped Badges to.</param>
        public void GetEquippedBadges(List<Badge> badgeList)
        {
            badgeList.CopyFromList(EquippedBadges);
        }

        /// <summary>
        /// Returns the number of equipped Badges of a particular BadgeType on the BattleEntity.
        /// </summary>
        /// <param name="badgeType">The BadgeType to get the equipped count for.</param>
        /// <returns>The number of Badges of this BadgeType equipped to the BattleEntity.</returns>
        public int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            int newBadgeCount = 0;
            if (EquippedBadgeCounts.TryGetValue(badgeType, out newBadgeCount) == false) return 0;

            return newBadgeCount;
        }

        #endregion

        #region Custom Target Methods

        /// <summary>
        /// Sets the value of the custom targeting delegate.
        /// </summary>
        /// <param name="customTargeting">The CustomTarget method to use.</param>
        public void SetCustomTargeting(CustomTarget customTargeting)
        {
            CustomTargeting = customTargeting;
        }

        #endregion
    }
}
