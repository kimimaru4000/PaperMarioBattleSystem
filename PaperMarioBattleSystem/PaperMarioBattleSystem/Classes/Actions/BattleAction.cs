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
    /// An action that is performed by an entity in battle
    /// </summary>
    public abstract class BattleAction
    {
        /// <summary>
        /// Values for each branch of a sequence.
        /// BattleActions switch branches based on what happens
        /// </summary>
        public enum SequenceBranch
        {
            Start, End, Command, Success, Failed, Backfire
        }

        /// <summary>
        /// The name of the action
        /// </summary>
        public string Name { get; protected set; } = "Action";

        /// <summary>
        /// The icon representing the action
        /// </summary>
        public Texture2D Icon { get; protected set; } = null;

        /// <summary>
        /// How much FP it costs to use the action
        /// </summary>
        public int FPCost { get; protected set; } = 0;

        /// <summary>
        /// The base damage of the action
        /// </summary>
        public int BaseDamage { get; protected set; } = 0;

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
        /// The type of contact this action makes
        /// </summary>
        public ContactTypes ContactType { get; protected set; } = ContactTypes.None;

        /// <summary>
        /// The heights of enemies this action affects
        /// </summary>
        public HeightStates[] HeightsAffected { get; protected set; } = null;

        /// <summary>
        /// The user of this action.
        /// Aside from the Guard action, it will be the entity whose turn it currently is
        /// </summary>
        public virtual BattleEntity User => BattleManager.Instance.EntityTurn;

        /// <summary>
        /// The ActionCommand associated with the BattleAction
        /// </summary>
        public ActionCommand Command { get; protected set; } = null;

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

        protected BattleEntity[] EntitiesAffected { get; private set; } = null;

        /// <summary>
        /// Tells whether the action command is enabled or not.
        /// Action commands are always disabled for enemies
        /// </summary>
        protected bool CommandEnabled => (Command != null && User.EntityType != EntityTypes.Enemy);

        protected BattleAction()
        {
            
        }

        // <summary>
        // TEMPORARY NAME AND LOCATION FOR THIS
        // </summary>
        // <param name="entityToAttack"></param>
        // <returns>true if the interaction was successful, false otherwise</returns>
        //protected bool HandleInteraction(BattleEntity entityToAttack)
        //{
        //    //Check if you successfully jump on the entity
        //    if (ContactType == ContactTypes.JumpContact)
        //    {
        //        if (entityToAttack.HasPhysAttributes(true,
        //            PhysicalAttributes.Spiked, PhysicalAttributes.Burning, PhysicalAttributes.Electrified) == true)
        //        {
        //            //Get hurt, not successful jump
        //            OnCommandBackfire();
        //            return false;
        //        }
        //    }
        //
        //    return true;
        //}

        public void DealDamage(int damage)
        {
            for (int i = 0; i < EntitiesAffected.Length; i++)
            {
                EntitiesAffected[i].LoseHP(damage);
            }
        }

        /// <summary>
        /// Starts the action sequence
        /// </summary>
        /// <param name="targets">The targets to perform the sequence on</param>
        public void StartSequence(params BattleEntity[] targets)
        {
            CurBranch = SequenceBranch.Start;
            InSequence = true;
            SequenceStep = 0;

            EntitiesAffected = targets;

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
        public void EndSequence()
        {
            CurBranch = SequenceBranch.End;
            InSequence = false;
            SequenceStep = 0;
            CurSequence = null;

            EntitiesAffected = null;

            OnEnd();

            if (User == BattleManager.Instance.EntityTurn)
            {
                User.UsedTurn = true;
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
        /// What occurs when the action command is successfully performed
        /// </summary>
        public abstract void OnCommandSuccess();

        /// <summary>
        /// What occurs when the action command is failed
        /// </summary>
        public abstract void OnCommandFailed();

        /// <summary>
        /// Handles BattleAction responses sent from an ActionCommand that are not a definite Success or Failure.
        /// Unlike a Success or Failure, the ActionCommand is not required to send this down at all
        /// <para>For example, the Hammer command sends back the number of lights lit up, and the Hammer action responds
        /// by speeding up Mario's hammer windup animation</para>
        /// </summary>
        /// <param name="response">A number representing a response from the action command</param>
        public abstract void OnCommandResponse(int response);

        /// <summary>
        /// What happens when the BattleAction is selected on the menu.
        /// The default behavior is to start target selection with the ActionStart method
        /// </summary>
        public virtual void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection(ActionStart, SelectionType, BattleManager.Instance.GetEntities(EntityType, HeightsAffected));
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
        public void ProgressSequence(uint progressAmount)
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
        /// What occurs next in the sequence when it's progressed.
        /// </summary>
        protected void OnProgressSequence()
        {
            switch (CurBranch)
            {
                case SequenceBranch.Start:
                    SequenceStartBranch();
                    break;
                case SequenceBranch.Command:
                    SequenceCommandBranch();
                    break;
                case SequenceBranch.Success:
                    SequenceSuccessBranch();
                    break;
                case SequenceBranch.Failed:
                    SequenceFailedBranch();
                    break;
                case SequenceBranch.Backfire:
                    SequenceBackfireBranch();
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
        /// The part of the action sequence revolving around the action command
        /// </summary>
        protected abstract void SequenceCommandBranch();
        
        /// <summary>
        /// What occurs when the action command for this action is performed successfully
        /// </summary>
        protected abstract void SequenceSuccessBranch();

        /// <summary>
        /// What occurs when the action command for this action is failed
        /// </summary>
        protected abstract void SequenceFailedBranch();

        /// <summary>
        /// What occurs when the action backfires.
        /// The most notable example of this is when Mario takes damage from jumping on a spiked enemy
        /// </summary>
        protected abstract void SequenceBackfireBranch();

        public void Update()
        {
            //Perform sequence
            if (InSequence == true)
            {
                //If the action command is enabled, let it handle the sequence
                if (CommandEnabled == true)
                {
                    if (Command.AcceptingInput == true)
                        Command.Update();
                }

                CurSequence.Update();
                if (CurSequence.IsDone == true)
                {
                    ProgressSequence(1);
                }
            }
        }

        public void Draw()
        {
            if (InSequence == true)
            {
                if (CommandEnabled == true)
                {
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font,
                    $"Command: {Name} performed by {User.Name}",
                    new Vector2(SpriteRenderer.Instance.WindowCenter.X, 50f), Color.Black, 0f, new Vector2(.5f, .5f), 1.1f, .9f, true);

                    Command?.Draw();
                }
            }
        }
    }
}
