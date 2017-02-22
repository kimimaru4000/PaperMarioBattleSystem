using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Tornado Jump Badge - Gives Mario the ability to use Tornado Jump
    /// </summary>
    public sealed class TornadoJumpBadge : Badge
    {
        public TornadoJumpBadge()
        {
            Name = "Tornado Jump";
            Description = "Wear this to use Tornado Jump. 3 FP are required to use this attack, which can damage all midair enemies if "
                + "executed superbly. Wearing two or more of these badges requires more FP for the move, but increases the Attack power.";

            BPCost = 2;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.TornadoJump;
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
