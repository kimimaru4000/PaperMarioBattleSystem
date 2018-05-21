using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Inputs the player must register at specific times during a BattleAction in battle.
    /// ActionCommands can be attached to any BattleAction.
    /// </summary>
    public abstract class ActionCommand
    {
        /// <summary>
        /// Indicates how well the player performed the Action Command.
        /// </summary>
        public enum CommandRank
        {
            None = 0, NiceM2, NiceM1, Nice, Good, Great, Wonderful, Excellent
        }

        public enum CommandResults
        {
            Success, Failure
        }

        public bool AcceptingInput { get; private set; } = false;
        public IActionCommandHandler Handler { get; private set; } = null;

        /// <summary>
        /// Whether the Action Command should automatically be successfully performed or not.
        /// <para>Since each action command is different, this will need to be manually implemented in each one.
        /// This may not always work, as the absolute success of some commands may be arbitrary (Ex. Art Attack, Bomb Squad).</para>
        /// </summary>
        public bool AutoComplete = false;

        protected ActionCommand(IActionCommandHandler commandHandler)
        {
            SetHandler(commandHandler);
        }

        public void SetHandler(IActionCommandHandler commandHandler)
        {
            Handler = commandHandler;
        }

        /// <summary>
        /// Performs any initialization to start reading input for the action command
        /// </summary>
        /// <param name="values">Any values passed to the ActionCommand just as it starts.
        /// An example includes the BattleEntity's position for the Tattle cursor.</param>
        public virtual void StartInput(params object[] values)
        {
            AcceptingInput = true;
        }

        /// <summary>
        /// Performs anything required when ending input for the action command
        /// </summary>
        public virtual void EndInput()
        {
            AcceptingInput = false;
        }

        /// <summary>
        /// Called by derived classes to send a completion response to the handler associated with the ActionCommand.
        /// This also ends input for the ActionCommand.
        /// </summary>
        /// <param name="result">The final result of the ActionCommand</param>
        protected void OnComplete(CommandResults result)
        {
            Debug.Log($"Command for {Handler.Name} has completed with a {result} result!");

            if (result == CommandResults.Success)
            {
                Handler.OnCommandSuccess();
            }
            else if (result == CommandResults.Failure)
            {
                Handler.OnCommandFailed();
            }

            //The Command is finished, so end input
            EndInput();
        }

        /// <summary>
        /// Sends a CommandRank to the Handler, which handles the value given.
        /// </summary>
        /// <param name="commandRank">The CommandRank indicating how well the Player performed the action command.</param>
        protected void SendCommandRank(CommandRank commandRank)
        {
            Debug.Log($"Command for {Handler.Name} has sent a {nameof(CommandRank)} of {commandRank}");

            Handler.OnCommandRankResult(commandRank);
        }

        /// <summary>
        /// Sends a response to the Handler, which handles the value given
        /// </summary>
        /// <param name="response">An object representing a response</param>
        protected void SendResponse(object response)
        {
            Debug.Log($"Command for {Handler.Name} has sent a response of {response}!");

            Handler.OnCommandResponse(response);
        }

        /// <summary>
        /// Reads input for the action command
        /// </summary>
        protected abstract void ReadInput();

        public void Update()
        {
            if (AcceptingInput == true)
            {
                ReadInput();
            }
        }
    }
}
