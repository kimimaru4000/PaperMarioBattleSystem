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
    /// The base class for all Badges
    /// </summary>
    
    //Notes on the Feeling Fine Badge:
    //It protects against every StatusEffect EXCEPT Burn, Frozen, and Allergic
    public abstract class Badge : Collectible
    {
        public int BPCost { get; protected set; } = 0;

        protected Badge()
        {

        }

        /// <summary>
        /// What occurs when the Badge is equipped
        /// </summary>
        public abstract void OnEquip();

        /// <summary>
        /// What occurs when the Badge is unequipped
        /// </summary>
        public abstract void OnUnequip();
    }
}
