using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Mario's Hammer move.
    /// </summary>
    public class HammerSequence : Sequence
    {
        protected float WalkDuration = 1000f;
        protected int DamageMod = 1;

        /// <summary>
        /// The number of lights lit at which the hammer windup animation's speed increases
        /// </summary>
        protected int LitWindupSpeed = 3;

        protected string PickupAnimName = AnimationGlobals.MarioBattleAnimations.HammerPickupName;
        protected string WindupAnimName = AnimationGlobals.MarioBattleAnimations.HammerWindupName;
        protected string SlamAnimName = AnimationGlobals.MarioBattleAnimations.HammerSlamName;

        public HammerSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void CommandSuccess()
        {
            DamageMod *= 2;

            ChangeSequenceBranch(SequenceBranch.Success);
        }

        protected override void CommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(object response)
        {
            if ((int)response == LitWindupSpeed)
            {
                Animation windupAnim = User.AnimManager.GetAnimation(WindupAnimName);
                windupAnim?.SetSpeed(2f);
            }
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0], User.EntityType != Enumerations.EntityTypes.Enemy), WalkDuration);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(PickupAnimName, true);
                    CurSequenceAction = new WaitForAnimationSeqAction(PickupAnimName);
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
                    User.AnimManager.PlayAnimation(WindupAnimName);
                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommandSeqAction(1500f, actionCommand, CommandEnabled);
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
                    User.AnimManager.PlayAnimation(SlamAnimName, true);
                    InteractionResult[] interactions = AttemptDamage(BaseDamage * DamageMod, EntitiesAffected, Action.DamageProperties, false);

                    //Show the command result if you hit
                    if (interactions[0] != null && interactions[0].WasVictimHit == true && interactions[0].WasAttackerHit == false)
                        ShowCommandRankVFX(HighestCommandRank, EntitiesAffected[0].Position);

                    CurSequenceAction = new WaitForAnimationSeqAction(SlamAnimName);
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
                    User.AnimManager.PlayAnimation(SlamAnimName, true);
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected, Action.DamageProperties, false);
                    CurSequenceAction = new WaitForAnimationSeqAction(SlamAnimName);
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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim(), true);
                    EndSequence();
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
