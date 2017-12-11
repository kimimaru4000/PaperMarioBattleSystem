using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Power Rush P Badge - Increases Partner's Attack by 2 when in Danger or Peril.
    /// </summary>
    public sealed class PowerRushPBadge : PowerRushBadge
    {
        public PowerRushPBadge()
        {
            Name = "Power Rush P";
            Description = $"Increase Attack power by {AttackBonus}\n when your ally is in Danger.";

            BadgeType = BadgeGlobals.BadgeTypes.PowerRushP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
