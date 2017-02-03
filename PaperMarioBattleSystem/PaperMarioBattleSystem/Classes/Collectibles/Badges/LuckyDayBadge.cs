using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lucky Day Badge - Increases Mario's Evasion by 25% (20% in PM). Evasion is stacked multiplicatively.
    /// </summary>
    public sealed class LuckyDayBadge : Badge
    {
        public LuckyDayBadge()
        {
            Name = "Lucky Day";
            Description = "When Mario's attacked, causes enemies to miss more often.";

            BPCost = 7;
            PriceValue = 250;

            BadgeType = BadgeGlobals.BadgeTypes.LuckyDay;
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
