using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An interface for BattleActions that utilize ActionCommands.
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
    }
}
