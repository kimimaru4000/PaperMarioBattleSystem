using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that plays the character's damaged animation.
    /// <para>This event doesn't occur when the entity is damaged during its own Sequence, as Sequence interruptions handle that.</para>
    /// </summary>
    public sealed class DamagedBattleEvent : BattleEvent
    {
        private BattleEntity Entity = null;
        private Animation HurtAnim = null;

        public DamagedBattleEvent(BattleEntity entity)
        {
            Entity = entity;
            HurtAnim = Entity.AnimManager.GetAnimation(AnimationGlobals.HurtName);

            IsUnique = true;
        }

        protected override void OnStart()
        {
            base.OnStart();
            BattleUIManager.Instance.SuppressMenus();

            Entity.AnimManager.PlayAnimation(AnimationGlobals.HurtName, true);
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            BattleUIManager.Instance.UnsuppressMenus();

            Entity.AnimManager.PlayAnimation(Entity.GetIdleAnim());
        }

        protected override void OnUpdate()
        {
            //If the hurt animation doesn't exist, end immediately
            if (HurtAnim == null)
            {
                End();
                return;
            }

            if (HurtAnim.Finished == true)
            {
                //NOTE: Check if the entity is being targeted here and replay the animation if so.
                //This prevents the death event from occurring until the entity is completely done being attacked
                //if (Entity.IsTargeted == true)
                    //HurtAnim.Play();
                //else
                End();
            }
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            DamagedBattleEvent damagedEvent = other as DamagedBattleEvent;

            return (damagedEvent != null && damagedEvent.Entity == Entity);
        }
    }
}
