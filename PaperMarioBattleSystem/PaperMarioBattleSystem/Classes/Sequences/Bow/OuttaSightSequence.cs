using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The first half of the Sequence for Bow's Outta Sight move.
    /// <para>With regards to turn count, this will function identically to Veil because we're using TTYD's partner system.
    /// In PM, partners always move to the back at the start of the enemy phase which caused its behavior in that game.</para>
    /// </summary>
    public sealed class OuttaSightSequence : Sequence
    {
        private const double MoveTime = 500d;

        private float AlphaVal = .3f;

        /// <summary>
        /// The BattleEntity using the action.
        /// </summary>
        private BattleEntity EntityUsing = null;

        /// <summary>
        /// The ally affected by the action.
        /// </summary>
        private BattleEntity AllyAffected = null;

        public OuttaSightSequence(MoveAction moveAction) : base(moveAction)
        {

        }

        protected override void OnStart()
        {
            base.OnStart();

            EntityUsing = User;
            AllyAffected = EntitiesAffected[0];
        }

        protected override void SequenceStartBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    //Move behind the ally
                    Vector2 movePosition = AllyAffected.BattlePosition;

                    //NOTE: Approximately up and behind
                    movePosition.X -= 15;
                    movePosition.Y = User.Position.Y;

                    CurSequenceAction = new MoveToSeqAction(User, movePosition, MoveTime);
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
                    //Add evasion
                    User.AddEvasionMod(0d);
                    AllyAffected.AddEvasionMod(0d);

                    //Turn them both transparent
                    User.TintColor = User.TintColor.CeilingMult(AlphaVal);
                    AllyAffected.TintColor = AllyAffected.TintColor.CeilingMult(AlphaVal);

                    //Suppress these Statuses in these ways
                    //In PM, Outta Sight technically doesn't suppress anything, since Status Effects take place on the character's turn instead
                    //of at the start of each phase cycle
                    //Follow Veil in this regard, since that was designed with TTYD's battle system
                    EntityUsing.EntityProperties.SuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);
                    AllyAffected.EntityProperties.SuppressStatuses(Enumerations.StatusSuppressionTypes.Effects, Enumerations.StatusTypes.Poison, Enumerations.StatusTypes.Burn, Enumerations.StatusTypes.Frozen);
                    EntityUsing.EntityProperties.SuppressStatuses(Enumerations.StatusSuppressionTypes.TurnCount, Enumerations.StatusTypes.Invisible);
                    AllyAffected.EntityProperties.SuppressStatuses(Enumerations.StatusSuppressionTypes.TurnCount, Enumerations.StatusTypes.Invisible);

                    //The ally assumes the Guard position if it's not immobile
                    if (AllyAffected.IsImmobile() == false)
                        AllyAffected.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName);

                    //This half of the sequence ends here
                    ChangeSequenceBranch(SequenceBranch.End);
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
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceFailedBranch()
        {
            switch (SequenceStep)
            {
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
                    //Prepare the next half of the sequence on the user's next turn
                    EntityUsing.TurnStartEvent -= OnUserTurnStart;
                    EntityUsing.TurnStartEvent += OnUserTurnStart;

                    //The ally has its turns skipped until the user finishes
                    AllyAffected.TurnStartEvent -= AllyAffectedTurnStartHandler;
                    AllyAffected.TurnStartEvent += AllyAffectedTurnStartHandler;
                    
                    //End the sequence
                    EndSequence();
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        protected override void SequenceMissBranch()
        {
            switch(SequenceStep)
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

            Debug.Log($"Starting second phase of {nameof(OuttaSightAction)} for {EntityUsing.Name}!");

            //Clear the menu stack as the action will be selected automatically
            BattleUIManager.Instance.ClearMenuStack();

            //Immediately start the second half of the sequence
            MoveAction outtaSightSecondHalf = new MoveAction(User, "Outta Sight Second Half",
                new MoveActionData(null, "Second half of Outta Sight", Enumerations.MoveResourceTypes.FP, 0,
                Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.None, Enumerations.EntitySelectionType.First,
                false, null), new OuttaSightSecondHalfSequence(null, AlphaVal));

            //Start the second half of the sequence
            EntityUsing.StartAction(outtaSightSecondHalf, true, AllyAffected);
        }

        //Event handler when the ally who is hidden starts their next turn
        //Ignore its turn as it needs to wait for the entity that hid them to finish
        //We don't simply set turn count to 0, as we need to make the ally's turn start right before ending it
        //This allows commands like Defend to end on the ally's next turn even if it can't move yet
        private void AllyAffectedTurnStartHandler()
        {
            Debug.Log($"Skipped {AllyAffected.Name}'s turn for {nameof(OuttaSightAction)} as it's being protected by {EntityUsing.Name}!");

            //Clear the menu stack - the ally can't move yet
            BattleUIManager.Instance.ClearMenuStack();

            //Make the ally do nothing on each of its turns
            AllyAffected.StartAction(new NoAction(AllyAffected), true, null);
            AllyAffected.AnimManager.PlayAnimation(AnimationGlobals.PlayerBattleAnimations.GuardName);
        }
    }
}
