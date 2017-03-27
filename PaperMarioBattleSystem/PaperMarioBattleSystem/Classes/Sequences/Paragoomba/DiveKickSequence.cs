using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for a Paragoomba's Dive Kick move.
    /// </summary>
    public class DiveKickSequence : Sequence
    {
        public DiveKickSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(new Vector2(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]).X, User.BattlePosition.Y), 700d);
                    ChangeSequenceBranch(SequenceBranch.Main);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceEndBranch()
        {
            switch (SequenceStep)
            {
                //Go back to your battle position
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(new Vector2(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]).X, User.BattlePosition.Y), 500d);
                    break;
                case 1:
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, 400d);
                    break;
                case 2:
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                //Perform the dive kick movement
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.ParagoombaBattleAnimations.DiveKickName);

                    //Move up a bit before swooping down
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, -25f), 350d);
                    break;
                case 1:
                    //Move back down
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, 25f), 350d);
                    break;
                case 2:
                    //Swoop in
                    CurSequenceAction = new MoveToSeqAction(new Vector2(EntitiesAffected[0].Position.X, User.Position.Y - 10), 500d);

                    //Here's likely where you'd check for an action command's input if this were to have one - it'd be like Jump
                    break;
                case 3:
                    //Damage
                    AttemptDamage(BaseDamage, EntitiesAffected, Action.DamageProperties, false);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
