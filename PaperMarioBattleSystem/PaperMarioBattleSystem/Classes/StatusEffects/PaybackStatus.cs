using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem.Classes.StatusEffects
{
    /// <summary>
    /// The Payback Status Effect.
    /// When direct contact is made with the entity afflicted, the attacker receives half the damage dealt in a specific Element.
    /// Additionally, the attacker can be inflicted with one or more StatusEffects
    /// </summary>
    public sealed class PaybackStatus : StatusEffect
    {
        private Elements PaybackDamageType = Elements.Normal;
        private StatusEffect[] StatusesInflicted = null;

        public PaybackStatus(int duration, Elements paybackDamageType, params StatusEffect[] statusesToInflict)
        {
            StatusType = StatusTypes.Payback;
            Alignment = StatusAlignments.Positive;

            Duration = duration;
            PaybackDamageType = paybackDamageType;

            StatusesInflicted = statusesToInflict;
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnPhaseStart()
        {
            IncrementTurns();
        }

        protected override void OnPhaseEnd()
        {
            
        }

        public override StatusEffect Copy()
        {
            return new PaybackStatus(Duration, PaybackDamageType, StatusesInflicted);
        }
    }
}
