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

        public HammerSequence()
        {
            //Name = "Hammer";
            //Description = "Whack an enemy with your Hammer.";
            //SelectionType = TargetSelectionMenu.EntitySelectionType.First;
            //ContactType = Enumerations.ContactTypes.HammerContact;
            //BaseDamage = (int)User.BattleStats.GetHammerLevel;
            //HeightsAffected = new HeightStates[] { HeightStates.Grounded };
            //
            //actionCommand = new HammerCommand(this, 4, 500d);
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
                Animation windupAnim = User.GetAnimation(WindupAnimName);
                windupAnim?.SetSpeed(2f);
            }
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(PickupAnimName, true);
                    CurSequenceAction = new WaitForAnimation(PickupAnimName);
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
                    User.PlayAnimation(WindupAnimName);
                    StartActionCommandInput();
                    CurSequenceAction = new WaitForCommand(1500f, actionCommand, CommandEnabled);
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
                    User.PlayAnimation(SlamAnimName, true);
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected, false);
                    CurSequenceAction = new WaitForAnimation(SlamAnimName);
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
                    User.PlayAnimation(SlamAnimName, true);
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected, false);
                    CurSequenceAction = new WaitForAnimation(SlamAnimName);
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
                    CurSequenceAction = new MoveTo(User.BattlePosition, WalkDuration);
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

        }
    }
}
