using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;
using static PaperMarioBattleSystem.StatusGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Payback Status Effect.
    /// When direct contact is made with the entity afflicted, the attacker receives half the damage dealt in a specific Element
    /// and can be inflicted with one or more StatusEffects.
    /// <para>If the entity already has the Payback effect when this is afflicted, the Element and StatusEffects should change to the new
    /// values if they are their default values (for now).</para>
    /// </summary>
    public sealed class PaybackStatus : StatusEffect
    {
        private PaybackHolder Paybackholder = PaybackHolder.Default;

        public PaybackStatus(int duration, PaybackHolder paybackHolder)
        {
            StatusType = StatusTypes.Payback;
            Alignment = StatusAlignments.Positive;

            Duration = duration;

            Paybackholder = paybackHolder;
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.AddPayback(Paybackholder);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.EntityProperties.RemovePayback(Paybackholder);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.EntityProperties.RemovePayback(Paybackholder);
        }

        protected override void OnResume()
        {
            EntityAfflicted.EntityProperties.AddPayback(Paybackholder);
        }

        public override StatusEffect Copy()
        {
            return new PaybackStatus(Duration, Paybackholder);
        }
    }
}
