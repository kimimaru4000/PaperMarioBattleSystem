using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Tiny Status Effect.
    /// The entity shrinks and has its Attack reduced by 2 until it ends.
    /// </summary>
    public sealed class TinyStatus : POWDownStatus
    {
        public TinyStatus(int duration) : base(2, duration)
        {
            StatusType = Enumerations.StatusTypes.Tiny;
        }

        public override StatusEffect Copy()
        {
            return new TinyStatus(Duration);
        }
    }
}
