using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.BadgeGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for all Badges
    /// </summary>
    //Notes on the Feeling Fine Badge:
    //It protects against every StatusEffect EXCEPT Burn, Frozen, and Allergic
    public abstract class Badge : Collectible
    {
        /// <summary>
        /// The amount of BP the Badge costs to wear
        /// </summary>
        public int BPCost { get; protected set; } = 0;

        /// <summary>
        /// The type of Badge this is
        /// </summary>
        public BadgeTypes BadgeType { get; protected set; } = BadgeTypes.None;

        protected Badge()
        {

        }

        /// <summary>
        /// Gets the order value of the Badge, based on its BadgeType
        /// </summary>
        public int Order => BadgeGlobals.GetBadgeOrderValue(BadgeType);

        /// <summary>
        /// What occurs when the Badge is equipped
        /// </summary>
        public abstract void OnEquip();

        /// <summary>
        /// What occurs when the Badge is unequipped
        /// </summary>
        public abstract void OnUnequip();

        #region Static Sort Methods

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
