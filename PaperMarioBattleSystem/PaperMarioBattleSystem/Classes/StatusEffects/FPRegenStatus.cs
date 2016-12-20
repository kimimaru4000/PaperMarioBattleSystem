using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The FPRegen Status Effect.
    /// The entity heals an amount of FP each turn until it ends.
    /// </summary>
    public sealed class FPRegenStatus : StatusEffect
    {
        /// <summary>
        /// The amount of FP to heal each turn.
        /// </summary>
        private int AmountHealed = 0;

        public FPRegenStatus(int amountHealed, int duration)
        {
            StatusType = Enumerations.StatusTypes.FPRegen;
            Alignment = StatusAlignments.Positive;

            AmountHealed = amountHealed;
            Duration = duration;
        }

        protected override void OnAfflict()
        {

        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
        {
            EntityAfflicted.HealFP(AmountHealed);
            IncrementTurns();
        }

        protected override void OnSuspend()
        {

        }

        protected override void OnResume()
        {

        }

        public override StatusEffect Copy()
        {
            return new FPRegenStatus(AmountHealed, Duration);
        }
    }
}
