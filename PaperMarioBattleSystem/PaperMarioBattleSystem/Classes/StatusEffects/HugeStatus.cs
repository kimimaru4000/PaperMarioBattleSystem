using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem.Classes.StatusEffects
{
    /// <summary>
    /// The Huge Status Effect.
    /// The entity grows and has its Attack raised by 2 until it ends.
    /// </summary>
    public sealed class HugeStatus : POWUpStatus
    {
        public HugeStatus(int duration) : base(2, duration)
        {
            StatusType = Enumerations.StatusTypes.Huge;

            AfflictedMessage = "Huge! Attack power is\nnow boosted!";
        }

        public override StatusEffect Copy()
        {
            return new HugeStatus(Duration);
        }
    }
}
