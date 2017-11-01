using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Defend Plus Badge - Increases Defense by 1
    /// </summary>
    public sealed class DefendPlusBadge : Badge
    {
        private const int DefenseBonus = 1;

        public DefendPlusBadge()
        {
            Name = "Defend Plus";
            Description = "Boost Mario's Defense by 1.";

            BPCost = 5;

            BadgeType = BadgeGlobals.BadgeTypes.DefendPlus;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.RaiseDefense(DefenseBonus);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.LowerDefense(DefenseBonus);
        }
    }
}
