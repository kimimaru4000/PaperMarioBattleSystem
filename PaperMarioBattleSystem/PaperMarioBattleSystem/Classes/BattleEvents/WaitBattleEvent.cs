using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event that simply waits for a specific amount of time.
    /// </summary>
    public class WaitBattleEvent : BattleEvent
    {
        /// <summary>
        /// The amount to wait in milliseconds.
        /// </summary>
        protected double WaitDuration = 0d;

        /// <summary>
        /// When the Battle Event started in milliseconds.
        /// </summary>
        protected double StartTime = 0d;

        /// <summary>
        /// When the Battle Event should end in milliseconds.
        /// </summary>
        protected double EndTime = 0d;

        public WaitBattleEvent(double waitDuration) : base(0)
        {
            WaitDuration = waitDuration;
        }

        protected override void OnStart()
        {
            base.OnStart();

            StartTime = Time.ActiveMilliseconds;
            EndTime = StartTime + WaitDuration;
        }

        protected override void OnUpdate()
        {
            if (Time.ActiveMilliseconds >= EndTime)
            {
                End();
            }
        }
    }
}
