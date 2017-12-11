using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The I Spy Badge - Alerts Mario when he's close to a hidden Star Piece panel.
    /// </summary>
    public sealed class ISpyBadge : Badge
    {
        public ISpyBadge()
        {
            Name = "I Spy";
            Description = "A sound and icon alert you to a nearby hidden panel.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.ISpy;
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
