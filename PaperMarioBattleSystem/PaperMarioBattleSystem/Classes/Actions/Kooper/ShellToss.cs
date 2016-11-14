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
    /// Kooper's Shell Toss action.
    /// </summary>
    public sealed class ShellToss : OffensiveAction
    {
        private float WalkDuration = 500f;
        private float SpinMoveDuration = 1000f;
        private int DamageMod = 1;

        public ShellToss()
        {
            Name = "Shell Toss";
            Description = "Shoot yourself at an enemy.";
            SelectionType = TargetSelectionMenu.EntitySelectionType.First;
            ContactType = Enumerations.ContactTypes.None;
            BaseDamage = 1;
            HeightsAffected = new HeightStates[] { HeightStates.Grounded };

            actionCommand = new HammerCommand(this, 4, 500d);
        }

        protected override void CommandSuccess()
        {
            DamageMod *= 2;
            SpinMoveDuration /= 2f;

            ChangeSequenceBranch(SequenceBranch.Success);
        }

        protected override void CommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(int response)
        {
            
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(BattleManager.Instance.GetMario()), WalkDuration);
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
                    User.PlayAnimation(AnimationGlobals.KooperBattleAnimations.ShellSpinName, true);
                    StartActionCommandInput();
                    CurSequence = new WaitForCommand(1500d, actionCommand, CommandEnabled);
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
                    User.GetAnimation(AnimationGlobals.KooperBattleAnimations.ShellSpinName).SetSpeed(3f);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), SpinMoveDuration);
                    break;
                case 1:
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected[0], false);
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
                    User.GetAnimation(AnimationGlobals.KooperBattleAnimations.ShellSpinName).SetSpeed(2f);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), SpinMoveDuration);
                    break;
                case 1:
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected[0], false);
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
                    User.PlayAnimation(AnimationGlobals.RunningName, true);
                    CurSequence = new MoveTo(User.BattlePosition, WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(AnimationGlobals.IdleName, true);
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
