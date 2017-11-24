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

            //Handle the entity dying on its turn
            if (Entity.IsTurn == true)
            {
                //If in the middle of a sequence, end it
                if (Entity.PreviousAction != null && Entity.PreviousAction.MoveSequence.InSequence == true)
                {
                    Entity.PreviousAction.MoveSequence.EndSequence();
                }
                //Otherwise end its turn
                else
                {
                    BattleManager.Instance.TurnEnd();
                }
            }

            BattleManager.Instance.HandleEntityDeaths();
        }

        protected override void OnUpdate()
        {
            //This can lead to a softlock if the event is added after the entity is removed from battle
            //The event system needs to be less error-prone
            //Add the in-battle check since the entity won't update its death animation if it's not in battle
            if (DeathAnim == null || Entity.IsInBattle == false)
            {
                End();
                return;
            }

            if (DeathAnim.Finished == true)
            {
                //Play death sound if it's an enemy
                if (Entity.EntityType == Enumerations.EntityTypes.Enemy)
                {
                    SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDeath);
                }

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
