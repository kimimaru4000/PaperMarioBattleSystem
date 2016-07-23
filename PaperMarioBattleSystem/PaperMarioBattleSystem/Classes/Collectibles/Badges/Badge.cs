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
        /// The Type Number of the Badge
        /// </summary>
        public int TypeNumber { get; protected set; } = 0;

        /// <summary>
        /// The BattleEntity the Badge is equipped to
        /// </summary>
        public BattleEntity EntityEquipped { get; private set; } = null;

        /// <summary>
        /// Whether the Badge is equipped or not
        /// </summary>
        public bool Equipped { get; private set; } = false;

        /// <summary>
        /// Tells if the Badge can be equipped by the Player or not
        /// </summary>
        public bool CanEquip => (Inventory.Instance.BP >= BPCost);

        protected Badge()
        {
            CollectibleType = Enumerations.CollectibleTypes.Badge;
        }

        /// <summary>
        /// Equips the Badge to a BattleEntity
        /// </summary>
        public void Equip(BattleEntity entity)
        {
            //Don't perform the equip effects again
            if (Equipped == true)
            {
                Debug.LogWarning($"Badge {Name} is already equipped to {EntityEquipped?.Name} and thus cannot be re-equipped");
                return;
            }

            EntityEquipped = entity;

            //Activate the Badge if equipped to a Player entity
            if (EntityEquipped?.EntityType == Enumerations.EntityTypes.Player)
            {
                Inventory.Instance.ActivateBadge(this);
            }

            //Set the equipped flag after activation
            Equipped = true;

            if (EntityEquipped == null)
            {
                Debug.LogError($"Equipping Badge to NULL BattleEntity! Fix ASAP");
            }
            else
            {
                Debug.Log($"Equipped {Name} Badge to {EntityEquipped.Name}");
            }

            //Apply Equip effects
            OnEquip();
        }

        /// <summary>
        /// Unequips the Badge from the BattleEntity
        /// </summary>
        public void UnEquip()
        {
            //Don't perform the unequip effects again
            if (Equipped == false)
            {
                Debug.LogWarning($"Badge {Name} is not equipped to {EntityEquipped?.Name} and thus cannot be unequipped");
                return;
            }

            //Apply UnEquip effects
            OnUnequip();

            //Dectivate the Badge if unequipped from a Player entity
            if (EntityEquipped?.EntityType == Enumerations.EntityTypes.Player)
            {
                Inventory.Instance.DeactivateBadge(this);
            }

            //Clear the equipped flag after deactivation
            Equipped = false;
            
            if (EntityEquipped == null)
            {
                Debug.LogError($"Unequipping Badge from NULL BattleEntity! Fix ASAP");
            }
            else
            {
                Debug.Log($"Unequipped {Name} Badge from {EntityEquipped.Name}");
            }

            EntityEquipped = null;
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
