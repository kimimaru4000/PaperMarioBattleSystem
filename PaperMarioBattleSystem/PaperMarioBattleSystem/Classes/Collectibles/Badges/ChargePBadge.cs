using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Charge P Badge - Adds the Charge action to the Partner's Tactics menu
    /// </summary>
    public sealed class ChargePBadge : ChargeBadge
    {
        public ChargePBadge()
        {
            Name = "Charge P";
            Description = "Wear this to add Charge to your partner's Tactics menu. This move requires one FP. " +
                "Wearing two or more of these badges requires more FP, but increases the charge power.";

            PriceValue = 37;

            BadgeType = BadgeGlobals.BadgeTypes.ChargeP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
