using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Poison Status Effect.
    /// The entity takes 1 HP in Poison damage at the start of each phase
    /// </summary>
    public sealed class PoisonStatus : StatusEffect
    {
        public PoisonStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Poison;
            Alignment = StatusAlignments.Negative;    

            Duration = duration;
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnPhaseStart()
        {
            EntityAfflicted.TakeDamage(Enumerations.Elements.Poison, 1, true);
            IncrementTurns();
        }

        protected override void OnPhaseEnd()
        {

        }

        public override StatusEffect Copy()
        {
            return new PoisonStatus(Duration);
        }
    }
}
