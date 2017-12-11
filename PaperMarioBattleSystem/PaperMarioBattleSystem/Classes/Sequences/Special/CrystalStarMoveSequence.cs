using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base sequence for Crystal Star moves.
    /// </summary>
    public abstract class CrystalStarMoveSequence : SpecialMoveSequence
    {
        protected double WaitTime = 1500d;

        public CrystalStarMoveSequence(SpecialMoveAction specialAction) : base(specialAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //NOTE: Mario moves up a bit when he's in the front, I haven't confirmed how it works in the back yet

                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    //NOTE: Show all Crystal Stars coming from the map with VFX and such

                    User.AnimManager.PlayAnimation(AnimationGlobals.MarioBattleAnimations.MapLiftName);
                    CurSequenceAction = new WaitSeqAction(WaitTime);
                    break;
                case 2:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
