using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Head Rattle Badge - Gives Mario the ability to use Head Rattle.
    /// </summary>
    public sealed class HeadRattleBadge : Badge
    {
        public HeadRattleBadge()
        {
            Name = "Head Rattle";
            Description = "Wear this to use Head Rattle. 2 FP are required to use this attack, which can confuse enemies if executed "
                + "superbly. Wearing two or more of these badges requires more FP for the move, but enemies stay confused longer.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.HeadRattle;
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
