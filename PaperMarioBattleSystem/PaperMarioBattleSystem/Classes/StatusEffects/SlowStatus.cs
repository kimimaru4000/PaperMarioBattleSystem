using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Slow Status Effect.
    /// Entities afflicted with this can move only once every two phases.
    /// <para>Mario and his Partner can still Guard and Superguard when afflicted with this Status Effect</para>
    /// </summary>
    public sealed class SlowStatus : StatusEffect
    {
        private bool PreventMovement = true;

        public SlowStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Slow;
            Alignment = StatusAlignments.Negative;

            Duration = duration;
        }

        protected override void OnAfflict()
        {
            //On affliction end the entity's turn
            //PreventMovement can be false here if this StatusEffect was previously suspended
            if (PreventMovement == true)
            {
                EntityAfflicted.SetMaxTurns(0);
                EntityAfflicted.SetTurnsUsed(EntityAfflicted.MaxTurns);
            }
        }

        protected override void OnEnd()
        {
            EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
        }

        protected override void OnPhaseStart()
        {
            //If the entity shouldn't move this turn, set its max turns to 0
            if (PreventMovement == true)
                EntityAfflicted.SetMaxTurns(0);

            //Flip the flag telling whether the entity can move next turn or not
            PreventMovement = !PreventMovement;
        }

        protected override void OnPhaseEnd()
        {
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new SlowStatus(Duration);
        }
    }
}
