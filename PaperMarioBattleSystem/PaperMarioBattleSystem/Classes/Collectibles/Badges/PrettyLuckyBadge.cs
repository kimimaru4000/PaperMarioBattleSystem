using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Pretty Lucky Badge - Increases Mario's Evasion by 10%. Evasion is stacked multiplicatively.
    /// </summary>
    public class PrettyLuckyBadge : Badge
    {
        protected const double EvasionValue = .9d;

        public PrettyLuckyBadge()
        {
            Name = "Pretty Lucky";
            Description = "When Mario's attacked, cause enemies to sometimes miss.";

            BPCost = 2;
            PriceValue = 75;

            BadgeType = BadgeGlobals.BadgeTypes.PrettyLucky;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected sealed override void OnEquip()
        {
            EntityEquipped.AddEvasionMod(EvasionValue);
        }

        protected sealed override void OnUnequip()
        {
            EntityEquipped.RemoveEvasionMod(EvasionValue);
        }
    }
}
