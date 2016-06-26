using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stone Status Effect.
    /// This is essentially Immobilized except the entity afflicted with it is immune to damage.
    /// <para>This has a Positive Alignment because entities that attack an entity afflicted with this basically waste their turns</para>
    /// </summary>
    public sealed class StoneStatus : ImmobilizedStatus
    {
        public StoneStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Stone;
            Alignment = StatusAlignments.Positive;
        }

        public sealed override StatusEffect Copy()
        {
            return new StoneStatus(Duration);
        }
    }
}
