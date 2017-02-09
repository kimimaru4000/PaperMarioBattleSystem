using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Dodgy Status Effect.
    /// The entity's Evasion increases until it ends.
    /// </summary>
    public class DodgyStatus : StatusEffect
    {
        /// <summary>
        /// The Evasion modifier.
        /// </summary>
        protected double EvasionValue = .5d;

        public DodgyStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Dodgy;
            Alignment = StatusAlignments.Positive;

            Duration = duration;

            AfflictedMessage = "Dodgy! Some attacks will\nautomatically be dodged!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.AddEvasionMod(EvasionValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RemoveEvasionMod(EvasionValue);
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
