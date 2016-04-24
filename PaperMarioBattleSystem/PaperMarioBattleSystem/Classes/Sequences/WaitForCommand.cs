using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Causes the entity to wait for the action command's result before proceeding in the action sequence.
    /// A fallback time is required if commands aren't enabled, in which this sequence will end after this time
    /// </summary>
    public class WaitForCommand : Wait
    {
        protected ActionCommand Command = null;
        protected bool CommandEnabled = true;

        public WaitForCommand(double fallbackTime, ActionCommand command, bool commandEnabled) : base(fallbackTime)
        {
            Command = command;
            CommandEnabled = commandEnabled;
        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void OnUpdate()
        {
            //If the command isn't enabled, wait on the fallback time
            if (CommandEnabled == false)
            {
                base.OnUpdate();
            }
            //Check if the ActionCommand is done
            else if (Command.AcceptingInput == false)
            {
                End();
            }
        }
    }
}
