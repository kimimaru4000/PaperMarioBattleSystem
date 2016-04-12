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
        /// The user of this action
        /// </summary>
        public BattleEntity User { get; protected set; } = null;

        /// <summary>
        /// The ActionCommand associated with the BattleAction
        /// </summary>
        public ActionCommand Command { get; protected set; } = null;

        /// <summary>
        /// Whether the action's sequence is being performed or not
        /// </summary>
        public bool InSequence { get; protected set; } = false;

        /// <summary>
        /// The default time it takes for the action to perform its effects.
        /// ActionCommands can cause the action to take longer or shorter to complete.
        /// This time always occurs for enemies, as they cannot perform ActionCommands
        /// </summary>
        public float BaseLength { get; protected set; } = 1000f;

        /// <summary>
        /// The time the action sequence should end, after it started, without action commands
        /// </summary>
        public float SequenceEndTime { get; private set; } = 0f;

        /// <summary>
        /// Whether the action sequence's base time, without action commands, has ended or not
        /// </summary>
        public bool IsSequenceBaseEnded => (float)Time.ActiveMilliseconds >= SequenceEndTime;

        /// <summary>
        /// The Sequence performed
        /// </summary>
        public Sequence ActionSequence { get; protected set; } = null;

        protected BattleEntity[] EntitiesAffected { get; private set; } = null;

        /// <summary>
        /// Tells whether the action command is enabled or not.
        /// Action commands are always disabled for enemies
        /// </summary>
        protected bool CommandEnabled => (Command != null && User.EntityType != EntityTypes.Enemy);

        protected BattleAction()
        {
            //TEMPORARY
            User = BattleManager.Instance.EntityTurn;

            ActionSequence = new Sequence();
        }

        /// <summary>
        /// Sets the user of this action
        /// </summary>
        /// <param name="user">The entity performing this action</param>
        public void SetUser(BattleEntity user)
        {
            User = user;
            ActionSequence.SetEntity(User);
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
            SequenceEndTime = (float)Time.ActiveMilliseconds + BaseLength;

            EntitiesAffected = targets;

            if (CommandEnabled == true)
                Command.StartInput();
        }
        
        /// <summary>
        /// Ends the action sequence
        /// </summary>
        public void EndSequence()
        {
            InSequence = false;
            SequenceEndTime = 0f;

            EntitiesAffected = null;

            if (User == BattleManager.Instance.EntityTurn)
            {
                User.UsedTurn = true;
                User.EndTurn();
            }
        }

        /// <summary>
        /// What occurs when the action command is successfully performed
        /// </summary>
        /// <param name="successRate">How well the action command was performed</param>
        public abstract void OnCommandSuccess(int successRate);

        /// <summary>
        /// What occurs when the action command is failed
        /// </summary>
        public virtual void OnCommandFailed()
        {
            EndSequence();
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
            BattleManager.Instance.EntityTurn.StartAction(this, targets);
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
                //Otherwise, wait for the action to finish
                else if (IsSequenceBaseEnded == true)
                {
                    OnCommandFailed();
                }

                ActionSequence.UpdateSequence();
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
