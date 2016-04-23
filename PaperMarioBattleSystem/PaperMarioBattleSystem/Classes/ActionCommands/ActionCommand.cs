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
        public bool AcceptingInput { get; protected set; } = false;
        protected BattleAction Action = null;

        protected ActionCommand(BattleAction battleAction)
        {
            Action = battleAction;
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

        protected abstract void OnSuccess();

        protected abstract void OnFailure();

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
