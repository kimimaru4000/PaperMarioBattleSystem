using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Message Routines for Dialogue Bubbles.
    /// </summary>
    public abstract class MessageRoutine : IUpdateable, ICleanup
    {
        /// <summary>
        /// The Dialogue Bubble the routine is for.
        /// </summary>
        protected DialogueBubble DBubble { get; private set; } = null;

        /// <summary>
        /// Whether the routine is complete or not.
        /// </summary>
        public bool Complete { get; protected set; }

        protected MessageRoutine(DialogueBubble bubble)
        {
            DBubble = bubble;
        }

        public virtual void CleanUp()
        {

        }

        /// <summary>
        /// What happens when the Message Routine starts.
        /// </summary>
        public abstract void OnStart();

        /// <summary>
        /// What happens when the Message Routine ends.
        /// </summary>
        public abstract void OnEnd();

        /// <summary>
        /// Updates the routine.
        /// </summary>
        public abstract void Update();
    }
}
