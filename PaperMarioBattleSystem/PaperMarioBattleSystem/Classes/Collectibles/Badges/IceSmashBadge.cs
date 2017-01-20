using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Ice Smash Badge - Gives Mario the ability to use Ice Smash.
    /// </summary>
    public sealed class IceSmashBadge : Badge
    {
        public IceSmashBadge()
        {
            Name = "Ice Smash";
            Description = "Wear this to use Ice Smash. 3 FP are required to use this attack, which can freeze and immobilize an enemy " +
                "if executed superbly. Wearing two or more of these badges requires more FP for the move, but enemies stay frozen longer.";

            BPCost = 1;
            PriceValue = 37;

            BadgeType = BadgeGlobals.BadgeTypes.IceSmash;
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
