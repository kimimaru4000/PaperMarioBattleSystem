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
        public float JumpDuration = 800f;
        public float JumpHeight = 100f;

        public virtual int DamageDealt => BaseDamage;
        public virtual BattleEntity CurTarget => EntitiesAffected[0];

        protected float XDiffOverTwo => (CurTarget.Position.X - User.Position.X) / 2f;

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
                    Vector2 frontPos = BattleManager.Instance.GetPositionInFront(BattleManager.Instance.GetFrontmostBattleEntity(CurTarget.EntityType, null), User.EntityType != EntityTypes.Enemy);

                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(frontPos, WalkDuration);
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
                    Vector2 posTo = User.Position + new Vector2(XDiffOverTwo, -JumpHeight);

                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
                    CurSequenceAction = new MoveToSeqAction(posTo, JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);
                    break;
                case 1:
                    Vector2 posTo2 = User.Position + new Vector2(XDiffOverTwo, JumpHeight);

                    StartActionCommandInput();
                    CurSequenceAction = new MoveToSeqAction(posTo2, JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
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
                    AttemptDamage(DamageDealt, CurTarget, Action.DamageProperties, false);
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, -JumpHeight), JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);
                    break;
                case 1:
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                    break;
                case 2:
                    AttemptDamage(DamageDealt, CurTarget, Action.DamageProperties, false);
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
                    AttemptDamage(DamageDealt, CurTarget, Action.DamageProperties, false);
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

        protected override bool OnMiss()
        {
            base.OnMiss();
            ChangeJumpBranch(SequenceBranch.Miss);

            return false;
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.SpikedTipHurtName, true);

                    Vector2 pos = BattleManager.Instance.GetPositionInFront(CurTarget, User.EntityType != Enumerations.EntityTypes.Player) + new Vector2(-50, -JumpHeight);
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpMissName, true);
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
