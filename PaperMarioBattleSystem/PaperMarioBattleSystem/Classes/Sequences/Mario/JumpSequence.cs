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
        public float JumpDuration = 600f;
        public float JumpHeight = 100f;
        public float JumpLandDuration = 300f;

        public virtual int DamageDealt => BaseDamage;
        public virtual BattleEntity CurTarget => EntitiesAffected[0];

        protected float XDiffOverTwo => UtilityGlobals.DifferenceDivided(CurTarget.Position.X, User.Position.X, 2f);

        protected ActionCommand.CommandRank SentRank = ActionCommand.CommandRank.Nice;

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
                    
                    break;
                case 1:

                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpStartName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.JumpStartName);
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

                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpRisingName);
                    CurSequenceAction = new MoveToSeqAction(posTo, JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);

                    SoundManager.Instance.PlaySound(SoundManager.Sound.MarioJump);
                    break;
                case 1:
                    Vector2 posTo2 = User.Position + new Vector2(XDiffOverTwo, JumpHeight);

                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpFallingName);
                    StartActionCommandInput(SentRank);
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpLandName);
                    CurSequenceAction = new WaitSeqAction(JumpLandDuration);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpRisingName);
                    InteractionResult[] interactions = AttemptDamage(DamageDealt, CurTarget, Action.DamageProperties, false);

                    //Show VFX for the highest command rank
                    if (interactions[0] != null && interactions[0].WasVictimHit == true && interactions[0].WasAttackerHit == false)
                    {
                        ShowCommandRankVFX(HighestCommandRank, CurTarget.Position);
                    }

                    //Set Stylish data
                    SetStylishData(200d, 600d, 0);

                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, -JumpHeight), JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);
                    break;
                case 2:
                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpFallingName);
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                    break;
                case 3:
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
                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpLandName);
                    CurSequenceAction = new WaitSeqAction(JumpLandDuration);
                    break;
                case 1:
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
                    //Do a little bounce at the end
                    Vector2 endPos = BattleManager.Instance.GetPositionInFront(CurTarget, User.EntityType == EntityTypes.Player);
                    Vector2 moveAmt = new Vector2(UtilityGlobals.DifferenceDivided(endPos.X, User.Position.X, 2f), -(JumpHeight / 2f));

                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpRisingName);
                    CurSequenceAction = new MoveAmountSeqAction(moveAmt, JumpDuration / 2f, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);
                    break;
                case 1:
                    endPos = BattleManager.Instance.GetPositionInFront(CurTarget, User.EntityType == EntityTypes.Player);

                    moveAmt = new Vector2(UtilityGlobals.DifferenceDivided(endPos.X, User.Position.X, 2f), (JumpHeight / 2f));

                    User.AnimManager.PlayAnimation(AnimationGlobals.JumpFallingName);
                    CurSequenceAction = new MoveAmountSeqAction(moveAmt, JumpDuration / 2f, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                    break;
                case 2:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User.BattlePosition, WalkDuration);
                    break;
                case 3:
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

        protected override void HandleStylishMove(int index)
        {
            base.HandleStylishMove(index);

            if (index == 0)
            {
                User.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.StarSpecialName);
            }
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

                    Vector2 offset = new Vector2(-50, -JumpHeight);
                    if (User.EntityType != EntityTypes.Player)
                        offset.X = -offset.X;

                    Vector2 pos = BattleManager.Instance.GetPositionInFront(CurTarget, User.EntityType != EntityTypes.Player) + offset;
                    CurSequenceAction = new MoveToSeqAction(pos, WalkDuration / 4d);
                    break;
                case 1:
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.SpikedTipHurtName);
                    break;
                case 2:
                    CurSequenceAction = new MoveAmountSeqAction(new Vector2(0f, JumpHeight), JumpDuration / 2f, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);
                    break;
                case 3:
                    //Do the same bounce as the end sequence, except keep playing the same animation
                    Vector2 endPos = BattleManager.Instance.GetPositionInFront(CurTarget, User.EntityType == EntityTypes.Player);
                    Vector2 moveAmt = new Vector2(UtilityGlobals.DifferenceDivided(endPos.X, User.Position.X, 2f), -(JumpHeight / 2f));

                    CurSequenceAction = new MoveAmountSeqAction(moveAmt, JumpDuration / 2f, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadOut);
                    break;
                case 4:
                    endPos = BattleManager.Instance.GetPositionInFront(CurTarget, User.EntityType == EntityTypes.Player);

                    moveAmt = new Vector2(UtilityGlobals.DifferenceDivided(endPos.X, User.Position.X, 2f), (JumpHeight / 2f));

                    CurSequenceAction = new MoveAmountSeqAction(moveAmt, JumpDuration / 2f, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.QuadIn);

                    ChangeSequenceBranch(SequenceBranch.End);

                    SequenceStep = 1;
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

                    //Set the sequence step so it doesn't perform the jump back part of the end sequence
                    SequenceStep = 1;
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
