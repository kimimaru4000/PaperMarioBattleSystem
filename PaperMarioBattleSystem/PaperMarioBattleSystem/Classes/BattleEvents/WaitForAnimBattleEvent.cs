using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent plays an animation for the character and waits for it to finish.
    /// Do not use this for animations that loop infinitely, as it will never end.
    /// </summary>
    public class WaitForAnimBattleEvent : BattleEvent
    {
        protected BattleEntity Entity = null;
        protected IAnimation Anim = null;
        protected bool PlayIdleOnEnd = true;
        private string AnimName = string.Empty;

        public WaitForAnimBattleEvent(BattleEntity entity, string animName, bool playIdleOnEnd)
        {
            Entity = entity;
            AnimName = animName;
            Anim = Entity.AnimManager.GetAnimation<IAnimation>(AnimName);
            PlayIdleOnEnd = playIdleOnEnd;

            IsUnique = true;
        }

        protected override void OnStart()
        {
            base.OnStart();

            Entity.AnimManager.PlayAnimation(AnimName, true);
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            if (PlayIdleOnEnd == true)
            {
                Entity.AnimManager.PlayAnimation(Entity.GetIdleAnim());
            }
        }

        protected override void OnUpdate()
        {
            //If the animation doesn't exist, end immediately
            if (Anim == null)
            {
                End();
                return;
            }

            if (Anim.Finished == true)
            {
                End();
            }
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            WaitForAnimBattleEvent waitForAnimEvent = other as WaitForAnimBattleEvent;

            //Don't compare the animation. In cases where two or more of this event have the same priority and same entity,
            //we don't want a latter animation to override this one
            return (waitForAnimEvent != null && waitForAnimEvent.Entity == Entity);
        }
    }
}
