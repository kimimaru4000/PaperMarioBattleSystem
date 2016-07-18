using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private List<Badge> AllBadges = new List<Badge>();

        /// <summary>
        /// The Badges the Player owns that are active
        /// </summary>
        private List<Badge> ActiveBadges = new List<Badge>();

        /// <summary>
        /// The Player's Item inventory
        /// </summary>
        private List<Item> Items = new List<Item>();

        /// <summary>
        /// The key items the Player possesses
        /// </summary>
        private List<Item> KeyItems = new List<Item>();

        private Inventory()
        {
            
        }
    }
}
