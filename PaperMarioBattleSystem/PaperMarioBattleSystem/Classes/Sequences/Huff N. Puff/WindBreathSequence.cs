using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class WindBreathSequence : Sequence
    {
        private int DamageDealt = 0;

        private MashButtonRangeActionCommandUI WindBreathUI = null;

        public WindBreathSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            DamageDealt = 0;

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                WindBreathUI = new MashButtonRangeActionCommandUI(actionCommand as MashButtonRangeCommand);
                BattleUIManager.Instance.AddUIElement(WindBreathUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            DamageDealt = 0;

            if (WindBreathUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(WindBreathUI);
                WindBreathUI = null;
            }
        }

        public override void OnCommandResponse(in object response)
        {
            DamageDealt = (int)response;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.HuffNPuffBattleAnimations.InhaleName);
                    CurSequenceAction = new WaitForAnimationSeqAction(AnimationGlobals.HuffNPuffBattleAnimations.InhaleName);
                    break;
                case 1:
                    User.AnimManager.PlayAnimation(AnimationGlobals.HuffNPuffBattleAnimations.ExhaleName);

                    DamageData damageData = Action.DamageProperties;
                    damageData.Damage = 0;

                    //Attempt 0 damage to see if you miss
                    //If not, continue to the Main branch
                    AttemptDamage(0, EntitiesAffected[0], damageData, true);

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
                    StartActionCommandInput();

                    CurSequenceAction = new WaitForCommandSeqAction(1000d, actionCommand, CommandEnabled);
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
                    User.AnimManager.PlayAnimation(User.GetIdleAnim(), true);

                    CurSequenceAction = new WaitSeqAction(300d);
                    break;
                case 1:
                    EndSequence();
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
                    AttemptDamage(DamageDealt, EntitiesAffected[0], Action.DamageProperties, false);
                    CurSequenceAction = new WaitSeqAction(0d);

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
                    CurSequenceAction = new WaitSeqAction(0d);

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
                    CurSequenceAction = new WaitSeqAction(1000d);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
