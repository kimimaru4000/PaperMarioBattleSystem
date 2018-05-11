using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Duplighost's Headbutt move.
    /// </summary>
    public class HeadbuttSequence : Sequence
    {
        private double WaitDur = 1000d;
        private double MoveDur = 500d;

        private float YHeight = -20f;

        public HeadbuttSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.DuplighostBattleAnimations.HeadbuttStartName);
                    CurSequenceAction = new WaitSeqAction(WaitDur);

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
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, MoveDur);
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

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.DuplighostBattleAnimations.HeadbuttName);
                    CurSequenceAction = new MoveToSeqAction(new Vector2(EntitiesAffected[0].Position.X, EntitiesAffected[0].Position.Y + YHeight), MoveDur);
                    break;
                case 1:
                    AttemptDamage(Action.DamageProperties.Damage, EntitiesAffected[0], Action.DamageProperties, false);

                    Vector2 pos = BattleManagerUtils.GetPositionInFront(EntitiesAffected[0], User.EntityType == Enumerations.EntityTypes.Player);
                    CurSequenceAction = new MoveToSeqAction(pos, MoveDur / 2d, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                    break;
                case 2:
                    User.AnimManager.PlayAnimation(AnimationGlobals.IdleName);

                    CurSequenceAction = new WaitSeqAction(WaitDur / 5d);
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
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
