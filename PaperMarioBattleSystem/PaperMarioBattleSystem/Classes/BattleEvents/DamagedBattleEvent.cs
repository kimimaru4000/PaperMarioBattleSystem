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
    public sealed class DamagedBattleEvent : WaitForAnimBattleEvent
    {
        public DamagedBattleEvent(BattleEntity entity) : base(entity, AnimationGlobals.HurtName, true)
        {
            
        }

        protected override void OnStart()
        {
            base.OnStart();
            BattleUIManager.Instance.SuppressMenus();
        }

        protected override void OnEnd()
        {
            base.OnEnd();
            BattleUIManager.Instance.UnsuppressMenus();
        }

        protected override void OnUpdate()
        {
            //Even if the animation is null, still wait
            bool animFinished = (Anim == null || Anim.Finished == true);

            if (animFinished)
            {
                //Check if the entity is being targeted here and replay the animation if so.
                //This prevents the death event from occurring until the entity is completely done being attacked
                if (Entity.IsTargeted == true)
                    Anim?.Play();
                else
                    End();
            }
        }
    }
}
