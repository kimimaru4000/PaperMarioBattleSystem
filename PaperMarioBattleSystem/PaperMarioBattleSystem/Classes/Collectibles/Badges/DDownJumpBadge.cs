using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The D-Down Jump Badge - Gives Mario the ability to use D-Down Jump.
    /// </summary>
    public sealed class DDownJumpBadge : Badge
    {
        public DDownJumpBadge()
        {
            Name = "D-Down Jump";
            Description = "Lets you do a D-Down Jump. Uses 2 FP. Disables an enemy's defense power and causes some damage.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.DDownJump;
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
