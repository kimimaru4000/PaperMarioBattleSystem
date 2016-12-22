using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.StatusGlobals;

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
        /// The types of Elemental damage to use when dealing damage to BattleEntities with particular PhysicalAttributes
        /// </summary>
        protected readonly Dictionary<PhysicalAttributes, Elements> ElementOverrides = new Dictionary<PhysicalAttributes, Elements>();

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
        protected readonly Dictionary<MoveCategories, bool> DisabledMoveCategories = new Dictionary<MoveCategories, bool>();

        #region Constructor

        public BattleEntityProperties(BattleEntity entity)
        {
            Entity = entity;
        }

        #endregion

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
        /// Determines the result of contact, based on the type of contact made, when it's made with this entity.
        /// <para>Contacts that aren't a Success are prioritized over any Payback.
        /// If a ContactResult of Success is found, then the Payback for this entity is added if it exists
        /// and the ContactResult becomes a PartialSuccess.</para>
        /// </summary>
        /// <param name="attacker">The entity attacking this one</param>
        /// <param name="contactType">The type of contact made with this entity</param>
        /// <returns>A ContactResultInfo containing the result of the interaction</returns>
        public ContactResultInfo GetContactResult(BattleEntity attacker, ContactTypes contactType)
        {
            ContactResultInfo contactResultInfo =  Interactions.GetContactResult(attacker, contactType, GetAllPhysAttributes(), attacker.EntityProperties.GetContactExceptions(contactType));

            //On a Success, check if this Entity has any Payback and add it if so
            if ((contactResultInfo.ContactResult == ContactResult.Success || contactResultInfo.ContactResult == ContactResult.PartialSuccess) && HasPayback() == true)
            {
                PaybackHolder paybackholder;

                //Factor in the contact's Payback on a PartialSuccess
                if (contactResultInfo.ContactResult == ContactResult.PartialSuccess)
                    paybackholder = GetPayback(contactResultInfo.Paybackholder);
                //Get only the BattleEntity's Payback on a Success
                else paybackholder = GetPayback();

                //Since there's Payback, the result is now a PartialSuccess
                contactResultInfo.ContactResult = ContactResult.PartialSuccess;
                contactResultInfo.Paybackholder = paybackholder;
            }

            return contactResultInfo;
        }

        /// <summary>
        /// Returns all PhysicalAttributes the BattleEntity has, sorted by value in decreasing order
        /// </summary>
        /// <returns>An array of all PhysicalAttributes the BattleEntity has, sorted with higher PhysicalAttribute values first</returns>
        protected PhysicalAttributes[] GetAllPhysAttributes()
        {
            //Get the values in a list, then sort them
            List<PhysicalAttributes> physAttributeList = new List<PhysicalAttributes>(PhysAttributes.Keys.ToArray());
            physAttributeList.Sort(SortPhysicalAttributes);

            return physAttributeList.ToArray();
        }

        #endregion

        #region Element Override Methods

        /// <summary>
        /// Adds an Element Override for this BattleEntity for a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the Element Override</param>
        /// <param name="element">The Element to add for this PhysicalAttribute</param>
        public void AddElementOverride(PhysicalAttributes attribute, Elements element)
        {
            if (HasElementOverride(attribute) == true)
            {
                Debug.LogWarning($"{Entity.Name} already has an element override for the {attribute} PhysicalAttribute!");
                return;
            }

            ElementOverrides.Add(attribute, element);
            Debug.Log($"Added a(n) {element} override to {Entity.Name} for the {attribute} PhysicalAttribute!");
        }

        /// <summary>
        /// Removes an Element Override this BattleEntity has for a PhysicalAttribute
        /// </summary>
        /// <param name="attribute">The PhysicalAttribute associated with the Element Override</param>
        public void RemoveElementOverride(PhysicalAttributes attribute)
        {
            if (HasElementOverride(attribute) == false)
            {
                Debug.LogWarning($"{Entity.Name} does not contain an element override for the {attribute} PhysicalAttribute and thus cannot remove one!");
                return;
            }

            ElementOverrides.Remove(attribute);
            Debug.Log($"Removed element override for the {attribute} PhysicalAttribute on {Entity.Name}");
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
        /// Retrieves the Element Override this BattleEntity has for the first PhysicalAttribute found on a victim
        /// </summary>
        /// <param name="attacker">The BattleEntity this one is attacking</param>
        /// <returns>The type of Element damage this BattleEntity will do to the victim</returns>
        public Elements GetTotalElementOverride(BattleEntity victim)
        {
            PhysicalAttributes[] victimAttributes = victim.EntityProperties.GetAllPhysAttributes();

            for (int i = 0; i < victimAttributes.Length; i++)
            {
                if (HasElementOverride(victimAttributes[i]) == true)
                    return ElementOverrides[victimAttributes[i]];
            }

            return Elements.Invalid;
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
            bool positiveStatusImmune = GetAdditionalProperty<bool>(AdditionalProperty.PositiveStatusImmune);
            bool negativeStatusImmune = GetAdditionalProperty<bool>(AdditionalProperty.NegativeStatusImmune);
            bool neutralStatusImmune = GetAdditionalProperty<bool>(AdditionalProperty.NeutralStatusImmune);
            if ((status.Alignment == StatusEffect.StatusAlignments.Positive && positiveStatusImmune == true)
                || (status.Alignment == StatusEffect.StatusAlignments.Negative && negativeStatusImmune == true)
                || (status.Alignment == StatusEffect.StatusAlignments.Neutral && neutralStatusImmune == true))
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
            //If the entity already has this StatusEffect, refresh its properties with the new properties.
            //By default, the duration is refreshed.
            //We don't remove the status then reafflict it because that would end it. With a status like Frozen,
            //it would deal damage to the entity when being removed and we don't want that
            if (HasStatus(status.StatusType) == true)
            {
                StatusEffect refreshedStatus = GetStatus(status.StatusType);
                refreshedStatus.Refresh(status);
                
                Debug.Log($"{status.StatusType} Status on {Entity.Name} was refreshed with a duration of {status.Duration}!");
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
        /// Retrieves a specific StatusEffect. This method is internal.
        /// </summary>
        /// <param name="statusType">The StatusTypes of the StatusEffect to get.</param>
        /// <returns>null if the entity isn't afflicted with the StatusEffect, otherwise the StatusEffect it's afflicted with.</returns>
        private StatusEffect GetStatus(StatusTypes statusType)
        {
            if (HasStatus(statusType) == true)
            {
                return Statuses[statusType];
            }

            return null;
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

        #region Payback Data Methods

        /// <summary>
        /// Adds Payback to the BattleEntity, causing it to deal damage to direct attackers.
        /// </summary>
        /// <param name="paybackHolder">The PaybackHolder to add.</param>
        public void AddPayback(PaybackHolder paybackHolder)
        {
            Debug.Log($"Added {paybackHolder.Element} Payback of type {paybackHolder.PaybackType} to {Entity.Name}!");

            Paybacks.Add(paybackHolder);
        }

        /// <summary>
        /// Removes a Payback on the BattleEntity.
        /// </summary>
        /// <param name="paybackHolder">The PaybackHolder to remove.</param>
        public void RemovePayback(PaybackHolder paybackHolder)
        {
            bool removed = Paybacks.Remove(paybackHolder);

            if (removed == true)
            {
                Debug.Log($"Successfully removed {paybackHolder.Element} Payback of type {paybackHolder.PaybackType} on {Entity.Name}!");
            }
        }

        /// <summary>
        /// Gets the total Payback a BattleEntity has by combining all of the current Paybacks affecting the BattleEntity.
        /// </summary>
        /// <param name="additionalPaybacks">Any additional PaybackHolders to factor in. This is used when determining the total contact result.</param>
        /// <returns>A PaybackHolder with the combined properties of all the Paybacks the BattleEntity has</returns>
        public PaybackHolder GetPayback(params PaybackHolder[] additionalPaybacks)
        {
            //Gather all the entity's Paybacks in the list
            List<PaybackHolder> allPaybacks = new List<PaybackHolder>(Paybacks);

            //Add any additional Paybacks
            if (additionalPaybacks != null && additionalPaybacks.Length > 0)
            {
                allPaybacks.AddRange(additionalPaybacks);
            }

            //Initialize default values
            PaybackTypes totalType = PaybackTypes.Constant;
            Elements totalElement = Elements.Normal;
            int totalDamage = 0;
            List<StatusEffect> totalStatuses = new List<StatusEffect>();

            //Go through all the Paybacks and add them up
            for (int i = 0; i < allPaybacks.Count; i++)
            {
                PaybackHolder paybackHolder = allPaybacks[i];

                //If there's a Half or Full Payback, upgrade the current one from Half to Full if it's currently Half
                if (paybackHolder.PaybackType != PaybackTypes.Constant)
                {
                    //If there are at least two Half Paybacks, upgrade it to Full
                    if (totalType == PaybackTypes.Half && paybackHolder.PaybackType == PaybackTypes.Half)
                        totalType = PaybackTypes.Full;
                    else if (totalType != PaybackTypes.Full)
                        totalType = paybackHolder.PaybackType;
                }

                //Check for a higher priority Element
                if (paybackHolder.Element > totalElement)
                    totalElement = paybackHolder.Element;

                //Add up all the damage
                totalDamage += paybackHolder.Damage;

                //Add in all the StatusEffects - note that StatusEffects with the same StatusType will increase the chance of
                //that StatusEffect being inflicted, as the first one may not succeed in being inflicted depending on the BattleEntity
                if (paybackHolder.StatusesInflicted != null && paybackHolder.StatusesInflicted.Length > 0)
                {
                    totalStatuses.AddRange(paybackHolder.StatusesInflicted);
                }
            }

            //Return the final Payback
            return new PaybackHolder(totalType, totalElement, totalDamage, totalStatuses.ToArray());
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
        public void AddAdditionalProperty(AdditionalProperty property, object value)
        {
            //Remove if the entity already has it
            if (HasAdditionalProperty(property) == true)
            {
                RemoveAdditionalProperty(property);
            }

            AdditionalProperties.Add(property, value);
            Debug.Log($"Added the {property} property to {Entity.Name}!");
        }

        /// <summary>
        /// Removes an AdditionalProperty from the entity
        /// </summary>
        /// <param name="property">The AdditionalProperty to remove.</param>
        public void RemoveAdditionalProperty(AdditionalProperty property)
        {
            if (HasAdditionalProperty(property) == true)
            {
                Debug.Log($"Removed the {property} property on {Entity.Name}!");
            }

            AdditionalProperties.Remove(property);
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
            if (HasAdditionalProperty(property) == false)
            {
                return default(T);
            }

            return (T)AdditionalProperties[property];
        }

        #endregion

        #region Move Category Methods

        /// <summary>
        /// Disables a particular MoveCategory from being used by the entity.
        /// </summary>
        /// <param name="category">The type of moves to disable.</param>
        public void DisableMoveCategory(MoveCategories category)
        {
            if (IsMoveCategoryDisabled(category) == true)
            {
                Debug.LogWarning($"Category {category} is already disabled for {Entity.Name}!");
                return;
            }

            Debug.Log($"Disabled {category} moves from use for {Entity.Name}");

            DisabledMoveCategories.Add(category, true);
        }

        /// <summary>
        /// Clears a particular MoveCategory from being disabled.
        /// </summary>
        /// <param name="category">The type of moves to enable.</param>
        public void EnableMoveCategory(MoveCategories category)
        {
            bool removed = DisabledMoveCategories.Remove(category);

            if (removed == true)
            {
                Debug.Log($"Enabled {category} moves for {Entity.Name} to use once again");
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
            return DisabledMoveCategories.Keys.ToArray();
        }

        #endregion

        #region Physical Attribute Sort Methods

        /// <summary>
        /// A Comparison sort method for PhysicalAttributes, putting higher valued attributes first for consistency with contact results
        /// </summary>
        /// <param name="physAttr1"></param>
        /// <param name="physAttr2"></param>
        /// <returns></returns>
        private static int SortPhysicalAttributes(PhysicalAttributes physAttr1, PhysicalAttributes physAttr2)
        {
            if (physAttr1 > physAttr2)
                return -1;
            else if (physAttr1 < physAttr2)
                return 1;

            return 0;
        }

        #endregion
    }
}
