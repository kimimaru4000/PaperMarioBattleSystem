using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Quake Hammer
    /// </summary>
    public class QuakeHammerSequence : HammerSequence
    {
        public QuakeHammerSequence(MoveAction moveAction) : base(moveAction, 0)
        {
            WalkDuration = 500f;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User, BattleManagerUtils.GetPositionInFront(User, User.EntityType != Enumerations.EntityTypes.Player), WalkDuration);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(PickupAnimName, true);
                    CurSequenceAction = new WaitForAnimationSeqAction(User, PickupAnimName);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
