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
    /// The base class for all types of Collectibles
    /// </summary>
    //NOTE: There is a hard cap of 32 for any type of collectible being dropped at once by an enemy encounter.
    //This includes Coins, Hearts, Flowers, Items, and Badges
    public abstract class Collectible : INameable
    {
        /// <summary>
        /// The name of the Collectible
        /// </summary>
        public string Name { get; protected set; } = "Item";

        /// <summary>
        /// The description of the Collectible
        /// </summary>
        public string Description { get; protected set; } = "N/A";

        /// <summary>
        /// The icon for the Collectible
        /// </summary>
        public Texture2D Icon { get; protected set; } = null;

        /// <summary>
        /// The type of Collectible this is
        /// </summary>
        public Enumerations.CollectibleTypes CollectibleType { get; protected set; } = Enumerations.CollectibleTypes.None;

        /// <summary>
        /// The price value of the Collectible
        /// </summary>
        public uint PriceValue { get; protected set; } = 0u;

        protected Collectible()
        {

        }
    }
}
