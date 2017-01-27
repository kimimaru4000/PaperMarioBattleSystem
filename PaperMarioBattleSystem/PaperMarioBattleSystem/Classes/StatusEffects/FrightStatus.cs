using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Fright Status Effect.
    /// Entities afflicted with it run out of battle in fear, essentially dying.
    /// </summary>
    public sealed class FrightStatus : KOStatus
    {
        public FrightStatus()
        {
            StatusType = Enumerations.StatusTypes.Fright;
            Alignment = StatusAlignments.Negative;
        }

        public override StatusEffect Copy()
        {
            return new FrightStatus();
        }
    }
}
