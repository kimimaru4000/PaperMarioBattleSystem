using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with StatusEffects
    /// </summary>
    public static class StatusGlobals
    {
        #region Enums

        public enum PaybackTypes
        {
            Constant, Half, Full
        }

        #endregion

        #region Structs

        /// <summary>
        /// Holds information about Payback damage
        /// </summary>
        public struct PaybackHolder
        {
            /// <summary>
            /// The type of Payback damage
            /// </summary>
            public PaybackTypes PaybackType { get; private set; }

            /// <summary>
            /// The Physical Attribute that the Payback is associated with (Ex. Fiery for Fiery enemies).
            /// </summary>
            public Enumerations.PhysicalAttributes PhysAttribute { get; private set; }

            /// <summary>
            /// The Elemental damage dealt
            /// </summary>
            public Enumerations.Elements Element { get; private set; }

            /// <summary>
            /// The ContactTypes that the Payback affects.
            /// </summary>
            public Enumerations.ContactTypes[] PaybackContacts { get; private set; }

            /// <summary>
            /// The ContactProperties that the Payback affects.
            /// </summary>
            public Enumerations.ContactProperties[] ContactProperties { get; private set; }

            /// <summary>
            /// Tells the ContactResult of the Payback.
            /// </summary>
            public Enumerations.ContactResult PaybackContactResult { get; private set; }

            /// <summary>
            /// The adjusted ContactResult if the Attacker has the same PhysicalAttribute as <see cref="PhysAttribute"/>.
            /// <para>Ex. This would be Success for Electrified.</para>
            /// </summary>
            public Enumerations.ContactResult SamePhysAttrResult { get; private set; }

            /// <summary>
            /// The amount of damage to deal.
            /// <para>If the PaybackType is Constant, this is the total damage dealt. Otherwise, this damage is added to the total.</para>
            /// </summary>
            public int Damage { get; private set; }

            /// <summary>
            /// The Status Effects to inflict and their chances of being inflicted.
            /// </summary>
            public StatusChanceHolder[] StatusesInflicted { get; private set; }

            public static PaybackHolder Default =>
            new PaybackHolder(PaybackTypes.Constant, Enumerations.PhysicalAttributes.None, Enumerations.Elements.Normal,
                new Enumerations.ContactTypes[] { Enumerations.ContactTypes.SideDirect, Enumerations.ContactTypes.TopDirect },
                new Enumerations.ContactProperties[] { Enumerations.ContactProperties.None },
                Enumerations.ContactResult.Success, Enumerations.ContactResult.Success, 0, null);

            public PaybackHolder(PaybackTypes paybackType, Enumerations.PhysicalAttributes physAttribute, Enumerations.Elements element,
                Enumerations.ContactTypes[] paybackContacts, Enumerations.ContactProperties[] contactProperties,
                Enumerations.ContactResult contactResult, Enumerations.ContactResult samePhysAttrResult,
                params StatusChanceHolder[] statusesInflicted)
            {
                PaybackType = paybackType;
                PhysAttribute = physAttribute;
                Element = element;
                PaybackContacts = paybackContacts;
                ContactProperties = contactProperties;
                PaybackContactResult = contactResult;
                SamePhysAttrResult = samePhysAttrResult;
                Damage = 0;
                StatusesInflicted = statusesInflicted;

            }

            public PaybackHolder(PaybackTypes paybackType, Enumerations.PhysicalAttributes physAttribute, Enumerations.Elements element,
                Enumerations.ContactTypes[] paybackContacts, Enumerations.ContactProperties[] contactProperties,
                Enumerations.ContactResult contactResult, Enumerations.ContactResult samePhysAttrResult, int constantDamage,
                params StatusChanceHolder[] statusesInflicted)
                : this(paybackType, physAttribute, element, paybackContacts, contactProperties, contactResult, samePhysAttrResult, statusesInflicted)
            {
                Damage = constantDamage;
            }

            /// <summary>
            /// Gets the Payback damage that this PaybackHolder deals.
            /// <para>The minimum damage Payback can deal is 1. Half Payback rounds down.</para>
            /// </summary>
            /// <remarks>TTYD takes the halved damage and doubles it for Full payback.
            /// This leads to some seemingly odd cases (Ex. 5 damage dealt is lowered to 2 and doubled, dealing 4).
            /// </remarks>
            /// <param name="damageDealt">The amount of damage to deal.</param>
            /// <returns>All the damage dealt, half of it, or a constant amount of damage based on the PaybackType.
            /// 1 if the Payback damage is 0 or less.</returns>
            public int GetPaybackDamage(int damageDealt)
            {
                switch (PaybackType)
                {
                    case PaybackTypes.Full: return Math.Max(((int)Math.Floor(damageDealt / 2f) * 2) + Damage, 1);
                    case PaybackTypes.Half: return Math.Max((int)Math.Floor(damageDealt / 2f) + Damage, 1);
                    default: return Math.Max(Damage, 1);
                }
            }

            /// <summary>
            /// Combines a set of Paybacks into one.
            /// </summary>
            /// <param name="paybackHolders">The set of PaybackHolders to combine.</param>
            /// <returns>A combined PaybackHolder. If the set is null or empty, then a PaybackHolder with default values.</returns>
            public static PaybackHolder CombinePaybacks(in IList<PaybackHolder> paybackHolders)
            {
                //Initialize default values
                PaybackTypes totalType = PaybackTypes.Constant;
                Enumerations.PhysicalAttributes totalPhysAttribute = Enumerations.PhysicalAttributes.None;
                Enumerations.Elements totalElement = Enumerations.Elements.Normal;
                Enumerations.ContactResult totalContactResult = Enumerations.ContactResult.Success;
                Enumerations.ContactResult totalAttrContactResult = Enumerations.ContactResult.Success;
                int totalDamage = 0;
                List<StatusChanceHolder> totalStatuses = new List<StatusChanceHolder>();

                List<Enumerations.ContactTypes> totalContactTypes = new List<Enumerations.ContactTypes>();
                List<Enumerations.ContactProperties> totalContactProperties = new List<Enumerations.ContactProperties>();

                //Go through all the Paybacks and add them up
                for (int i = 0; i < paybackHolders.Count; i++)
                {
                    PaybackHolder paybackHolder = paybackHolders[i];

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

                    //Check for a higher priority PhysicalAttribute
                    if (paybackHolder.PhysAttribute > totalPhysAttribute)
                        totalPhysAttribute = paybackHolder.PhysAttribute;

                    //Check for higher priority ContactResults
                    if (paybackHolder.PaybackContactResult > totalContactResult)
                        totalContactResult = paybackHolder.PaybackContactResult;
                    if (paybackHolder.SamePhysAttrResult > totalAttrContactResult)
                        totalAttrContactResult = paybackHolder.SamePhysAttrResult;

                    //Add up all the damage
                    totalDamage += paybackHolder.Damage;

                    //Add in all the StatusEffects - note that StatusEffects with the same StatusType will increase the chance of
                    //that StatusEffect being inflicted, as the first one may not succeed in being inflicted depending on the BattleEntity
                    if (paybackHolder.StatusesInflicted != null && paybackHolder.StatusesInflicted.Length > 0)
                    {
                        totalStatuses.AddRange(paybackHolder.StatusesInflicted);
                    }

                    //Check for affected ContactTypes and add ones that the combined payback doesn't have
                    if (paybackHolder.PaybackContacts != null)
                    {
                        for (int j = 0; j < paybackHolder.PaybackContacts.Length; j++)
                        {
                            Enumerations.ContactTypes contactType = paybackHolder.PaybackContacts[j];
                            if (totalContactTypes.Contains(contactType) == false)
                            {
                                totalContactTypes.Add(contactType);
                            }
                        }
                    }

                    //Check for affected ContactProperties and combine them
                    if (paybackHolder.ContactProperties != null)
                    {
                        for (int j = 0; j < paybackHolder.ContactProperties.Length; j++)
                        {
                            Enumerations.ContactProperties contactProperty = paybackHolder.ContactProperties[j];
                            if (totalContactProperties.Contains(contactProperty) == false)
                            {
                                totalContactProperties.Add(contactProperty);
                            }
                        }
                    }
                }

                //Return the final Payback
                return new PaybackHolder(totalType, totalPhysAttribute, totalElement, totalContactTypes.ToArray(), totalContactProperties.ToArray(), totalContactResult, totalAttrContactResult, totalDamage, totalStatuses.Count == 0 ? null : totalStatuses.ToArray());
            }
        }

        /// <summary>
        /// Holds information about Confusion
        /// </summary>
        public struct ConfusionHolder
        {
            public int ConfusionPercent { get; private set; }

            public ConfusionHolder(int confusionPercent)
            {
                ConfusionPercent = confusionPercent;
            }
        }

        #endregion

        #region Classes

        /// <summary>
        /// Compares StatusEffects by their priorities.
        /// </summary>
        public class StatusComparer : IComparer<StatusEffect>
        {
            /// <summary>
            /// An IComparer method used to sort StatusEffects by their Priorities.
            /// </summary>
            /// <param name="status1">The first StatusEffect to compare.</param>
            /// <param name="status2">The second StatusEffect to compare.</param>
            /// <returns>-1 if status1 has a higher priority, 1 if status2 has a higher priority, 0 if they have the same priorities.</returns>
            public int Compare(StatusEffect status1, StatusEffect status2)
            {
                return StatusPrioritySort(status1, status2);
            }
        }

        /// <summary>
        /// Compares StatusTypes by their priorities.
        /// </summary>
        public class StatusTypeComparer : IComparer<Enumerations.StatusTypes>
        {
            /// <summary>
            /// An IComparer method used to sort StatusTypes by their Priorities.
            /// </summary>
            /// <param name="statusType1">The first StatusType to compare.</param>
            /// <param name="statusType2">The second StatusType to compare.</param>
            /// <returns>-1 if statusType1 has a higher priority, 1 if statusType2 has a higher priority, and 0 if they have the same priorities.</returns>
            public int Compare(Enumerations.StatusTypes statusType1, Enumerations.StatusTypes statusType2)
            {
                return StatusTypePrioritySort(statusType1, statusType2);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// Defines the priority of StatusEffects. Higher priorities affect BattleEntities sooner.
        /// <para>Related StatusEffects are sometimes grouped together in lines for readability</para>
        /// </summary>
        private readonly static Dictionary<StatusTypes, int> StatusOrder = new Dictionary<StatusTypes, int>()
        {
            { StatusTypes.KO, 350 }, { StatusTypes.Fright, 349 }, { StatusTypes.Blown, 348 }, { StatusTypes.Lifted, 347 },
            { StatusTypes.WaterBlock, 300 }, { StatusTypes.CloudNine, 299 }, { StatusTypes.TurboCharge, 298 },
            { StatusTypes.Sleep, 250 }, { StatusTypes.Stop, 249 }, { StatusTypes.Paralyzed, 248 }, { StatusTypes.Injured, 247 },
            { StatusTypes.Dizzy, 200 }, { StatusTypes.Confused, 199 },
            { StatusTypes.Electrified, 150 }, { StatusTypes.Dodgy, 149 },
            { StatusTypes.Burn, 130 }, { StatusTypes.Frozen, 129 },
            { StatusTypes.Huge, 120 }, { StatusTypes.Tiny, 119 },
            { StatusTypes.Poison, 110 },
            { StatusTypes.POWUp, 100 }, { StatusTypes.POWDown, 99 }, { StatusTypes.DEFUp, 98 }, { StatusTypes.DEFDown, 97 },
            { StatusTypes.Allergic, 90 },
            { StatusTypes.HPRegen, 80 }, { StatusTypes.FPRegen, 79 },
            { StatusTypes.Invisible, 70 },
            { StatusTypes.Stone, 60 },
            { StatusTypes.NoSkills, 50 },
            { StatusTypes.Fast, 40 }, { StatusTypes.Slow, 39 },
            { StatusTypes.Payback, 30 }, { StatusTypes.HoldFast, 29 },
            { StatusTypes.Charged, 0 }
        };

        #endregion

        #region Constants

        /// <summary>
        /// Denotes a duration value for a StatusEffect that does not go away
        /// </summary>
        public const int InfiniteDuration = 0;

        /// <summary>
        /// Denotes the Y offset for displaying StatusEffect icons if a BattleEntity is afflicted with more than one.
        /// </summary>
        public const int IconYOffset = 35;

        /// <summary>
        /// The time it takes for the HPRegen and FPRegen Status Effects to lerp between their colors.
        /// </summary>
        public const double RegenColorLerpTime = 330d;

        #endregion

        #region Methods

        /// <summary>
        /// Gets the Priority value of a particular type of StatusEffect
        /// </summary>
        /// <param name="statusType">The StatusType to get priority for</param>
        /// <returns>The Priority value corresponding to the StatusType if it has an entry, otherwise 0</returns>
        public static int GetStatusPriority(Enumerations.StatusTypes statusType)
        {
            int order = 0;
            StatusOrder.TryGetValue(statusType, out order);

            return order;
        }

        /// <summary>
        /// A Comparison method used to sort StatusEffects by their Priorities
        /// </summary>
        /// <param name="status1">The first StatusEffect to compare</param>
        /// <param name="status2">The second StatusEffect to compare</param>
        /// <returns>-1 if status1 has a higher priority, 1 if status2 has a higher priority, and 0 if they have the same priorities.</returns>
        public static int StatusPrioritySort(StatusEffect status1, StatusEffect status2)
        {
            if (status1 == null)
                return 1;
            if (status2 == null)
                return -1;

            return StatusTypePrioritySort(status1.StatusType, status2.StatusType);
        }

        /// <summary>
        /// A Comparison method used to sort StatusTypes by their Priorities.
        /// </summary>
        /// <param name="statusType1">The first StatusType to compare.</param>
        /// <param name="statusType2">The second StatusType to compare.</param>
        /// <returns>-1 if statusType1 has a higher priority, 1 if statusType2 has a higher priority, and 0 if they have the same priorities.</returns>
        public static int StatusTypePrioritySort(StatusTypes statusType1, StatusTypes statusType2)
        {
            int priority1 = GetStatusPriority(statusType1);
            int priority2 = GetStatusPriority(statusType2);

            if (priority1 < priority2)
                return 1;
            else if (priority1 > priority2)
                return -1;

            return 0;
        }

        #endregion
    }
}
