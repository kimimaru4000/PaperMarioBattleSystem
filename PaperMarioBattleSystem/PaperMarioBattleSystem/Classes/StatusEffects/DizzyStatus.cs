using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Dizzy Status Effect.
    /// The entity's Accuracy decreases until it ends
    /// </summary>
    public sealed class DizzyStatus : StatusEffect
    {
        public DizzyStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Dizzy;
            Alignment = StatusAlignments.Negative;

            Duration = duration;

            AfflictedMessage = "Dizzy! Attacks might miss!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.ModifyAccuracy(-50);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.ModifyAccuracy(50);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            OnEnd();
        }

        protected override void OnResume()
        {
            OnAfflict();
        }

        public override StatusEffect Copy()
        {
            return new DizzyStatus(Duration);
        }
    }
}
