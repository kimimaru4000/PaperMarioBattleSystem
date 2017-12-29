using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The D-Down Pound Badge - Gives Mario the ability to use D-Down Pound.
    /// </summary>
    public sealed class DDownPoundBadge : Badge
    {
        public DDownPoundBadge()
        {
            Name = "D-Down Pound";
            Description = "Lets you do a D-Down Pound. Uses 2 FP. Disables an enemy's defense power and injures it.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.DDownPound;
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
