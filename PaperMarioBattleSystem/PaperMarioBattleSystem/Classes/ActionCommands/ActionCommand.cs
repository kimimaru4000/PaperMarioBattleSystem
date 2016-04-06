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
    /// ActionCommands can be attached to any BattleAction
    /// </summary>
    public abstract class ActionCommand
    {
        protected BattleAction Action = null;

        protected ActionCommand(BattleAction battleAction)
        {
            Action = battleAction;
        }

        /// <summary>
        /// Reads input for the action command
        /// </summary>
        protected abstract void ReadInput();

        public void Update()
        {
            ReadInput();
        }

        /// <summary>
        /// What occurs when the player succeeds in performing the ActionCommand
        /// </summary>
        //public abstract void OnSuccess();

        /// <summary>
        /// What occurs when the player fails to perform the ActionCommand
        /// </summary>
        //public abstract void OnFailure();
    }
}
