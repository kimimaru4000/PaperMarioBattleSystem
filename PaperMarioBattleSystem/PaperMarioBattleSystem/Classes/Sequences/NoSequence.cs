using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Sequence that causes the BattleEntity to do nothing.
    /// <para>This is used when Mario or his Partner take the "Do Nothing" action and when entities are unable to attack an ally
    /// because it has no allies remaining or the move can't reach the ally.</para>
    /// </summary>
    public sealed class NoSequence : Sequence
    {
        public NoSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceMainBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceSuccessBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceFailedBranch()
        {
            PrintInvalidSequence();
        }

        protected override void SequenceMissBranch()
        {
            PrintInvalidSequence();
        }
    }
}
