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
            InSequence = true;

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
        /// by speeding up Mario's hammer windup animation
        /// </para>
        /// </summary>
        /// <param name="response">A number representing a response from the action command</param>
        public abstract void OnCommandResponse(int response);

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
            BattleManager.Instance.EntityTurn.StartAction(this, targets);
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
        /// A separate method for the top-most base for progressing the sequence.
        /// Move the entity back to its battle position
        /// </summary>
        protected void OnProgressBase()
        {
            switch (SequenceStep)
            {
                default:
                    CurSequence = new MoveTo(User.BattlePosition, 1000f);
                    break;
            }
        }

        /// <summary>
        /// What occurs next in the sequence when it's progressed.
        /// <para>Base functionality moves the entity back to its battle position</para>
        /// </summary>
        protected virtual void OnProgressSequence()
        {
            OnProgressBase();
        }

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
