using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Slow Go Badge - Prevents Mario from running in the field.
    /// </summary>
    public sealed class SlowGoBadge : Badge
    {
        public SlowGoBadge()
        {
            Name = "Slow Go";
            Description = "Makes Mario sluggish, so he can no longer run.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.SlowGo;
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
