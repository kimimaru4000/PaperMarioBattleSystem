using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The FP Plus Badge - Raises Mario's Max FP by 5.
    /// </summary>
    public sealed class FPPlusBadge : Badge
    {
        public const int FPValue = 5;

        public FPPlusBadge()
        {
            Name = "FP Plus";
            Description = $"Increases maximum FP by {FPValue}.";

            BPCost = 3;
            PriceValue = 75;

            BadgeType = BadgeGlobals.BadgeTypes.FPPlus;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.RaiseMaxFP(FPValue);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.LowerMaxFP(FPValue);
        }
    }
}
