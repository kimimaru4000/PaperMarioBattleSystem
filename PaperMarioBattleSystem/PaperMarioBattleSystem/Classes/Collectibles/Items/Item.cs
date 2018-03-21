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
        /// The categories of items available.
        /// </summary>
        public enum ItemCategories
        {
            Standard, KeyItem
        }

        /// <summary>
        /// The types of items available.
        /// This enum is a bit field, so handle it with bitwise operations.
        /// </summary>
        [Flags]
        public enum ItemTypes
        {
            None = 0,
            Healing = 1 << 0,
            Damage = 1 << 1,
            Status = 1 << 2,
            Revival = 1 << 3,

            /// <summary>
            /// Items that are used for non-standard purposes.
            /// This includes the Lucky Star, which enables Action Commands.
            /// </summary>
            Special = 1 << 4
        }

        public ItemCategories ItemCategory { get; protected set; } = ItemCategories.Standard;

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
