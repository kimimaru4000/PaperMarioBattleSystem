using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The HPRegen Status Effect.
    /// The entity heals an amount of HP each turn until it ends.
    /// </summary>
    public sealed class HPRegenStatus : StatusEffect
    {
        /// <summary>
        /// The amount of HP to heal each turn.
        /// </summary>
        private int AmountHealed = 0;

        public HPRegenStatus(int amountHealed, int duration)
        {
            StatusType = Enumerations.StatusTypes.HPRegen;
            Alignment = StatusAlignments.Positive;

            AmountHealed = amountHealed;
            Duration = duration;

            AfflictedMessage = "HP will briefly recover!";
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnPhaseCycleStart()
        {
            EntityAfflicted.HealHP(AmountHealed);
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
            return new HPRegenStatus(AmountHealed, Duration);
        }
    }
}
