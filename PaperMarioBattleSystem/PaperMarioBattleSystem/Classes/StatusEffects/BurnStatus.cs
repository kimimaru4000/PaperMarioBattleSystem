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

        protected override void OnAfflict()
        {
            //Remove the Frozen status if the entity was afflicted with Burn
            if (EntityAfflicted.HasStatus(Enumerations.StatusTypes.Frozen) == true)
            {
                Debug.Log($"{StatusType} removed {Enumerations.StatusTypes.Frozen} on the entity!");
                EntityAfflicted.RemoveStatus(Enumerations.StatusTypes.Frozen);
            }
        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseStart()
        {
            
        }

        protected override void OnPhaseEnd()
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
