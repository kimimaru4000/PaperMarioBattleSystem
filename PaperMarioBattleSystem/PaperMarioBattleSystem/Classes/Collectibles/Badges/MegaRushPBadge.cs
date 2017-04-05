using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Mega Rush P Badge - Increases Partner's Attack by 5 when in Peril.
    /// </summary>
    public sealed class MegaRushPBadge : MegaRushBadge
    {
        public MegaRushPBadge()
        {
            Name = "Mega Rush P";
            Description = $"Increase Attack power by {AttackBonus}\nwhen your partner is in Peril.";

            BadgeType = BadgeGlobals.BadgeTypes.MegaRushP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
