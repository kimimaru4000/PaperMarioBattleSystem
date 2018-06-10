using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Sequence for Parakarry's Air Lift move.
    /// </summary>
    public class AirLiftSequence : Sequence
    {
        private double MoveDur = 1000d;
        private double FailMoveDur = 200d;
        private double FailWaitDur = 300d;
        private double LiftMoveDur = 3500d;
        private double EndMoveDur = 1400d;

        /// <summary>
        /// The chance of Air Lift being successful. This comes in from a response the Action Command sends.
        /// </summary>
        private double SuccessPercentage = 0d;

        private FillBarActionCommandUI<AirLiftCommand> AirLiftUI = null;

        public AirLiftSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            if (Action.DrawActionCommandInfo == true)
            {
                AirLiftUI = new FillBarActionCommandUI<AirLiftCommand>(actionCommand as AirLiftCommand, new Vector2(250, 150), new Vector2(100f, 1f), null);
                User.BManager.battleUIManager.AddUIElement(AirLiftUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            SuccessPercentage = 0d;

            if (AirLiftUI != null)
            {
                User.BManager.battleUIManager.RemoveUIElement(AirLiftUI);
                AirLiftUI = null;
            }
        }

        protected override void CommandSuccess()
        {
            
        }

        protected override void CommandFailed()
        {
            
        }

        public override void OnCommandResponse(in object response)
        {
            SuccessPercentage = (double)response;
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);

                    CurSequenceAction = new MoveToSeqAction(User, EntitiesAffected[0].Position + new Vector2(0f, -10f), MoveDur, Interpolation.InterpolationTypes.QuadInOut, Interpolation.InterpolationTypes.Linear);
                    ChangeSequenceBranch(SequenceBranch.Main);
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

                    CurSequenceAction = new MoveToSeqAction(User, User.BattlePosition, EndMoveDur, Interpolation.InterpolationTypes.Linear, Interpolation.InterpolationTypes.Linear);
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

        protected override void SequenceMainBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Attempt to latch onto the BattleEntity
                    //This is essentially a check for BattleEntities that are Spiked, Electrified, etc., as Air Lift can't grab them
                    AttemptDamage(0, EntitiesAffected[0], Action.DamageProperties, true);

                    CurSequenceAction = new WaitSeqAction(300d);
                    break;
                case 1:
                    //We latched, so start the Action Command
                    User.AnimManager.PlayAnimation(AnimationGlobals.ParakarryBattleAnimations.AirLiftName);

                    //Get the percentage of the BattleEntity being afflicted with Lifted
                    //This is used to scale the difficulty of performing Air Lift's Action Command
                    double percentage = EntitiesAffected[0].EntityProperties.GetStatusProperty(Enumerations.StatusTypes.Lifted).StatusPercentage / 100d;

                    //Manually do this; don't go to the Failed branch unless we fail to pick up the BattleEntity
                    if (CommandEnabled == true)
                    {
                        actionCommand.StartInput(percentage);
                    }

                    CurSequenceAction = new WaitForCommandSeqAction(3500d, actionCommand, CommandEnabled);
                    break;
                case 2:
                    //Check if we can pick up the BattleEntity using the success percentage from the Action Command
                    bool pickedUp = UtilityGlobals.TestRandomCondition(SuccessPercentage);

                    CurSequenceAction = new WaitSeqAction(0d);

                    //If we didn't, go to the Failed branch
                    if (pickedUp == false)
                    {
                        ChangeSequenceBranch(SequenceBranch.Failed);
                    }
                    //We did, so go to the Success branch
                    else
                    {
                        ChangeSequenceBranch(SequenceBranch.Success);
                    }
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //This branch serves for succeeding in lifting the BattleEntity
        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Move offscreen
                    CurSequenceAction = new MoveToSeqAction(User, new Vector2(RenderingGlobals.BaseResolutionWidth + 100f, User.Position.Y - 50), LiftMoveDur);
                    AddSideSeqAction(new FollowSeqAction(EntitiesAffected[0], User, LiftMoveDur, new Vector2(0f, 10f)));
                    break;
                case 1:
                    //Remove the BattleEntity from battle and kill it
                    User.BManager.RemoveEntity(EntitiesAffected[0], true);
                    EntitiesAffected[0].Die();

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Air Lift's Action Command always succeeds; what happens depends on how well it was performed
        //This branch serves for failing to lift the BattleEntity
        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    User.AnimManager.PlayAnimation(AnimationGlobals.RunningName);
                    CurSequenceAction = new MoveAmountSeqAction(User, new Vector2(0f, -10f), FailMoveDur);
                    break;
                case 1:
                    CurSequenceAction = new WaitSeqAction(FailWaitDur);

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
                //NOTE: For now, go to the end if you miss
                //Find out what actually happens when you miss, if you can miss to begin with
                case 0:
                    CurSequenceAction = new WaitSeqAction(0d);

                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
