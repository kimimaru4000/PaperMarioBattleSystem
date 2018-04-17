using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Mario's Power Smash move.
    /// </summary>
    public sealed class PowerSmashSequence : HammerSequence
    {
        public PowerSmashSequence(MoveAction moveAction, int finalDamageAddition) : base(moveAction, finalDamageAddition)
        {

        }
    }
}
