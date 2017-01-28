using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Chill Out Badge - Makes Mario immune to First Strikes.
    /// <para>If an enemy hits Mario on the field with this equipped, the battle starts as if the enemy didn't hit Mario.</para>
    /// </summary>
    public sealed class ChillOutBadge : Badge
    {
        public ChillOutBadge()
        {
            Name = "Chill Out";
            Description = "Never succumb to a First Strike.";

            BPCost = 1;
            PriceValue = 37;

            BadgeType = BadgeGlobals.BadgeTypes.ChillOut;
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
