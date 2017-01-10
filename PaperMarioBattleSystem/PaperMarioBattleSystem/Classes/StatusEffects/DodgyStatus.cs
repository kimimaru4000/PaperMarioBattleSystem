using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Dodgy Status Effect.
    /// The entity's Evasion increases until it ends
    /// </summary>
    public sealed class DodgyStatus : StatusEffect
    {
        public DodgyStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Dodgy;
            Alignment = StatusAlignments.Positive;

            Duration = duration;

            AfflictedMessage = "Dodgy! Some attacks will\nautomatically be dodged!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.ModifyEvasion(50);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.ModifyEvasion(-50);
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
            return new DodgyStatus(Duration);
        }
    }
}
