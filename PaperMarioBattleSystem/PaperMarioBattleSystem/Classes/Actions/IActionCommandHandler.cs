using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for BattleActions that handle ActionCommands.
    /// </summary>
    interface IActionCommandHandler
    {
        /// <summary>
        /// What occurs when the action command is successfully performed.
        /// </summary>
        void OnCommandSuccess();

        /// <summary>
        /// What occurs when the action command is failed.
        /// </summary>
        void OnCommandFailed();

        /// <summary>
        /// Handles BattleAction responses sent from an ActionCommand that are not a definite Success or Failure.
        /// Unlike a Success or Failure, the ActionCommand is not required to send this down at all.
        /// <para>For example, the Hammer command sends back the number of lights lit up, and the Hammer action responds
        /// by speeding up Mario's hammer windup animation.</para>
        /// </summary>
        /// <param name="response">An object representing a response from the action command.</param>
        void OnCommandResponse(object response);
    }
}
