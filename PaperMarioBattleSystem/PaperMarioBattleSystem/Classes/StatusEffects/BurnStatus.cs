using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Burn Status Effect.
    /// The entity takes 1 HP in Fire damage at the end of each phase
    /// </summary>
    public sealed class BurnStatus : StatusEffect
    {
        public BurnStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Burn;
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
            
        }

        public override void OnPhaseEnd()
        {
            EntityAfflicted.TakeDamage(Enumerations.Elements.Fire, 1, true);
            IncrementTurns();
        }

        public override StatusEffect Copy()
        {
            return new BurnStatus(Duration);
        }
    }
}
