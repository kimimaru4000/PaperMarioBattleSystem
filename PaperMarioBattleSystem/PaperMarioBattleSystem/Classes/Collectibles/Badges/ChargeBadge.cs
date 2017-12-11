using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Charge Badge - Adds the Charge action to Mario's Tactics menu
    /// </summary>
    public class ChargeBadge : Badge
    {
        public ChargeBadge()
        {
            Name = "Charge";
            Description = "Wear this to add Charge to Mario's Tactics menu. This move requires one FP. " +
                "Wearing two or more of these badges requires more FP, but increases the charge power.";

            BPCost = 1;
            PriceValue = 25;

            BadgeType = BadgeGlobals.BadgeTypes.Charge;
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
