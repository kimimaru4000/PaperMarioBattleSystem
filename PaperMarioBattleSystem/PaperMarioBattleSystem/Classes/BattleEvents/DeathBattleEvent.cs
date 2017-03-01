using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that plays the entity's death animation.
    /// After the animation, if the entity has a Life Shroom it will then queue a revive event.
    /// <para>If it's an Enemy, it'll give Star Points and be removed from battle.
    /// If it's Mario, the battle ends.</para>
    /// </summary>
    public sealed class DeathBattleEvent : BattleEvent
    {
        private BattleEntity Entity = null;
        private Animation DeathAnim = null;

        public DeathBattleEvent(BattleEntity entity)
        {
            Entity = entity;
            DeathAnim = Entity.AnimManager.GetAnimation(AnimationGlobals.DeathName);

            IsUnique = true;
        }

        protected override void OnStart()
        {
            base.OnStart();
            BattleUIManager.Instance.SuppressMenus();

            Entity.AnimManager.PlayAnimation(AnimationGlobals.DeathName, true);
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            BattleUIManager.Instance.UnsuppressMenus();

            BattleManager.Instance.HandleEntityDeaths();
        }

        protected override void OnUpdate()
        {
            if (DeathAnim == null)
            {
                End();
                return;
            }

            if (DeathAnim.Finished == true)
            {
                End();
            }
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            DeathBattleEvent deathEvent = other as DeathBattleEvent;

            return (deathEvent != null && deathEvent.Entity == Entity);
        }
    }
}
