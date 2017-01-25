using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that plays the character's damaged animation.
    /// This event doesn't occur when the entity is damaged during its own Sequence.
    /// </summary>
    public sealed class DamagedBattleEvent : WaitForAnimBattleEvent
    {
        private BattleEntity Entity = null;

        public DamagedBattleEvent(BattleEntity entity)
        {
            Entity = entity;
            Anim = Entity.GetAnimation(AnimationGlobals.HurtName);
        }

        protected override void OnStart()
        {
            base.OnStart();
            BattleUIManager.Instance.SuppressMenus();

            Anim.Play();
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            BattleUIManager.Instance.UnsuppressMenus();

            Entity.PlayAnimation(Entity.GetIdleAnim());
        }
    }
}
