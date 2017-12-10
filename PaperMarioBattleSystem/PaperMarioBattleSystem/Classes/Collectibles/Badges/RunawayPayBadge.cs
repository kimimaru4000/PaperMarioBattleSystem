using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Ruanway Pay Badge - Mario gains any Star Points he earned in battle when running away.
    /// </summary>
    public sealed class RunawayPayBadge : Badge
    {
        public RunawayPayBadge()
        {
            Name = "Runaway Pay";
            Description = "Lets Mario earn Star Points even if he escapes from battle.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.RunawayPay;
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
