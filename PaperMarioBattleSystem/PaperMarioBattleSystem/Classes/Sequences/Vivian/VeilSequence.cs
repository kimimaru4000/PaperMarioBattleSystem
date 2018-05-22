using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The first half of the Sequence for Vivian's Veil move.
    /// </summary>
    public sealed class VeilSequence : Sequence
    {
        private const double MoveTime = 500d;

        /// <summary>
        /// The BattleEntity using the action.
        /// </summary>
        private BattleEntity EntityUsing = null;

        /// <summary>
        /// The ally affected by the action.
        /// </summary>
        private BattleEntity AllyAffected = null;

        private float ScaleVal = .000001f;

        private MultiButtonActionCommandUI VeilUI = null;

        public VeilSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            EntityUsing = User;
            AllyAffected = EntitiesAffected[0];

            if (Action.CommandEnabled == true && Action.DrawActionCommandInfo == true)
            {
                VeilUI = new MultiButtonActionCommandUI(actionCommand as MultiButtonCommand);
                BattleUIManager.Instance.AddUIElement(VeilUI);
            }
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (VeilUI != null)
            {
                BattleUIManager.Instance.RemoveUIElement(VeilUI);
                VeilUI = null;
            }
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Move behind the ally
                    Vector2 movePosition = AllyAffected.BattlePosition;

                    movePosition.X -= 25;

                    CurSequenceAction = new MoveToSeqAction(movePosition, MoveTime);
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
                    //Start the Action Command if it's enabled
                    //If not enabled, make it succeed automatically
                    if (CommandEnabled == true)
                    {
                        actionCommand.StartInput();
                    }
                    else
                    {
                        OnCommandSuccess();
                    }

                    CurSequenceAction = new WaitForCommandSeqAction(500d, actionCommand, CommandEnabled);
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
                    //Move down
                    //NOTE: Move the ally as well - this needs to be added in
                    CurSequenceAction = new MoveToSeqAction(EntityUsing.Position + new Vector2(0f, 25f), MoveTime);

                    break;
                case 1:
                    //Scale down the characters to almost 0; that is how it works in the actual game
                    EntityUsing.Scale *= ScaleVal;
                    AllyAffected.Scale *= ScaleVal;
                    
                    //Add evasion so attacks against these entities always miss
                    EntityUsing.AddEvasionMod(0d);
                    AllyAffected.AddEvasionMod(0d);

                    //Add Effects suppression to the Poison, Burn, and Frozen Status Effects
                    EntityUsing.EntityProperties.SuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);
                    AllyAffected.EntityProperties.SuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);

                    CurSequenceAction = new WaitSeqAction(0d);
                    break;
                case 2:
                    //Set up the entities for the second phase of the move
                    EntityUsing.TurnStartEvent -= OnUserTurnStart;
                    EntityUsing.TurnStartEvent += OnUserTurnStart;

                    //Skip the ally's turns until the user comes back up
                    AllyAffected.TurnStartEvent -= AllyAffectedTurnStartHandler;
                    AllyAffected.TurnStartEvent += AllyAffectedTurnStartHandler;

                    //Go to the end of the sequence
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
                    //The user goes back to its battle position when failing and ends the sequence
                    CurSequenceAction = new MoveToSeqAction(EntityUsing.BattlePosition, MoveTime);

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
                    //End the sequence
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Veil doesn't do any damage so it can't miss
        protected override void SequenceMissBranch()
        {
            switch (SequenceStep)
            {
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        //Event handler when the user's next turn is started
        private void OnUserTurnStart()
        {
            AllyAffected.TurnStartEvent -= AllyAffectedTurnStartHandler;
            EntityUsing.TurnStartEvent -= OnUserTurnStart;

            Debug.Log($"Starting second phase of {nameof(VeilAction)} for {EntityUsing.Name}!");

            //Clear the menu stack as the action will be selected automatically
            BattleUIManager.Instance.ClearMenuStack();

            //Immediately start the second half of the sequence
            MoveAction veilSecondHalf = new MoveAction("Veil Second Half",
                new MoveActionData(null, "Second half of Veil", Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.First,
                false, null), new VeilSecondHalfSequence(null, ScaleVal));

            //Start the second half of the sequence
            EntityUsing.StartAction(veilSecondHalf, true, AllyAffected);
        }

        //Event handler when the ally who is in Veil starts their next turn
        //Ignore its turn as it needs to wait for the entity that initiated it to finish
        //We don't simply set turn count to 0, as we need to make the ally's turn start right before ending it
        //This allows commands like Defend to end on the ally's next turn even if it can't move yet
        private void AllyAffectedTurnStartHandler()
        {
            Debug.Log($"Skipped {AllyAffected.Name}'s turn for {nameof(VeilAction)} as it's being protected by {EntityUsing.Name}!");

            //Clear the menu stack - the ally can't move yet
            BattleUIManager.Instance.ClearMenuStack();

            //Make the ally do nothing on each of its turns
            AllyAffected.StartAction(new NoAction(), true, null);
        }
    }
}
