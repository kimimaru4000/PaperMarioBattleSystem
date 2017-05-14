using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Peekaboo Badge - Allows Mario and his Partner to see enemy HP.
    /// </summary>
    public sealed class PeekabooBadge : Badge
    {
        public PeekabooBadge()
        {
            Name = "Peekaboo";
            Description = "Make enemy HP visible.";

            BPCost = 2;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.Peekaboo;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
