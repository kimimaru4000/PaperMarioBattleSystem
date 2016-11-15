using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for BattleActions that utilize ActionCommands
    /// </summary>
    public interface IActionCommand
    {
        /// <summary>
        /// The name of the BattleAction
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The Action Command associated with the BattleAction
        /// </summary>
        ActionCommand actionCommand { get; }

        /// <summary>
        /// Whether the Action Command is enabled or not
        /// </summary>
        bool CommandEnabled { get; }

        /// <summary>
        /// A value denoting to disable the Action Command
        /// </summary>
        bool DisableActionCommand { get; set; }

        /// <summary>
        /// What occurs when the action command is successfully performed
        /// </summary>
        void OnCommandSuccess();

        /// <summary>
        /// What occurs when the action command is failed
        /// </summary>
        void OnCommandFailed();

        /// <summary>
        /// Handles BattleAction responses sent from an ActionCommand that are not a definite Success or Failure.
        /// Unlike a Success or Failure, the ActionCommand is not required to send this down at all
        /// <para>For example, the Hammer command sends back the number of lights lit up, and the Hammer action responds
        /// by speeding up Mario's hammer windup animation</para>
        /// </summary>
        /// <param name="response">A number representing a response from the action command</param>
        void OnCommandResponse(int response);
    }
}
