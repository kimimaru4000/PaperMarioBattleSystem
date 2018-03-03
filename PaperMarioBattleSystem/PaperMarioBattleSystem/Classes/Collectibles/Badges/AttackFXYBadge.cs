using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX Y Badge - Changes the sounds of Mario's attacks to the sound of a dull bell.
    /// </summary>
    public sealed class AttackFXYBadge : AttackFXBadge
    {
        public AttackFXYBadge()
        {
            Name = "Attack FX Y";
            Description = "Change the sound effects of Mario's attacks.";

            BPCost = 0;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXY;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }
    }
}
