using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Simplifier Badge - Makes Action Commands easier for Mario and his Partner to perform but lowers the final Action Command value
    /// </summary>
    public sealed class SimplifierBadge : Badge
    {
        public SimplifierBadge()
        {
            Name = "Simplifier";
            Description = "Make action commands easy, but earn less Star Power.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.Simplifier;
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
