using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.BadgeGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Player's inventory
    /// </summary>
    public class Inventory
    {
        #region Singleton Fields

        public static Inventory Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Inventory();
                }

                return instance;
            }
        }

        private static Inventory instance = null;

        #endregion

        /// <summary>
        /// The number of Badge Points (BP) Mario currently has available
        /// </summary>
        public int BP { get; private set; } = 3;

        /// <summary>
        /// The max number of Badge Points (BP) Mario has
        /// </summary>
        public int MaxBP { get; private set; } = 3;

        /// <summary>
        /// All Badges the Player owns
        /// </summary>
        private readonly List<Badge> AllBadges = new List<Badge>();

        /// <summary>
        /// Counts for all Badges the Player owns
        /// </summary>
        private readonly Dictionary<BadgeTypes, int> AllBadgeCounts = new Dictionary<BadgeTypes, int>();

        /// <summary>
        /// The Badges the Player owns that are active
        /// </summary>
        private readonly List<Badge> ActiveBadges = new List<Badge>();
        
        /// <summary>
        /// Counts for the active Badges the Player owns
        /// </summary>
        private readonly Dictionary<BadgeTypes, int> ActiveBadgeCounts = new Dictionary<BadgeTypes, int>();

        /// <summary>
        /// The Player's Item inventory
        /// </summary>
        private readonly List<Item> Items = new List<Item>();

        /// <summary>
        /// The key items the Player possesses
        /// </summary>
        private readonly List<Item> KeyItems = new List<Item>();

        private Inventory()
        {
            
        }

        #region Item Methods

        /// <summary>
        /// Adds an Item to the Player's Inventory
        /// </summary>
        /// <param name="item">The Item to add</param>
        public void AddItem(Item item)
        {
            if (item.ItemType == Item.ItemTypes.KeyItem)
            {
                KeyItems.Add(item);
            }
            else
            {
                Items.Add(item);
            }
        }

        /// <summary>
        /// Removes an Item from the Player's Inventory
        /// </summary>
        /// <param name="item">The Item to remove</param>
        public void RemoveItem(Item item)
        {
            if (item.ItemType == Item.ItemTypes.KeyItem)
            {
                KeyItems.Remove(item);
            }
            else
            {
                Items.Remove(item);
            }
        }

        /// <summary>
        /// Finds the first instance of an Item by name
        /// </summary>
        /// <param name="name">The name of the Item to find</param>
        /// <param name="keyItem">Whether the Item is a key item or not</param>
        /// <returns>The Item if found, otherwise null</returns>
        public Item FindItem(string name, bool keyItem)
        {
            if (keyItem == true)
            {
                return KeyItems.Find((keyitem) => keyitem.Name == name);
            }
            else
            {
                return Items.Find((item) => item.Name == name);
            }
        }

        #endregion

        #region Badge Methods

        /// <summary>
        /// Adds a Badge to the Player's Inventory
        /// </summary>
        /// <param name="badge">The Badge to add</param>
        public void AddBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError($"Attempting to add a null Badge to the Inventory!");
                return;
            }

            //Add to the badge list
            AllBadges.Add(badge);

            //Increment number if a Badge of this type already exists
            if (HasBadge(badge.BadgeType) == true)
            {
                AllBadgeCounts[badge.BadgeType]++;
            }
            //Otherwise add a new entry with a count of 1
            else
            {
                AllBadgeCounts.Add(badge.BadgeType, 1);
            }
        }

        /// <summary>
        /// Removes a Badge from the Player's Inventory.
        /// If the Badge is active, it also unequips the Badge and removes it from the active Badge list
        /// </summary>
        /// <param name="badge">The Badge to remove</param>
        public void RemoveBadge(Badge badge)
        {
            if (badge == null)
            {
                Debug.LogError($"Attempting to remove a null Badge from the Inventory!");
                return;
            }

            if (HasBadge(badge.BadgeType) == false)
            {
                Debug.LogWarning($"Badge of type {badge.BadgeType} cannot be removed because it doesn't exist in the Inventory!");
                return;
            }

            //Remove from the all badges list
            AllBadges.Remove(badge);
            AllBadgeCounts[badge.BadgeType]--;
            if (AllBadgeCounts[badge.BadgeType] <= 0)
            {
                AllBadgeCounts.Remove(badge.BadgeType);
            }

            //Remove from active badges if it's active, and unequip it
            if (badge.Equipped == true)
            {
                //Unequip badge first to remove its effects
                badge.UnEquip();
                
                //Remove from the active badges list
                ActiveBadges.Remove(badge);
                ActiveBadgeCounts[badge.BadgeType]--;
                if (ActiveBadgeCounts[badge.BadgeType] <= 0)
                {
                    ActiveBadgeCounts.Remove(badge.BadgeType);
                }
            }
        }

        /// <summary>
        /// Finds the first instance of a Badge with a particular BadgeType
        /// </summary>
        /// <param name="badgeType">The BadgeType of the Badge</param>
        /// <param name="equipped">Whether the Badge is equipped or not</param>
        /// <returns>null if no Badge was found, otherwise the first Badge matching the parameters</returns>
        public Badge GetBadge(BadgeTypes badgeType, bool equipped)
        {
            if (HasBadge(badgeType) == false) return null;
        
            if (equipped == true)
            {
                return GetActiveBadge(badgeType);
            }
            else
            {
                return AllBadges.Find((badge) => badge.BadgeType == badgeType);
            }
        }

        /// <summary>
        /// Gets the number of Badges of a particular BadgeType in the Player's Inventory
        /// </summary>
        /// <param name="badgeType">The BadgeType to find</param>
        /// <returns>The number of Badges of the BadgeType in the Player's Inventory</returns>
        public int GetBadgeCount(BadgeTypes badgeType)
        {
            if (HasBadge(badgeType) == false) return 0;

            return AllBadgeCounts[badgeType];
        }

        /// <summary>
        /// Tells whether the Player owns a particular Badge or not
        /// </summary>
        /// <param name="badgeType">The BadgeType</param>
        /// <returns>true if the Player owns the badge, false if not</returns>
        public bool HasBadge(BadgeTypes badgeType)
        {
            return AllBadgeCounts.ContainsKey(badgeType);
        }

        /*Active Badges*/

        /// <summary>
        /// Finds the first instance of an active Badge with a particular BadgeType
        /// </summary>
        /// <param name="badgeType">The BadgeType of the Badge</param>
        /// <returns>null if no Badge was found, otherwise the first active Badge matching the BadgeType</returns>
        private Badge GetActiveBadge(BadgeTypes badgeType)
        {
            if (IsBadgeActive(badgeType) == false) return null;

            return ActiveBadges.Find((badge) => badge.BadgeType == badgeType);
        }

        /// <summary>
        /// Gets the number of active Badges of a particular BadgeType
        /// </summary>
        /// <param name="badgeType">The BadgeType to find</param>
        /// <returns>The number of active Badges of the BadgeType</returns>
        public int GetActiveBadgeCount(BadgeTypes badgeType)
        {
            if (IsBadgeActive(badgeType) == false) return 0;

            return ActiveBadgeCounts[badgeType];
        }

        /// <summary>
        /// Tells whether the Player has an active Badge of a particular type
        /// </summary>
        /// <param name="badgeType">The BadgeType</param>
        /// <returns>true if the Player owns the and the Badge is active, false otherwise</returns>
        public bool IsBadgeActive(BadgeTypes badgeType)
        {
            return ActiveBadgeCounts.ContainsKey(badgeType);
        }

        #endregion

        #region Static Badge Sort Methods

        /// <summary>
        /// A Comparison method used to sort Badges by their Orders (Types)
        /// </summary>
        /// <param name="badge1">The first Badge to compare</param>
        /// <param name="badge2">The second Badge to compare</param>
        /// <returns>-1 if badge1 has a lower Order, 1 if badge2 has a lower Order, 0 if they have the same Order</returns>
        public static int BadgeOrderSort(Badge badge1, Badge badge2)
        {
            if (badge1 == null && badge2 == null) return 0;
            if (badge1 == null) return 1;
            if (badge2 == null) return -1;

            if (badge1.Order < badge2.Order)
                return -1;
            if (badge1.Order > badge2.Order)
                return 1;

            return 0;
        }

        /// <summary>
        /// A Comparison method used to sort Badges alphabetically (ABC)
        /// </summary>
        /// <param name="badge1">The first Badge to compare</param>
        /// <param name="badge2">The second Badge to compare</param>
        /// <returns>-1 if badge1 has a lower Order, 1 if badge2 has a lower Order, 0 if they have the same Order</returns>
        public static int BadgeAlphabeticalSort(Badge badge1, Badge badge2)
        {
            if (badge1 == null && badge2 == null) return 0;
            if (badge1 == null) return 1;
            if (badge2 == null) return -1;

            return string.Compare(badge1.Name, badge2.Name, StringComparison.CurrentCulture);
        }

        /// <summary>
        /// A Comparison method used to sort Badges by BP cost (BP Needed)
        /// </summary>
        /// <param name="badge1">The first Badge to compare</param>
        /// <param name="badge2">The second Badge to compare</param>
        /// <returns>-1 if badge1 has a lower BP cost, 1 if badge2 has a lower BP cost, 0 if they have the same BP cost and Order</returns>
        public static int BadgeBPSort(Badge badge1, Badge badge2)
        {
            if (badge1 == null && badge2 == null) return 0;
            if (badge1 == null) return 1;
            if (badge2 == null) return -1;

            if (badge1.BPCost < badge2.BPCost)
                return -1;
            if (badge1.BPCost > badge2.BPCost)
                return 1;

            //Resort to their Orders if they have the same BP cost
            return BadgeOrderSort(badge1, badge2);
        }

        #endregion
    }
}
