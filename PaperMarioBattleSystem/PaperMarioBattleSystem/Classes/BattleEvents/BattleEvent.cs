using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Battle Events.
    /// Battle Events take over entity turns and must finish before the next entity can go.
    /// They have a priority value, with higher priorities taking precedence over lower priority events.
    /// <para>Examples of Battle Events include Battle Messages, entities dying or taking damage, dialogue, and Status Effect animations.</para>
    /// </summary>
    public abstract class BattleEvent
    {
        /// <summary>
        /// The Priority of the Battle Event.
        /// </summary>
        //public int Priority { get; protected set; } = 0;

        /// <summary>
        /// Whether the Battle Event started or not.
        /// </summary>
        protected bool Started { get; private set; } = false;

        /// <summary>
        /// Whether the Battle Event is done or not.
        /// </summary>
        protected bool Done { get; private set; } = false;

        /// <summary>
        /// Tells if the Battle Event has started.
        /// </summary>
        public bool HasStarted => Started;

        /// <summary>
        /// Tells if the Battle Event is finished.
        /// </summary>
        public bool IsDone => Done;

        /// <summary>
        /// The BattleEntities involved in the Battle Event. Not always applicable, as some events don't require BattleEntities.
        /// </summary>
        public BattleEntity[] Entities { get; protected set; } = null;

        protected BattleEvent()//int priority)
        {
            //Priority = priority;
        }

        /// <summary>
        /// Starts the Battle Event.
        /// </summary>
        public void Start()
        {
            Started = true;

            OnStart();
        }

        /// <summary>
        /// What happens when the Battle Event starts.
        /// </summary>
        protected virtual void OnStart()
        {

        }

        /// <summary>
        /// Ends the Battle Event and marks it as done.
        /// </summary>
        public void End()
        {
            Done = true;

            OnEnd();
        }

        /// <summary>
        /// What happens when the Battle Event finishes.
        /// </summary>
        protected virtual void OnEnd()
        {

        }

        /// <summary>
        /// Updates the Battle Event.
        /// </summary>
        public void Update()
        {
            if (IsDone == false)
            {
                OnUpdate();
            }
        }

        /// <summary>
        /// What the Battle Event does.
        /// </summary>
        protected abstract void OnUpdate();
    }
}
