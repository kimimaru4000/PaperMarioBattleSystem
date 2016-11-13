using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sleepy Sheep item. It inflicts Sleep on all enemies.
    /// </summary>
    public sealed class SleepySheep : Item, IStatusInflictingItem
    {
        public StatusEffect[] StatusesInflicted { get; private set; }

        public SleepySheep()
        {
            Name = "Sleepy Sheep";
            Description = "Targets all enemies and may cause them to fall asleep.";

            ItemType = ItemTypes.Damage;

            StatusesInflicted = new StatusEffect[] { new SleepStatus(2) };
        }
    }
}
