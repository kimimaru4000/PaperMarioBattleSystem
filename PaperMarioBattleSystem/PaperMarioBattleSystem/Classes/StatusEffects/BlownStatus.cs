using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Blown Status Effect.
    /// Entities afflicted with it are blown out of battle, essentially dying.
    /// <para>This Status Effect is inflicted with Flurrie's Gale Force move and Lakilester's Hurricane move.</para>
    /// </summary>
    public sealed class BlownStatus : KOStatus
    {
        public BlownStatus()
        {
            StatusType = Enumerations.StatusTypes.Blown;
            Alignment = StatusAlignments.Negative;
        }

        public override StatusEffect Copy()
        {
            return new BlownStatus();
        }
    }
}
