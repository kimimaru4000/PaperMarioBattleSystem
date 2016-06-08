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
    /// Mario's Jump action
    /// </summary>
    public class Jump : BattleAction
    {
        protected float WalkDuration = 1000f;
        protected float JumpDuration = 1000f;
        protected float JumpHeight = 100f;

        protected virtual int DamageDealt => BaseDamage;

        public Jump()
        {
            Name = "Jump";
            Description = "Jump and stomp on an enemy.";
            ContactType = Enumerations.ContactTypes.JumpContact;
            BaseDamage = 1;

            Command = new JumpCommand(this, JumpDuration, (int)(JumpDuration / 2f));
            HeightsAffected = new Enumerations.HeightStates[] { HeightStates.Grounded, HeightStates.Airborne };
        }

        public override void OnCommandSuccess()
        {
            //Show "NICE" here or something
            ChangeSequenceBranch(SequenceBranch.Success);
        }

        public override void OnCommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(int response)
        {
            
        }

        public override void OnMenuSelected()
        {
            base.OnMenuSelected();
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), WalkDuration);
                    ChangeSequenceBranch(SequenceBranch.Command);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceCommandBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    CurSequence = new MoveAmount(new Vector2(0f, -JumpHeight), JumpDuration);
                    break;
                case 1:
                    if (CommandEnabled == true) Command.StartInput();
                    else ChangeSequenceBranch(SequenceBranch.Failed);
                    CurSequence = new MoveAmount(new Vector2(0f, JumpHeight), JumpDuration);
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
                    AttemptDamage(DamageDealt, EntitiesAffected);
                    CurSequence = new MoveAmount(new Vector2(0f, -JumpHeight), JumpDuration);
                    break;
                case 1:
                    CurSequence = new MoveAmount(new Vector2(0f, JumpHeight), JumpDuration);
                    break;
                case 2:
                    ChangeSequenceBranch(SequenceBranch.End);
                    AttemptDamage(DamageDealt, EntitiesAffected);
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
                    AttemptDamage(DamageDealt, EntitiesAffected);
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
                    CurSequence = new MoveTo(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName);
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceBackfireBranch()
        {
            switch(SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.SpikedTipHurtName, true);
                    
                    Vector2 pos = BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]) + new Vector2(-50, -JumpHeight);
                    CurSequence = new MoveTo(pos, WalkDuration / 4d);
                    break;
                case 1:
                    CurSequence = new Wait(JumpDuration / 2f);
                    break;
                case 2:
                    CurSequence = new MoveAmount(new Vector2(0f, JumpHeight), JumpDuration / 2f);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
