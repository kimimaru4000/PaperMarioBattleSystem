using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for all types of Items
    /// </summary>
    public abstract class Item : Collectible
    {
        /// <summary>
        /// The types of items available
        /// </summary>
        public enum ItemTypes
        {
            None, Healing, Damage, KeyItem
        }

        /// <summary>
        /// The type of Item this is
        /// </summary>
        public ItemTypes ItemType { get; protected set; } = ItemTypes.None;

        protected Item()
        {
            CollectibleType = Enumerations.CollectibleTypes.Item;
        }
    }
}
