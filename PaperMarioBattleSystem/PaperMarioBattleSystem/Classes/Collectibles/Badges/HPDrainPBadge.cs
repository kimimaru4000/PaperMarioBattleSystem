using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The HP Drain P Badge - Reduces Partner's Attack by 1 and recovers 1 HP after damaging one or more enemies.
    /// </summary>
    public sealed class HPDrainPBadge : HPDrainBadge
    {
        public HPDrainPBadge()
        {
            Name = "HP Drain P";
            Description = "Drops your ally's Attack by 1 but regain 1 HP per attack.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.HPDrainP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
