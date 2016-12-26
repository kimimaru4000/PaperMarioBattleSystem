using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Flower Saver P Badge - Reduces the FP cost of Partner moves by 1, to a minimum of 1.
    /// </summary>
    public sealed class FlowerSaverPBadge : FlowerSaverBadge
    {
        public FlowerSaverPBadge()
        {
            Name = "Flower Saver P";
            Description = "Drop FP used when your partner attacks by 1.";

            BPCost = 4;
            PriceValue = 125;

            BadgeType = BadgeGlobals.BadgeTypes.FlowerSaverP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
