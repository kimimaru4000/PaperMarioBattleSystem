using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Speedy Spin Badge - Increases the speed of Mario's spin dash.
    /// </summary>
    public sealed class SpeedySpinBadge : Badge
    {
        public SpeedySpinBadge()
        {
            Name = "Speedy Spin";
            Description = "Increases the distance Mario can Spin Dash.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.SpeedySpin;
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
