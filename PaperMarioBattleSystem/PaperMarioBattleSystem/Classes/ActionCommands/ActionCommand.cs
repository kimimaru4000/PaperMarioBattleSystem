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
    /// Inputs the player must register at specific times during a BattleAction in battle
    /// </summary>
    public abstract class ActionCommand
    {
        /// <summary>
        /// What occurs when the player succeeds in performing the ActionCommand
        /// </summary>
        public abstract void OnSuccess();

        /// <summary>
        /// What occurs when the player fails to perform the ActionCommand
        /// </summary>
        public abstract void OnFailure();
    }
}
