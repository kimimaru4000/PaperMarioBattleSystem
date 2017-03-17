using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Volt Shroom item. It inflicts Electrified for 5 turns on an ally.
    /// </summary>
    public sealed class VoltShroom : BattleItem, IStatusInflictingItem
    {
        public StatusChanceHolder[] StatusesInflicted { get; private set; }
        private const int ElectrifiedTurns = 5;

        public VoltShroom()
        {
            Name = "Volt Shroom";
            Description = "Electrifies you to damage\ndirect-attackers.";

            ItemType = ItemTypes.Damage;

            StatusesInflicted = new StatusChanceHolder[] { new StatusChanceHolder(100d, new ElectrifiedStatus(ElectrifiedTurns)) };

            SelectionType = TargetSelectionMenu.EntitySelectionType.Single;
            EntityType = Enumerations.EntityTypes.Player;
        }
    }
}
