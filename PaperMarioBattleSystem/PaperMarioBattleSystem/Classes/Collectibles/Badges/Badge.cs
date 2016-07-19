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

        /// <summary>
        /// Which type of entity the Badge affects when equipped
        /// </summary>
        public AffectedTypes AffectedType { get; protected set; } = AffectedTypes.Self;

        /// <summary>
        /// Whether the Badge is equipped or not
        /// </summary>
        public bool Equipped { get; private set; } = false;

        protected Badge()
        {
            CollectibleType = Enumerations.CollectibleTypes.Badge;
        }

        /// <summary>
        /// Gets the order value of the Badge, based on its BadgeType
        /// </summary>
        public int Order => BadgeGlobals.GetBadgeOrderValue(BadgeType);

        /// <summary>
        /// Tells if the Badge can be equipped or not
        /// </summary>
        public bool CanEquip => (Inventory.Instance.BP >= BPCost);

        /// <summary>
        /// Equips the Badge
        /// </summary>
        public void Equip()
        {
            //Don't perform the equip effects again
            if (Equipped == true) return;

            Equipped = true;

            OnEquip();
        }

        /// <summary>
        /// Unequips the Badge
        /// </summary>
        public void UnEquip()
        {
            //Don't perform the unequip effects again
            if (Equipped == false) return;

            Equipped = false;
            
            OnUnequip();
        }

        /// <summary>
        /// What occurs when the Badge is equipped
        /// </summary>
        protected abstract void OnEquip();

        /// <summary>
        /// What occurs when the Badge is unequipped
        /// </summary>
        protected abstract void OnUnequip();
    }
}
