using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Poison Status Effect
    /// </summary>
    public sealed class PoisonStatus : StatusEffect
    {
        public PoisonStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Poison;
            Alignment = StatusAlignments.Negative;    

            Duration = duration;
        }

        public override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {
            
        }

        public override void OnPhaseStart()
        {
            EntityAfflicted.TakeDamage(Enumerations.Elements.Poison, 1, true);
            IncrementTurns();
        }

        public override void OnPhaseEnd()
        {

        }

        public override StatusEffect Copy()
        {
            return new PoisonStatus(Duration);
        }
    }
}
