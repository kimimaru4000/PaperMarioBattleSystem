using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Mario's Jump move.
    /// </summary>
    public class JumpSequence : Sequence
    {
        public float WalkDuration = 1000f;
        public float JumpDuration = 1000f;
        public float JumpHeight = 100f;

        public virtual int DamageDealt => BaseDamage;
        public virtual BattleEntity CurTarget => EntitiesAffected[0];

        public JumpSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void CommandSuccess()
        {
            //Show "NICE" here or something
            ChangeSequenceBranch(SequenceBranch.Success);
        }

        protected override void CommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(object response)
        {

        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(BattleManager.Instance.GetPositionInFront(CurTarget), WalkDuration);
                    ChangeSequenceBranch(SequenceBranch.Main);
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
                    User.PlayAnimation(User.GetIdleAnim());
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, -JumpHeight), JumpDuration);
                    break;
                case 1:
                    StartActionCommandInput();
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration);
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
                    AttemptDamage(DamageDealt, CurTarget, false);
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, -JumpHeight), JumpDuration);
                    break;
                case 1:
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration);
                    break;
                case 2:
                    AttemptDamage(DamageDealt, CurTarget, false);
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
                    AttemptDamage(DamageDealt, CurTarget, false);
                    ChangeSequenceBranch(SequenceBranch.End);
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
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(User.GetIdleAnim());
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void OnMiss()
        {
            base.OnMiss();
            ChangeJumpBranch(SequenceBranch.Miss);
        }

        protected override void OnInterruption(Elements element)
        {
            if (element == Elements.Sharp) InterruptionHandler = SpikedEntityInterruption;
            else base.OnInterruption(element);
        }

        /// <summary>
        /// The interruption that occurs when jumping on a Spiked entity
        /// </summary>
        protected void SpikedEntityInterruption()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.SpikedTipHurtName, true);

                    Vector2 pos = BattleManager.Instance.GetPositionInFront(CurTarget) + new Vector2(-50, -JumpHeight);
                    CurSequenceAction = new MoveToSeqAction(pos, WalkDuration / 4d);
                    break;
                case 1:
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.SpikedTipHurtName);
                    break;
                case 2:
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration / 2f);
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
                    User.PlayAnimation(AnimationGlobals.JumpMissName, true);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.JumpMissName);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
