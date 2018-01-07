using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Spin Attack Badge - Causes Mario's spin dash to instantly defeat weaker enemies on the field.
    /// </summary>
    public sealed class SpinAttackBadge : Badge
    {
        public SpinAttackBadge()
        {
            Name = "Spin Attack";
            Description = "Lets Mario destroy a weaker enemy with a spinning move.";

            BPCost = 3;

            BadgeType = BadgeGlobals.BadgeTypes.SpinAttack;
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
