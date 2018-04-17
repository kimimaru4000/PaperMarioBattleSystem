using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Power Smash Badge - Gives Mario the ability to use Power Smash.
    /// </summary>
    public sealed class PowerSmashBadge : Badge
    {
        public PowerSmashBadge()
        {
            Name = "Power Smash";
            Description = "Wear this to use Power\nSmash. 2 FP are required to\n"
                + "use this attack, which lets\nyou whack an enemy with\n"
                + "great power.\nWearing two or more of these\nbadges requires"
                + " more FP for\nthe move, but increases the\nAttack power.";

            BPCost = 1;
            PriceValue = 100;

            BadgeType = BadgeGlobals.BadgeTypes.PowerSmash;
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
