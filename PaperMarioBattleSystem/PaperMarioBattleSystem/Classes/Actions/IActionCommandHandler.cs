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
    public interface IActionCommandHandler : INameable
    {
        /// <summary>
        /// The result of performing the Action Command.
        /// </summary>
        ActionCommand.CommandResults CommandResult { get; }

        /// <summary>
        /// What occurs when the action command is successfully performed.
        /// </summary>
        void OnCommandSuccess();

        /// <summary>
        /// What occurs when the action command is failed.
        /// </summary>
        void OnCommandFailed();

        /// <summary>
        /// What occurs when a CommandRank is sent. This is an indicator of how well the Action Command was performed.
        /// </summary>
        /// <param name="actionResult">The result of the Action Command.</param>
        void OnCommandRankResult(ActionCommand.CommandRank commandRank);

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
