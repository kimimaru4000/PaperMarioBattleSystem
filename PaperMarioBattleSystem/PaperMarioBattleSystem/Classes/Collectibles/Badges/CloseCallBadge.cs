using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Close Call Badge - Increases Mario's Evasion by 33% (30% in PM) when he's in Danger or Peril.
    /// </summary>
    public class CloseCallBadge : HealthStateBadge
    {
        protected const double EvasionValue = .66d;

        protected override bool CanActivate => EntityEquipped.IsInDanger;

        public CloseCallBadge()
        {
            Name = "Close Call";
            Description = "When Mario's in Danger, cause enemies to sometimes miss.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.CloseCall;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void ApplyEffects()
        {
            EntityEquipped.AddEvasionMod(EvasionValue);
        }

        protected override void RemoveEffects()
        {
            EntityEquipped.RemoveEvasionMod(EvasionValue);
        }
    }
}
