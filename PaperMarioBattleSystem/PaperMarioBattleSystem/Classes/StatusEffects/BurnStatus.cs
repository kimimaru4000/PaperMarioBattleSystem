using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Burn Status Effect.
    /// The entity takes 1 HP in Fire damage at the start of each phase cycle
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
                Debug.Log($"{StatusType} was inflicted on an entity afflicted with {Enumerations.StatusTypes.Frozen}, negating both effects!");
                EntityAfflicted.RemoveStatus(Enumerations.StatusTypes.Frozen);

                //Also remove Burn, as these two statuses negate each other
                EntityAfflicted.RemoveStatus(Enumerations.StatusTypes.Burn);
            }
        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
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
