using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Power Plus Badge - Increases Attack by 1
    /// </summary>
    public sealed class PowerPlusBadge : Badge
    {
        private const int AttackBonus = 1;

        public PowerPlusBadge()
        {
            Name = "Power Plus";
            Description = "Boost Mario's jump and hammer attacks by 1.";

            BPCost = 6;

            BadgeType = BadgeGlobals.BadgeTypes.PowerPlus;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.RaiseAttack(AttackBonus);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.LowerAttack(AttackBonus);
        }
    }
}
