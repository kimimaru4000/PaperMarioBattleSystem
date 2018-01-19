using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Quake Hammer Badge - Gives Mario the ability to use Quake Hammer.
    /// </summary>
    public sealed class QuakeHammerBadge : Badge
    {
        public QuakeHammerBadge()
        {
            Name = "Quake Hammer";
            Description = "Wear this to use Quake\nHammer. 3 FP are required\nto use this attack, which\nslightly damages all ground\nenemies.\n"
                + "Wearing two or more of these\nbadges requires more FP for\nthe move, but increases the\nAttack power.";

            BPCost = 2; //Costs 1 BP in PM
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.QuakeHammer;
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
