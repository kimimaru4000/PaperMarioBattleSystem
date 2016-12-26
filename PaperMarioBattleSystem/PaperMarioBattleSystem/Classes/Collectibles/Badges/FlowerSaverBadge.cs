using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Flower Saver Badge - Reduces the FP cost of Mario's moves by 1, to a minimum of 1.
    /// </summary>
    public class FlowerSaverBadge : Badge
    {
        public FlowerSaverBadge()
        {
            Name = "Flower Saver";
            Description = "Drop FP used when Mario attacks by 1.";

            //TTYD BP cost
            BPCost = 4;
            PriceValue = 125;

            BadgeType = BadgeGlobals.BadgeTypes.FlowerSaver;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
