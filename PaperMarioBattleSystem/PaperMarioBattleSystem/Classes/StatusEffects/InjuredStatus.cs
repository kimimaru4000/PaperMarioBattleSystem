using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Injured Status Effect.
    /// Entities afflicted with this can't move until it ends.
    /// <para>This Status Effect is inflicted when Partners take damage in the original Paper Mario.</para>
    /// </summary>
    public sealed class InjuredStatus : ImmobilizedStatus
    {
        public InjuredStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Injured;

            StatusIcon = null;

            AfflictedMessage = "You're injured--you can't move!";
        }

        public override StatusEffect Copy()
        {
            return new InjuredStatus(Duration);
        }
    }
}
