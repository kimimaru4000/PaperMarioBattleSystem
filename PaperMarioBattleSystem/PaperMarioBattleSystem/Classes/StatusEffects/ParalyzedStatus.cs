using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem.Classes.StatusEffects
{
    /// <summary>
    /// The Paralyzed Status Effect.
    /// Entities afflicted with this cannot move until it ends.
    /// </summary>
    public sealed class ParalyzedStatus : ImmobilizedStatus
    {
        public ParalyzedStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Paralyzed;

            AfflictedMessage = "Your enemy's paralyzed and can't move!";
        }

        public override StatusEffect Copy()
        {
            return new ParalyzedStatus(Duration);
        }
    }
}
