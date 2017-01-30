using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Bump Attack Badge - Allows Mario to automatically win battles with weaker enemies when touching them on the field.
    /// </summary>
    public sealed class BumpAttackBadge : Badge
    {
        public BumpAttackBadge()
        {
            Name = "Bump Attack";
            Description = "Bump into weak foes to defeat them without battling.";

            BPCost = 5;
            PriceValue = 150;

            BadgeType = BadgeGlobals.BadgeTypes.BumpAttack;
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
