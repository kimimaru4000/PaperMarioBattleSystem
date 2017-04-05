using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Unsimplifier Badge - Makes Action Commands harder for Mario and his Partner to perform but raises the final Action Command value
    /// </summary>
    public sealed class UnsimplifierBadge : Badge
    {
        public UnsimplifierBadge()
        {
            Name = "Unsimplifier";
            Description = "Make action commands hard, but earn more Star Power.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.Unsimplifier;
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
