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
        public bool AcceptingInput { get; protected set; } = true;
        protected BattleAction Action = null;

        protected ActionCommand(BattleAction battleAction)
        {
            Action = battleAction;
        }

        /// <summary>
        /// Performs any initialization to start reading input for the action command
        /// </summary>
        public abstract void StartInput();

        protected abstract void OnSuccess(int successRate);

        protected abstract void OnFailure();

        /// <summary>
        /// Reads input for the action command
        /// </summary>
        protected abstract void ReadInput();

        public void Update()
        {
            ReadInput();
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
