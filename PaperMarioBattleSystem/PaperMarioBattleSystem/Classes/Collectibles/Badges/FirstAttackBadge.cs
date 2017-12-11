using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The First Attack Badge - Allows Mario to automatically win battles with weaker enemies when performing a First Strike on the field.
    /// </summary>
    public sealed class FirstAttackBadge : Badge
    {
        public FirstAttackBadge()
        {
            Name = "First Attack";
            Description = "Do a First Strike to defeat weak foes without battling.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.FirstAttack;
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
