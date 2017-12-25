using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base Sequence for all Special Moves.
    /// <para>The starts of Star Spirit and Crystal Star Special Moves are different, 
    /// so they will need to be done in separate derived classes.</para>
    /// <para>Special Move Sequences do not have any Stylish moves.</para>
    /// </summary>
    public abstract class SpecialMoveSequence : Sequence
    {
        protected double WalkDuration = 500d;

        public SpecialMoveSequence(MoveAction specialAction) : base(specialAction)
        {
            
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch(SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
