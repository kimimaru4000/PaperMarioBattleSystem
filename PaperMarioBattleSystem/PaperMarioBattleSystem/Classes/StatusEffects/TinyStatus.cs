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
        private const int AttackReduction = 2;

        public TinyStatus(int duration) : base(AttackReduction, duration)
        {
            StatusType = Enumerations.StatusTypes.Tiny;

            AfflictedMessage = "Tiny! Attack power has\nnow dropped!";
        }

        public override StatusEffect Copy()
        {
            return new TinyStatus(Duration);
        }
    }
}
