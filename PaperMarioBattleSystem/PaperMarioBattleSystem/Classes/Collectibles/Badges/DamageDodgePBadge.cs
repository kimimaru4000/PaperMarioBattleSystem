using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Damage Dodge P Badge - Adds 1 to Partner's Defense when successfully performing a Guard.
    /// </summary>
    public sealed class DamageDodgePBadge : DamageDodgeBadge
    {
        public DamageDodgePBadge()
        {
            Name = "Damage Dodge P";
            Description = "Cut damage by 1 HP with a Guard Action Command. (Ally)";

            BadgeType = BadgeGlobals.BadgeTypes.DamageDodgeP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
