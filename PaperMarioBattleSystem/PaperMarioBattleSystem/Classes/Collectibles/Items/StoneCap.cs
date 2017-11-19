using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stone Cap item. It inflicts Stone for 3 turns on an ally.
    /// </summary>
    public sealed class StoneCap : BattleItem, IStatusInflictingItem
    {
        public StatusChanceHolder[] StatusesInflicted { get; private set; }
        private const int StoneTurns = 3;

        public StoneCap()
        {
            Name = "Stone Cap";
            Description = "Turns Mario to stone and makes\nhim unable to move for a while.";

            ItemType = ItemTypes.Damage;

            StatusesInflicted = new StatusChanceHolder[] { new StatusChanceHolder(100d, new StoneStatus(StoneTurns)) };

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            MoveAffectionType = Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally;
        }
    }
}
