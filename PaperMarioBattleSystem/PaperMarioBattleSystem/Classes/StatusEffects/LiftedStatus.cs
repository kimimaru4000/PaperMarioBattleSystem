using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lifted Status Effect.
    /// Entities afflicted with it are lifted out of battle, essentially dying.
    /// <para>This Status Effect is inflicted with Parakarry's Air Lift move.</para>
    /// </summary>
    public sealed class LiftedStatus : KOStatus
    {
        public LiftedStatus()
        {
            StatusType = Enumerations.StatusTypes.Lifted;
            Alignment = StatusAlignments.Negative;
        }

        public override StatusEffect Copy()
        {
            return new LiftedStatus();
        }
    }
}
