using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base sequence for Star Spirit moves.
    /// </summary>
    public abstract class StarSpiritMoveSequence : SpecialMoveSequence
    {
        protected double WaitTime = 700d;

        public StarSpiritMoveSequence(SpecialMoveAction specialAction) : base(specialAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //NOTE: Mario moves up a tiny bit when he's in the front, I haven't confirmed how it works in the back yet

                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName);
                    CurSequenceAction = new WaitSeqAction(WaitTime);
                    break;
                case 2:
                    User.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.StarWishName);
                    //NOTE: Show Star Spirit appearing and VFX and such
                    CurSequenceAction = new WaitSeqAction(WaitTime);
                    break;
                case 3:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Star Spirit Special Moves don't have action commands
        protected sealed override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Star Spirit Special Moves don't have action commands
        protected sealed override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
