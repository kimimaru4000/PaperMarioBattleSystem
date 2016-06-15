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
    public abstract class Collectible
    {
        /// <summary>
        /// The name of the Collectible
        /// </summary>
        public string Name { get; protected set; } = "Item";

        /// <summary>
        /// The icon for the Collectible
        /// </summary>
        public Texture2D Icon { get; protected set; } = null;

        /// <summary>
        /// The price  value of the Collectible
        /// </summary>
        public int PriceValue { get; protected set; } = 0;

        protected Collectible()
        {

        }

        protected Collectible(string name, Texture2D icon, int priceValue)
        {
            Name = name;
            Icon = icon;
            PriceValue = priceValue;
        }
    }
}
