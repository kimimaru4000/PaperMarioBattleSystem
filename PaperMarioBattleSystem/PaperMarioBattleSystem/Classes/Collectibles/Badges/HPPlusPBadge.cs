using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The HP Plus P Badge - Raises Partner's Max HP by 5.
    /// </summary>
    public sealed class HPPlusPBadge : HPPlusBadge
    {
        public HPPlusPBadge()
        {
            Name = "HP Plus P";
            Description += " (Partner)";
            
            BPCost = 6;
            PriceValue = 150;

            BadgeType = BadgeGlobals.BadgeTypes.HPPlusP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
