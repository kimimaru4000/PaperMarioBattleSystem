using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Power Rush Badge - Increases Mario's Attack by 2 when in Danger or Peril.
    /// </summary>
    public class PowerRushBadge : HealthStateBadge
    {
        /// <summary>
        /// The Attack bonus of Power Rush.
        /// </summary>
        protected const int AttackBonus = 2;

        protected sealed override bool CanActivate => EntityEquipped.IsInDanger;

        public PowerRushBadge()
        {
            Name = "Power Rush";
            Description = $"Increase Attack power by {AttackBonus}\n when Mario is in Danger.";

            BPCost = 1;
            PriceValue = 25;

            BadgeType = BadgeGlobals.BadgeTypes.PowerRush;
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
