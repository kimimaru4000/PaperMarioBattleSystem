using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Frozen Status Effect.
    /// Entities afflicted with this cannot move until it ends, in which the entity will take 1 Ice damage.
    /// If the entity is afflicted with Burn while it is Frozen, both effects will negate each other
    /// </summary>
    public sealed class FrozenStatus : ImmobilizedStatus
    {
        public FrozenStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Frozen;
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();

            //The entity takes 1 Ice damage when Frozen ends
            EntityAfflicted.TakeDamage(Enumerations.Elements.Ice, 1, true);
        }

        public sealed override StatusEffect Copy()
        {
            return new FrozenStatus(Duration);
        }
    }
}
