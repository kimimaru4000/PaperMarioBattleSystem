using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The most basic item. It restores 5 HP.
    /// </summary>
    public class Mushroom : BattleItem, IHPHealingItem
    {
        public int HPRestored { get; protected set; }

        public Mushroom()
        {
            Name = "Mushroom";
            Description = "Heals 5 HP.";

            ItemType = ItemTypes.Healing;

            HPRestored = 5;

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            EntityType = Enumerations.EntityTypes.Player;
        }
    }
}
