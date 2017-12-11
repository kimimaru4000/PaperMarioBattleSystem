using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Mega Rush Badge - Increases Mario's Attack by 5 (4 in PM) when in Peril.
    /// </summary>
    public class MegaRushBadge : HealthStateBadge
    {
        /// <summary>
        /// The Attack bonus of Mega Rush.
        /// </summary>
        protected const int AttackBonus = 5;

        protected sealed override bool CanActivate => (EntityEquipped.HealthState == Enumerations.HealthStates.Peril);

        public MegaRushBadge()
        {
            Name = "Mega Rush";
            Description = $"Increase Attack power by {AttackBonus}\nwhen Mario is in Peril.";

            BPCost = 1;
            PriceValue = 25;

            BadgeType = BadgeGlobals.BadgeTypes.MegaRush;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected sealed override void ApplyEffects()
        {
            EntityEquipped.RaiseAttack(AttackBonus);
        }

        protected sealed override void RemoveEffects()
        {
            EntityEquipped.LowerAttack(AttackBonus);
        }
    }
}
