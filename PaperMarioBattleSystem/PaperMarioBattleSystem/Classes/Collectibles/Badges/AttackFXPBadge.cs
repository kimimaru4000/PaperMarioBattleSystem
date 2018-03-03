using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX P Badge - Changes the sounds of Mario's attacks to the sound of a Bowser roar.
    /// </summary>
    public sealed class AttackFXPBadge : AttackFXBadge
    {
        public AttackFXPBadge()
        {
            Name = "Attack FX P";
            Description = "Change the sound effects of Mario's attacks.";

            BPCost = 0;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXP;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }
    }
}
