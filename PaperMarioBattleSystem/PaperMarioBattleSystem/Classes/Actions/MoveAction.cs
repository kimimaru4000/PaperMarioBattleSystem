using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.TargetSelectionMenu;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A type of BattleAction selectable from a BattleMenu.
    /// They have sequences that the BattleEntity performs.
    /// </summary>
    public abstract class MoveAction : BattleAction
    {
        /// <summary>
        /// Values for each branch of a sequence.
        /// BattleActions switch branches based on what happens.
        /// <para>The None branch is only used to indicate whether to jump to a certain branch after the sequence updates.
        /// The most common use-case is switching to the Interruption branch</para>
        /// </summary>
        public enum SequenceBranch
        {
            None, Start, End, Main, Success, Failed, Interruption, Miss
        }

        /// <summary>
        /// A delegate for handling the sequence interruption branch.
        /// It should follow the same conventions as the other branches
        /// </summary>
        protected delegate void InterruptionDelegate();

        #region Fields/Properties

        /// <summary>
        /// The icon representing the action
        /// </summary>
        public Texture2D Icon { get; protected set; } = null;

        /// <summary>
        /// How much FP it costs to use the action
        /// </summary>
        public int FPCost { get; protected set; } = 0;

        /// <summary>
        /// The description of the action
        /// </summary>
        public string Description { get; protected set; } = "Error";

        /// <summary>
        /// The amount of entities this action can select
        /// </summary>
        public EntitySelectionType SelectionType { get; protected set; } = EntitySelectionType.Single;

        /// <summary>
        /// The type of entities this action selects
        /// </summary>
        public EntityTypes EntityType { get; protected set; } = EntityTypes.Enemy;

        /// <summary>
        /// Whether the action's sequence is being performed or not
        /// </summary>
        public bool InSequence { get; protected set; } = false;

        /// <summary>
        /// The current step of the sequence
        /// </summary>
        public int SequenceStep { get; protected set; } = 0;

        /// <summary>
        /// The current branch of the sequence
        /// </summary>
        protected SequenceBranch CurBranch { get; private set; } = SequenceBranch.Start;

        /// <summary>
        /// The current SequenceAction being performed
        /// </summary>
        protected SequenceAction CurSequence { get; set; } = null;

        /// <summary>
        /// The BattleEntities affected by this action
        /// </summary>
        protected BattleEntity[] EntitiesAffected { get; private set; } = null;

        /// <summary>
        /// A value denoting if we should jump to a particular branch or not after the sequence progresses.
        /// This allows the sequences to remain flexible and not cause any sequence or branch conflicts with this branch
        /// </summary>
        protected SequenceBranch JumpToBranch { get; private set; } = SequenceBranch.None;

        /// <summary>
        /// The handler used for interruptions. This exists so each action can specify different handlers for
        /// different types of damage. It defaults to the base method at the start and end of each action
        /// </summary>
        protected InterruptionDelegate InterruptionHandler = null;

        #endregion

        protected MoveAction()
        {
            InterruptionHandler = BaseInterruptionHandler;
        }

        /// <summary>
        /// Starts the action sequence
        /// </summary>
        /// <param name="targets">The targets to perform the sequence on</param>
        public virtual void StartSequence(params BattleEntity[] targets)
        {
            CurBranch = SequenceBranch.Start;
            InSequence = true;
            SequenceStep = 0;

            ChangeJumpBranch(SequenceBranch.None);

            EntitiesAffected = targets;

            InterruptionHandler = BaseInterruptionHandler;

            OnStart();

            //Start the first sequence
            ProgressSequence(0);
        }

        /// <summary>
        /// BattleAction-specific logic when the action is started
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// Ends the action sequence
        /// </summary>
        public virtual void EndSequence()
        {
            CurBranch = SequenceBranch.End;
            InSequence = false;
            SequenceStep = 0;
            CurSequence = null;

            ChangeJumpBranch(SequenceBranch.None);

            EntitiesAffected = null;

            InterruptionHandler = BaseInterruptionHandler;

            OnEnd();

            if (User == BattleManager.Instance.EntityTurn)
            {
                User.EndTurn();
            }
        }

        /// <summary>
        /// BattleAction-specific logic when the action is complete
        /// </summary>
        protected virtual void OnEnd()
        {

        }

        /// <summary>
        /// What happens when the BattleAction is selected on the menu.
        /// The default behavior is to start target selection with the ActionStart method
        /// </summary>
        public virtual void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection(ActionStart, SelectionType, BattleManager.Instance.GetEntities(EntityType));
        }

        /// <summary>
        /// Clears the menu stack and makes the entity whose turn it is start performing this action
        /// </summary>
        /// <param name="targets"></param>
        private void ActionStart(BattleEntity[] targets)
        {
            BattleUIManager.Instance.ClearMenuStack();
            User.StartAction(this, targets);
        }

        /// <summary>
        /// Prints an error message when an invalid sequence is occurred.
        /// It includes information such as the action and the entity performing it, the sequence branch, and the sequence step
        /// </summary>
        protected void PrintInvalidSequence()
        {
            Debug.LogError($"{User.Name} entered an invalid state in {Name} with a {nameof(SequenceStep)} of {SequenceStep} in {nameof(CurBranch)}: {CurBranch}");
        }

        /// <summary>
        /// Progresses the BattleAction further into its sequence
        /// </summary>
        /// <param name="progressAmount">The amount to progress the sequence</param>
        private void ProgressSequence(uint progressAmount)
        {
            SequenceStep += (int)progressAmount;

            OnProgressSequence();
            if (InSequence == true)
            {
                CurSequence.Start();
            }
        }

        /// <summary>
        /// Switches to a new sequence branch. This also resets the current step
        /// </summary>
        /// <param name="newBranch">The new branch to switch to</param>
        protected void ChangeSequenceBranch(SequenceBranch newBranch)
        {
            CurBranch = newBranch;

            //Set to -1 as it'll be incremented next time the sequence progresses
            SequenceStep = -1;
        }

        /// <summary>
        /// Sets the branch to jump to after the current sequence updates
        /// </summary>
        /// <param name="newJumpBranch">The new branch to jump to</param>
        protected void ChangeJumpBranch(SequenceBranch newJumpBranch)
        {
            JumpToBranch = newJumpBranch;
        }

        /// <summary>
        /// What occurs next in the sequence when it's progressed.
        /// </summary>
        private void OnProgressSequence()
        {
            switch (CurBranch)
            {
                case SequenceBranch.Start:
                    SequenceStartBranch();
                    break;
                case SequenceBranch.Main:
                    SequenceMainBranch();
                    break;
                case SequenceBranch.Success:
                    SequenceSuccessBranch();
                    break;
                case SequenceBranch.Failed:
                    SequenceFailedBranch();
                    break;
                case SequenceBranch.Interruption:
                    SequenceInterruptionBranch();
                    break;
                case SequenceBranch.Miss:
                    SequenceMissBranch();
                    break;
                case SequenceBranch.End:
                default:
                    SequenceEndBranch();
                    break;
            }
        }

        /// <summary>
        /// The start of the action sequence
        /// </summary>
        protected abstract void SequenceStartBranch();

        /// <summary>
        /// The end of the action sequence
        /// </summary>
        protected abstract void SequenceEndBranch();

        /// <summary>
        /// The main part of the action sequence. If the action has an Action Command, this branch will likely incorporate it
        /// </summary>
        protected abstract void SequenceMainBranch();

        /// <summary>
        /// What occurs when the action command for this action is performed successfully
        /// </summary>
        protected abstract void SequenceSuccessBranch();

        /// <summary>
        /// What occurs when the action command for this action is failed
        /// </summary>
        protected abstract void SequenceFailedBranch();

        /// <summary>
        /// What occurs when the action is interrupted.
        /// The most notable example of this is when Mario takes damage from jumping on a spiked enemy
        /// <para>This is overrideable through the InterruptionHandler, as actions can handle this in more than one way</para>
        /// </summary>
        protected void SequenceInterruptionBranch()
        {
            if (InterruptionHandler == null)
            {
                Debug.LogError($"{nameof(InterruptionHandler)} is null for {Name}! This should NEVER happen - look into it ASAP");
                return;
            }

            InterruptionHandler();
        }

        /// <summary>
        /// What occurs when the action misses
        /// </summary>
        protected abstract void SequenceMissBranch();

        /// <summary>
        /// The base interruption handler
        /// </summary>
        protected void BaseInterruptionHandler()
        {
            float moveX = -20f;
            float moveY = 70f;

            double time = 500d;

            switch (SequenceStep)
            {
                case 0:
                    User.PlayAnimation(AnimationGlobals.HurtName, true);

                    Vector2 pos = User.Position + new Vector2(moveX, -moveY);
                    CurSequence = new MoveTo(pos, time / 2d);
                    break;
                case 1:
                    CurSequence = new WaitForAnimation(AnimationGlobals.HurtName);
                    break;
                case 2:
                    CurSequence = new MoveAmount(new Vector2(0f, moveY), time);
                    ChangeSequenceBranch(SequenceBranch.End);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }

        /// <summary>
        /// Starts the interruption, which occurs when a BattleEntity takes damage mid-sequence
        /// </summary>
        /// <param name="element">The elemental damage being dealt</param>
        public virtual void StartInterruption(Elements element)
        {
            ChangeJumpBranch(SequenceBranch.Interruption);

            //Call the action-specific interruption method to set the interruption handler
            OnInterruption(element);
        }

        /// <summary>
        /// How the action handles a miss.
        /// The base implementation is to do nothing, but actions such as Jump may go to the Miss branch
        /// </summary>
        protected virtual void OnMiss()
        {
            Debug.Log($"{User.Name} has missed with the {Name} Action and will act accordingly");
        }

        /// <summary>
        /// Sets the InterruptionHandler based on the type of damage dealt
        /// </summary>
        /// <param name="element">The elemental damage being dealt</param>
        protected virtual void OnInterruption(Elements element)
        {
            InterruptionHandler = BaseInterruptionHandler;
        }

        /// <summary>
        /// What occurs right before the Sequence updates
        /// </summary>
        protected virtual void PreSequenceUpdate()
        {

        }

        public override void Update()
        {
            //Perform sequence
            if (InSequence == true)
            {
                PreSequenceUpdate();

                CurSequence.Update();
                if (CurSequence.IsDone == true)
                {
                    ProgressSequence(1);
                }
            }
        }

        /// <summary>
        /// Handles anything that needs to be done directly after updating the sequence.
        /// This is where it jumps to a new branch, if it should
        /// </summary>
        public void PostUpdate()
        {
            if (InSequence == true && JumpToBranch != SequenceBranch.None)
            {
                //Change the sequence action itself to cancel out anything that it will be waiting for to finish
                //We don't end the previous sequence action because it has been interrupted by the new branch
                CurSequence = new Wait(0d);
                ChangeSequenceBranch(JumpToBranch);

                ChangeJumpBranch(SequenceBranch.None);
            }
        }

        public override void Draw()
        {
            
        }
    }
}
