using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Power Bounce Badge - Gives Mario the ability to use Power Bounce
    /// </summary>
    public sealed class PowerBounceBadge : Badge
    {
        public PowerBounceBadge()
        {
            Name = "Power Bounce";
            Description = "Wear this to use Power Bounce. 3 FP are required to use this attack,"
                + " which lets you jump on one enemy until you miss an Action Command.";

            BPCost = 3;

            BadgeType = BadgeGlobals.BadgeTypes.PowerBounce;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
