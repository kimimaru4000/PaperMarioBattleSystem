using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles numerous BattleEntity properties like weaknesses/resistances to certain Element, and more
    /// </summary>
    public class BattleEntityProperties
    {
        /// <summary>
        /// The BattleEntity these properties are for
        /// </summary>
        protected readonly BattleEntity Entity = null;

        /// <summary>
        /// The physical attributes the entity possesses
        /// </summary>
        protected readonly Dictionary<PhysicalAttributes, int> PhysAttributes = new Dictionary<PhysicalAttributes, int>();

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
        /// The Resistances the entity has
        /// <para>A List is used for the value so additional Resistances can be added via moves or equipment.</para>
        /// </summary>
        protected readonly Dictionary<Elements, List<ResistanceHolder>> Resistances = new Dictionary<Elements, List<ResistanceHolder>>();

        /// <summary>
        /// The damage modifiers used when dealing damage to BattleEntities with particular PhysicalAttributes
        /// </summary>
        protected readonly Dictionary<PhysicalAttributes, int> DamageMods = new Dictionary<PhysicalAttributes, int>();

        /// <summary>
        /// The StatusEffects the entity is afflicted with
        /// </summary>
        protected readonly Dictionary<StatusTypes, StatusEffect> Statuses = new Dictionary<StatusTypes, StatusEffect>();

        /// <summary>
        /// Properties relating to how the entity is affected by each StatusEffect.
        /// Examples include the likelihood of being inflicted by a StatusEffect-inducing attack and
        /// how many additional turns the entity is affected by the StatusEffect
        /// </summary>
        protected readonly Dictionary<StatusTypes, StatusPropertyHolder> StatusProperties = new Dictionary<StatusTypes, StatusPropertyHolder>();

        /// <summary>
        /// Miscellaneous properties of the entity
        /// </summary>
        protected readonly Dictionary<MiscProperty, MiscValueHolder> MiscProperties = new Dictionary<MiscProperty, MiscValueHolder>();

        #region Constructor

        public BattleEntityProperties(BattleEntity entity)
        {
            Entity = entity;
        }

        #endregion

        #region Phys Attribute Methods

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
            if (PhysAttributes.ContainsKey(physicalAttribute) == false)
            {
                Debug.LogWarning($"Cannot remove physical attribute {physicalAttribute} because {Entity.Name} does not have it!");
                return;
            }

            PhysAttributes[physicalAttribute]--;
            if (PhysAttributes[physicalAttribute] <= 0)
            {
                PhysAttributes.Remove(physicalAttribute);
                Debug.Log($"Removed the physical attribute {physicalAttribute} from {Entity.Name}'s existing attributes!");
            }
            else
            {
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
        /// Determines the result of contact, based on the type of contact made, when it's made with this entity
        /// </summary>
        /// <param name="attacker">The entity attacking this one</param>
        /// <param name="contactType">The type of contact made with this entity</param>
        /// <returns>A ContactResultInfo containing the result of the interaction</returns>
        public ContactResultInfo GetContactResult(BattleEntity attacker, ContactTypes contactType)
        {
            return Interactions.GetContactResult(attacker, contactType, GetAllPhysAttributes(), attacker.EntityProperties.GetContactExceptions(contactType));
        }

        /// <summary>
        /// Returns all PhysicalAttributes the BattleEntity has
        /// </summary>
        /// <returns>An array of all PhysicalAttributes the BattleEntity has</returns>
        protected PhysicalAttributes[] GetAllPhysAttributes()
        {
            return PhysAttributes.Keys.ToArray();
        }

        #endregion

        #region Damage Modifier Methods

        /// <summary>
        /// Adds a DamageMod for this BattleEntity against a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the DamageMod</param>
        /// <param name="damageValue">The damage modifier to add</param>
        public void AddDamageMod(PhysicalAttributes attribute, int damageValue)
        {
            //Don't allow 0, as it can mess up the accuracy if it's the first value
            if (damageValue == 0)
            {
                Debug.LogWarning($"Attempting to add a modifier of 0 against the {attribute} PhysicalAttribute for {Entity.Name}!" +
                                   " This isn't supported, as it can break the accuracy of the total modifier");
                return;
            }

            if (HasDamageMod(attribute) == false)
            {
                DamageMods.Add(attribute, 0);
            }

            DamageMods[attribute] += damageValue;
            Debug.Log($"Added damage mod of {damageValue} against the {attribute} PhysicalAttribute for {Entity.Name}");
        }

        /// <summary>
        /// Removes a DamageMod this BattleEntity has against a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the DamageMod</param>
        /// <param name="damageValue">The damage modifier to remove</param>
        public void RemoveDamageMod(PhysicalAttributes attribute, int damageValue)
        {
            if (HasDamageMod(attribute) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not contain a damage modifier for the {attribute} PhysicalAttribute and thus cannot remove one!");
                return;
            }

            DamageMods[attribute] -= damageValue;
            //Remove if the total value is 0
            if (DamageMods[attribute] == 0)
            {
                DamageMods.Remove(attribute);
            }
        }

        /// <summary>
        /// Retrieves the DamageMod associated with a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the DamageMod</param>
        /// <returns>An int for the DamageMod if it exists, otherwise 0</returns>
        public int GetDamageMod(PhysicalAttributes attribute)
        {
            if (HasDamageMod(attribute) == false) return 0;

            return DamageMods[attribute];
        }

        /// <summary>
        /// Tells if the BattleEntity has a DamageMod against a particular PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the DamageMod</param>
        /// <returns>true if the DamageMod exists for the PhysicalAttribute, otherwise false</returns>
        public bool HasDamageMod(PhysicalAttributes attribute)
        {
            return DamageMods.ContainsKey(attribute);
        }

        /// <summary>
        /// Retrieves the total damage modifier another BattleEntity has for this BattleEntity
        /// </summary>
        /// <param name="attacker">The BattleEntity attacking this one</param>
        /// <returns>A sum of the damage modifiers the BattleEntity attacking will have against this BattleEntity</returns>
        public int GetTotalDamageMod(BattleEntity attacker)
        {
            int totalDamageMod = 0;
            PhysicalAttributes[] attributes = GetAllPhysAttributes();
            for (int i = 0; i < attributes.Length; i++)
            {
                if (HasPhysAttributes(true, attributes[i]) == true)
                {
                    totalDamageMod += attacker.EntityProperties.GetDamageMod(attributes[i]);
                }
            }

            return totalDamageMod;
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
            //Add a new key if one doesn't exist
            if (ContactExceptions.ContainsKey(contactType) == false)
            {
                ContactExceptions.Add(contactType, new List<PhysicalAttributes>());
            }

            //Add to the list
            ContactExceptions[contactType].Add(physAttribute);

            Debug.Log($"Added contact exception on {Entity.Name} for the {physAttribute} PhysicalAttribute during {contactType} contact!");
        }

        public void RemoveContactException(ContactTypes contactType, PhysicalAttributes physAttribute)
        {
            if (ContactExceptions.ContainsKey(contactType) == false)
            {
                Debug.LogError($"Cannot remove {physAttribute} from the exception list on {Entity.Name} for {contactType} because no list exists!");
                return;
            }

            bool removed = ContactExceptions[contactType].Remove(physAttribute);
            if (removed == true)
            {
                Debug.Log($"Removed {physAttribute} attribute exception on {Entity.Name} for {contactType} contact!");
            }

            //If there are no PhysicalAttributes in the exceptions list for this ContactType, remove the key
            if (ContactExceptions[contactType].Count == 0)
            {
                ContactExceptions.Remove(contactType);
            }
        }

        /// <summary>
        /// Returns a set of PhysicalAttributes to ignore when the BattleEntity makes contact
        /// </summary>
        /// <param name="contactType">The type of contact this BattleEntity made</param>
        /// <returns>An array of PhysicalAttributes this BattleEntity can ignore when making contact, otherwise an empty array</returns>
        public PhysicalAttributes[] GetContactExceptions(ContactTypes contactType)
        {
            //Return an empty array if no exceptions exist for this type of contact
            if (ContactExceptions.ContainsKey(contactType) == false)
            {
                return new PhysicalAttributes[0];
            }

            return ContactExceptions[contactType].ToArray();
        }

        #endregion

        #region Weakness Methods

        /// <summary>
        /// Adds a Weakness on the BattleEntity
        /// </summary>
        /// <param name="element">The Element the BattleEntity is weak to</param>
        /// <param name="weaknessHolder">The data for the Weakness</param>
        public void AddWeakness(Elements element, WeaknessHolder weaknessHolder)
        {
            if (HasWeakness(element) == false)
            {
                Weaknesses.Add(element, new List<WeaknessHolder>());
            }

            Weaknesses[element].Add(weaknessHolder);
            Debug.Log($"Added {weaknessHolder.WeaknessType} Weakness to {Entity.Name} for the {element} Element!");
        }

        /// <summary>
        /// Removes a Weakness on the BattleEntity
        /// </summary>
        /// <param name="element">The Element the BattleEntity is weak to</param>
        public void RemoveWeakness(Elements element, WeaknessHolder weakness)
        {
            if (HasWeakness(element) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not have a weakness for {element}");
                return;
            }

            bool removed = Weaknesses[element].Remove(weakness);
            if (Weaknesses[element].Count == 0)
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
            if (HasWeakness(element) == false)
            {
                //Debug.Log($"{Name} does not have a weakness for {element}");
                return WeaknessHolder.Default;
            }

            WeaknessHolder weaknessHolder = default(WeaknessHolder);

            //Get the total Weakness
            Weaknesses[element].ForEach((weakness) =>
            {
                weaknessHolder.Value += weakness.Value;
                //Stronger WeaknessTypes are prioritized
                if (weakness.WeaknessType > weaknessHolder.WeaknessType)
                    weaknessHolder.WeaknessType = weakness.WeaknessType;
            });

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
        ///Adds a Weakness on the BattleEntity
        ///</summary>
        ///<param name="element">The element the BattleEntity is resistant to</param>
        ///<param name="resistanceHolder">The data for the Resistance</param>
        public void AddResistance(Elements element, ResistanceHolder resistanceHolder)
        {
            if (HasResistance(element) == false)
            {
                Resistances.Add(element, new List<ResistanceHolder>());
            }

            Resistances[element].Add(resistanceHolder);
            Debug.Log($"Added {resistanceHolder.ResistanceType} Resistance to {Entity.Name} for the {element} Element!");
        }

        /// <summary>
        /// Removes a Resistance on the BattleEntity
        /// </summary>
        /// <param name="element">The Element the BattleEntity is resistant to</param>
        public void RemoveResistance(Elements element, ResistanceHolder resistanceHolder)
        {
            if (HasResistance(element) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not have a resistance for {element}");
                return;
            }

            bool removed = Resistances[element].Remove(resistanceHolder);
            if (Resistances[element].Count == 0)
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
            if (HasResistance(element) == false)
            {
                //Debug.Log($"{Entity.Name} does not have a resistance for {element}");
                return ResistanceHolder.Default;
            }

            ResistanceHolder resistanceHolder = default(ResistanceHolder);

            //Get the total resistance
            Resistances[element].ForEach((resistance) =>
            {
                resistanceHolder.Value += resistance.Value;
                //Stronger ResistanceTypes are prioritized
                if (resistance.ResistanceType > resistanceHolder.ResistanceType)
                    resistanceHolder.ResistanceType = resistance.ResistanceType;
            });

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

        #region Status Effect Methods

        /// <summary>
        /// Attempts to afflict the entity with a StatusEffect, based on its properties and status percentage for the StatusEffect
        /// </summary>
        /// <param name="status">The StatusEffect to afflict the entity with</param>
        /// <returns>true if the StatusEffect was successfully afflicted, false otherwise</returns>
        public bool TryAfflictStatus(StatusEffect status)
        {
            //Test for StatusEffect immunity - if the entity is immune to a particular alignment, don't allow the StatusEffect to be inflicted
            bool positiveStatusImmune = GetMiscProperty(MiscProperty.PositiveStatusImmune).BoolValue;
            bool negativeStatusImmune = GetMiscProperty(MiscProperty.NegativeStatusImmune).BoolValue;
            if ((status.Alignment == StatusEffect.StatusAlignments.Positive && positiveStatusImmune == true)
                || (status.Alignment == StatusEffect.StatusAlignments.Negative && negativeStatusImmune == true))
            {
                return false;
            }

            StatusPropertyHolder statusProperty = GetStatusProperty(status.StatusType);

            //Test the percentage
            int percentage = statusProperty.StatusPercentage;
            int valueTest = GeneralGlobals.Randomizer.Next(1, 101);

            return (valueTest <= percentage);
        }

        /// <summary>
        /// Directly afflicts the entity with a StatusEffect
        /// </summary>
        /// <param name="status">The StatusEffect to afflict the entity with</param>
        public void AfflictStatus(StatusEffect status)
        {
            //Don't do anything if the entity already has this StatusEffect
            if (HasStatus(status.StatusType) == true)
            {
                Debug.Log($"{Entity.Name} is already afflicted with the {status.StatusType} Status!");
                return;
            }

            StatusEffect newStatus = status.Copy();

            //Add the status then afflict it
            Statuses.Add(newStatus.StatusType, newStatus);
            newStatus.SetEntity(Entity);
            newStatus.Afflict();

            Debug.LogWarning($"Afflicted {Entity.Name} with the {newStatus.StatusType} Status for {newStatus.TotalDuration} turns!");
        }

        /// <summary>
        /// Ends and removes a StatusEffect on the entity
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to remove</param>
        public void RemoveStatus(StatusTypes statusType)
        {
            //Don't do anything if the entity doesn't have this status
            if (HasStatus(statusType) == false)
            {
                Debug.Log($"{Entity.Name} is not currently afflicted with the {statusType} Status!");
                return;
            }

            StatusEffect status = Statuses[statusType];

            //End the status then remove it
            status.End();
            status.ClearEntity();
            Statuses.Remove(statusType);

            Debug.LogWarning($"Removed the {statusType} Status on {Entity.Name} after being inflicted for {status.TotalDuration} turns!");
        }

        /// <summary>
        /// Ends and removes all StatusEffects on the entity
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
        /// Returns all StatusEffects the entity is afflicted with, sorted by their Priority
        /// </summary>
        /// <returns>An array of StatusEffects sorted by their Priority. If no StatusEffects are on the entity, it'll return an empty array</returns>
        public StatusEffect[] GetStatuses()
        {
            //Get the values in a list, then sort them
            List<StatusEffect> statusList = Statuses.Values.ToList();
            statusList.Sort(StatusEffect.StatusPrioritySort);

            return statusList.ToArray();
        }

        #endregion

        #region Status Property Methods

        /// <summary>
        /// Adds a StatusProperty for a particular StatusEffect to the entity.
        /// If a StatusProperty already exists for a StatusEffect, it will be replaced.
        /// </summary>
        /// <param name="statusType">The StatusType of the StatusEffect.</param>
        /// <param name="statusProperty">The StatusPropertyHolder associated with the StatusEffect.</param>
        public void AddStatusProperty(StatusTypes statusType, StatusPropertyHolder statusProperty)
        {
            if (StatusProperties.ContainsKey(statusType) == true)
            {
                Debug.Log($"Replacing {nameof(StatusPropertyHolder)} for the {statusType} Status as {Entity.Name} already has one!");
                StatusProperties.Remove(statusType);
            }

            StatusProperties.Add(statusType, statusProperty);
        }

        /// <summary>
        /// Tells if the entity has a StatusPropertyHolder associated with a particular StatusEffect or not
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
            if (HasStatusProperty(statusType) == false)
            {
                return StatusPropertyHolder.Default;
            }

            return StatusProperties[statusType];
        }

        #endregion

        #region Misc Property Methods

        /// <summary>
        /// Adds a MiscProperty to the entity if it doesn't already have it
        /// </summary>
        /// <param name="property">The MiscProperty to add</param>
        /// <param name="value">The value of the MiscProperty</param>
        public void AddMiscProperty(MiscProperty property, MiscValueHolder value)
        {
            //Return if the entity already has it
            if (HasMiscProperty(property) == true)
            {
                Debug.LogWarning($"{Entity.Name} already has the {property} property!");
                return;
            }

            MiscProperties.Add(property, value);
            Debug.Log($"Added the {property} property to {Entity.Name}!");
        }

        /// <summary>
        /// Removes a MiscProperty from the entity
        /// </summary>
        /// <param name="property">The MiscProperty to remove</param>
        public void RemoveMiscProperty(MiscProperty property)
        {
            if (HasMiscProperty(property) == true)
            {
                Debug.Log($"Removed the {property} property on {Entity.Name}!");
            }

            MiscProperties.Remove(property);
        }

        /// <summary>
        /// Checks if the entity has a MiscProperty
        /// </summary>
        /// <param name="property">The MiscProperty to check</param>
        /// <returns>true if the entity has the MiscProperty, otherwise false</returns>
        public bool HasMiscProperty(MiscProperty property)
        {
            return MiscProperties.ContainsKey(property);
        }

        /// <summary>
        /// Gets the value of a MiscProperty the entity has.
        /// </summary>
        /// <param name="property">The MiscProperty to get the value for</param>
        /// <returns>A MiscValueHolder corresponding to the MiscProperty if it has an entry, otherwise a default MiscValueHolder</returns>
        public MiscValueHolder GetMiscProperty(MiscProperty property)
        {
            if (HasMiscProperty(property) == false)
            {
                return new MiscValueHolder();
            }

            return MiscProperties[property];
        }

        #endregion
    }
}
