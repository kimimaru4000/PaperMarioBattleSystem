using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Fast Status Effect.
    /// Entities afflicted with this can move an additional turn each phase it's active
    /// </summary>
    public sealed class FastStatus : StatusEffect
    {
        private const int AdditionalTurns = 1;

        public FastStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Fast;
            Alignment = StatusAlignments.Positive;
            
            Duration = duration;
        }

        protected override void OnAfflict()
        {
            //Set this on affliction as well, as the entity could have not used its turn yet if it's in the same phase
            //First check if the max turn count is greater than 0, as a turn count of 0 indicates the entity was not able to move this turn
            if (EntityAfflicted.MaxTurns > 0)
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns + AdditionalTurns);
        }

        protected override void OnEnd()
        {
            if (IsFinished == false && EntityAfflicted.MaxTurns > 0)
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
            if (IsFinished == false && EntityAfflicted.MaxTurns > 0)
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns + AdditionalTurns);
        }

        public override StatusEffect Copy()
        {
            return new FastStatus(Duration);
        }
    }
}
