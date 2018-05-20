using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for UI involving Action Commands.
    /// </summary>
    /// <typeparam name="T">A type derived from ActionCommand.</typeparam>
    public abstract class ActionCommandUI<T> : UIElement where T: ActionCommand
    {
        protected T ActionCmd = null;

        protected ActionCommandUI()
        {

        }

        protected ActionCommandUI(T actionCommand) : base()
        {
            ActionCmd = actionCommand;
        }

        public override void Update()
        {
            
        }
    }
}
