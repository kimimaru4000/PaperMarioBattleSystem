using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event that makes a BattleEntity play an animation.
    /// </summary>
    public class PlayAnimBattleEvent : BattleEvent
    {
        protected BattleEntity Entity = null;
        protected string AnimName = string.Empty;

        /// <summary>
        /// Whether to wait for the BattleEntity to not be targeted to play the animation or not.
        /// </summary>
        protected bool WaitNotTargeted = false;

        public PlayAnimBattleEvent(BattleEntity entity, string animName, bool waitNotTargeted)
        {
            Entity = entity;
            AnimName = animName;
            WaitNotTargeted = waitNotTargeted;
        }

        protected override void OnUpdate()
        {
            if (WaitNotTargeted == true && Entity.IsTargeted == true)
                return;

            Entity.AnimManager.PlayAnimation(AnimName);

            End();
        }
    }
}
