using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Piercing Blow Badge - Gives Mario the ability to use Piercing Blow.
    /// </summary>
    public class PiercingBlowBadge : Badge
    {
        public PiercingBlowBadge()
        {
            Name = "Piercing Blow";
            Description = "Wear this to use Piercing Blow. 2 FP are required to use this attack, which deals damage that pierces enemy defenses.";

            BPCost = 1;
            PriceValue = 75;

            BadgeType = BadgeGlobals.BadgeTypes.PiercingBlow;
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
