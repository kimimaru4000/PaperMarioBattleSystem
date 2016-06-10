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
    /// Mario's Hammer action
    /// </summary>
    public class Hammer : BattleAction
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

        public Hammer()
        {
            Name = "Hammer";
            Description = "Whack an enemy with your Hammer.";
            SelectionType = TargetSelectionMenu.EntitySelectionType.First;
            ContactType = Enumerations.ContactTypes.HammerContact;
            BaseDamage = 1;
            HeightsAffected = new HeightStates[] { HeightStates.Grounded };

            Command = new HammerCommand(this);
        }

        public override void OnCommandSuccess()
        {
            DamageMod *= 2;

            ChangeSequenceBranch(SequenceBranch.Success);
        }

        public override void OnCommandFailed()
        {
            ChangeSequenceBranch(SequenceBranch.Failed);
        }

        public override void OnCommandResponse(int response)
        {
            if (response == LitWindupSpeed)
            {
                Animation windupAnim = User.GetAnimation(WindupAnimName);
                windupAnim?.SetSpeed(2f);
            }
        }

        protected override void SequenceStartBranch()
        {
            switch(SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequence = new MoveTo(BattleManager.Instance.GetPositionInFront(EntitiesAffected[0]), WalkDuration);
                    break;
                case 1:
                    User.PlayAnimation(PickupAnimName, true);
                    CurSequence = new WaitForAnimation(PickupAnimName);
                    ChangeSequenceBranch(SequenceBranch.Command);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceCommandBranch()
        {
            switch(SequenceStep)
            {
                case 0:
                    User.PlayAnimation(WindupAnimName);
                    if (CommandEnabled == true) Command.StartInput();
                    CurSequence = new WaitForCommand(1500f, Command, CommandEnabled);
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
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected);
                    CurSequence = new WaitForAnimation(SlamAnimName);
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
                    AttemptDamage(BaseDamage * DamageMod, EntitiesAffected);
                    CurSequence = new WaitForAnimation(SlamAnimName);
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

        protected override void SequenceBackfireBranch()
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
