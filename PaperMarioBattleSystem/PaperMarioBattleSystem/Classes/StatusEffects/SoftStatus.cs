using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Soft Status Effect.
    /// The entity's Defense is reduced by 3 until it ends.
    /// </summary>
    public sealed class SoftStatus : DEFDownStatus
    {
        public SoftStatus(int duration) : base(3, duration)
        {
            StatusType = Enumerations.StatusTypes.Soft;
        }

        public override StatusEffect Copy()
        {
            return new SoftStatus(Duration);
        }
    }
}