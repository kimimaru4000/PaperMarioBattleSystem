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
    /// ActionCommands can be attached to any BattleAction, but enemies cannot use them
    /// </summary>
    public abstract class ActionCommand
    {
        public enum CommandResults
        {
            Success, Failure
        }

        public bool AcceptingInput { get; protected set; } = false;
        protected ICommandAction Action = null;

        protected ActionCommand()
        {
            
        }

        protected ActionCommand(ICommandAction commandAction)
        {
            Action = commandAction;
        }

        /// <summary>
        /// Performs any initialization to start reading input for the action command
        /// </summary>
        public virtual void StartInput()
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
        /// Called by derived classes to send a completion response to the Action associated with the Command.
        /// This also ends input for the Command
        /// </summary>
        /// <param name="result">The final result of the ActionCommand</param>
        protected void OnComplete(CommandResults result)
        {
            Debug.Log($"Command for {Action.Name} has completed with a {result} result!");

            if (result == CommandResults.Success)
            {
                Action.OnCommandSuccess();
            }
            else if (result == CommandResults.Failure)
            {
                Action.OnCommandFailed();
            }

            //The Command is finished, so end input
            EndInput();
        }

        /// <summary>
        /// Sends a response to the Action, which handles the value given
        /// </summary>
        /// <param name="response">An int value representing a response</param>
        protected void SendResponse(int response)
        {
            Debug.Log($"Command for {Action.Name} has sent a response of {response}!");

            Action.OnCommandResponse(response);
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

        public void Draw()
        {
            if (AcceptingInput == true)
            {
                OnDraw();   
            }
        }

        protected virtual void OnDraw()
        {

        }
    }
}
