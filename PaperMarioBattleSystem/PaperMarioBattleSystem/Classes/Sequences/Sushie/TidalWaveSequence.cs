using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Sushie's Tidal Wave.
    /// </summary>
    public class TidalWaveSequence : Sequence
    {
        public float WalkDuration = 700f;
        public int AdditionalDamage = 0;

        private TidalWaveActionCommandUI TidalWaveUI = null;

        public TidalWaveSequence(MoveAction moveAction) : base(moveAction)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                TidalWaveUI = new TidalWaveActionCommandUI(actionCommand as TidalWaveCommand);
                BattleUIManager.Instance.AddUIElement(TidalWaveUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            AdditionalDamage = 0;

            if (TidalWaveUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(TidalWaveUI);
                TidalWaveUI = null;
            }
        }

        protected override void CommandSuccess()
        {
            ChangeSequenceBranch(SequenceBranch.Success);
        }

        protected override void CommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(in object response)
        {
            AdditionalDamage = (int)response;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveToSeqAction(User, BattleManagerUtils.GetPositionInFront(User.BManager.FrontPlayer,
                        User.EntityType != Enumerations.EntityTypes.Player), WalkDuration);
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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim());
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
                    AttemptDamage(BaseDamage + AdditionalDamage, EntitiesAffected, Action.DamageProperties, false);
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
                    AttemptDamage(BaseDamage + AdditionalDamage, EntitiesAffected, Action.DamageProperties, false);
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
                    CurSequenceAction = new MoveToSeqAction(User, User.BattlePosition, WalkDuration);
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

        protected override void SequenceMissBranch()
        {

        }
    }
}
