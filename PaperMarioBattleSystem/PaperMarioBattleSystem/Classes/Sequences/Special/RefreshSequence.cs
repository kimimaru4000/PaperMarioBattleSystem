using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for the Refresh move.
    /// </summary>
    public sealed class RefreshSequence : StarSpiritMoveSequence
    {
        public RefreshSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Heal 5 HP and FP
                    User.HealHP(5);
                    User.HealFP(5);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
