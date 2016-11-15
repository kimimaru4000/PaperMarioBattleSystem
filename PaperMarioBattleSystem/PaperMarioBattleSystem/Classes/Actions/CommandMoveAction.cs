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
    /// A MoveAction that has an Action Command.
    /// </summary>
    public abstract class CommandMoveAction : MoveAction, IActionCommand
    {
        /// <summary>
        /// The ActionCommand associated with the BattleAction
        /// </summary>
        public ActionCommand actionCommand { get; set; }

        /// <summary>
        /// Tells whether the action command is enabled or not.
        /// Action commands are always disabled for enemies
        /// </summary>
        public bool CommandEnabled => (actionCommand != null && User.EntityType != EntityTypes.Enemy && DisableActionCommand == false);

        /// <summary>
        /// Whether Action Commands are disabled on this action.
        /// This value automatically resets to false after the action is completed.
        /// </summary>
        public bool DisableActionCommand { get; set; }

        /// <summary>
        /// The result of performing the Action Command
        /// </summary>
        protected ActionCommand.CommandResults CommandResult { get; private set; } = ActionCommand.CommandResults.Success;

        protected CommandMoveAction()
        {

        }

        /// <summary>
        /// Starts the Action Command's input.
        /// If the Action Command is not enabled, it will switch to the Failed branch
        /// </summary>
        protected void StartActionCommandInput()
        {
            if (CommandEnabled == true) actionCommand.StartInput();
            else OnCommandFailed();
        }

        protected override void OnStart()
        {
            base.OnStart();
            CommandResult = ActionCommand.CommandResults.Success;
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            DisableActionCommand = false;
            CommandResult = ActionCommand.CommandResults.Success;
        }

        //We have OnCommandSuccess() non-virtual so we can perform general functionality for successfully completing any Action Command
        //Examples include showing the degrees of success ("Nice," "Great," etc.) and increasing the damage dealt by All or Nothing
        public void OnCommandSuccess()
        {
            CommandResult = ActionCommand.CommandResults.Success;

            CommandSuccess();
        }

        //We have OnCommandFailed() non-virtual so we can perform general functionality for failing to complete any Action Command
        //Examples include dealing no damage with the All or Nothing Badge
        public void OnCommandFailed()
        {
            CommandResult = ActionCommand.CommandResults.Failure;

            CommandFailed();
        }

        public abstract void OnCommandResponse(int response);

        protected abstract void CommandSuccess();
        protected abstract void CommandFailed();

        public override void StartInterruption(Elements element)
        {
            if (CommandEnabled == true) actionCommand.EndInput();
            base.StartInterruption(element);
        }

        protected override void PreSequenceUpdate()
        {
            base.PreSequenceUpdate();

            //If the action command is enabled, let it handle the sequence
            if (CommandEnabled == true)
            {
                if (actionCommand.AcceptingInput == true)
                    actionCommand.Update();
            }
        }

        public override void Draw()
        {
            base.Draw();

            if (InSequence == true)
            {
                if (CommandEnabled == true)
                {
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font,
                    $"Command: {Name} performed by {User.Name}",
                    new Vector2(SpriteRenderer.Instance.WindowCenter.X, 50f), Color.Black, 0f, new Vector2(.5f, .5f), 1.1f, .9f, true);

                    actionCommand?.Draw();
                }
            }
        }
    }
}
