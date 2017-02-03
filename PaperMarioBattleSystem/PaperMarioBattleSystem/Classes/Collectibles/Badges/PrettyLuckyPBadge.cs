using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Pretty Lucky P Badge - Increases Partner's Evasion by 10%. Evasion is stacked multiplicatively.
    /// </summary>
    public sealed class PrettyLuckyPBadge : PrettyLuckyBadge
    {
        public PrettyLuckyPBadge()
        {
            Name = "Pretty Lucky P";
            Description = "When your ally's attacked, cause foes to sometimes miss.";

            BPCost = 2;
            PriceValue = 75;

            BadgeType = BadgeGlobals.BadgeTypes.PrettyLuckyP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
