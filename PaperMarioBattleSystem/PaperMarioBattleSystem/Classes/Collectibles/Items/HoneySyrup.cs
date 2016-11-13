using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The most basic FP restoring item. It restores 5 FP.
    /// </summary>
    public class HoneySyrup : Item, IFPHealingItem
    {
        public int FPRestored { get; protected set; }

        public HoneySyrup()
        {
            Name = "Honey Syrup";
            Description = "Restores 5 FP.";

            ItemType = ItemTypes.Healing;

            FPRestored = 5;
        }
    }
}
