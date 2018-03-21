using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lucky Star key item. It allows Mario and his Partner to perform Action Commands.
    /// </summary>
    public sealed class LuckyStar : Item
    {
        /// <summary>
        /// The name of the Lucky Star key item.
        /// </summary>
        public const string LuckyStarName = "Lucky Star";

        public LuckyStar()
        {
            Name = LuckyStarName;
            Description = "A star-shaped pendant.Lets\nyou use the action command.";

            ItemCategory = ItemCategories.KeyItem;
            ItemType = ItemTypes.Special;
        }
    }
}
