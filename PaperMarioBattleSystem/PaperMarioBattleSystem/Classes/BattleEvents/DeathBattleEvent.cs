using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that plays the entity's death animation.
    /// After the animation, if the entity has a Life Shroom, it will then queue a revive event.
    /// <para>If it's an Enemy, it'll give Star Points and be removed from battle.
    /// If it's Mario, the battle ends.</para>
    /// </summary>
    public sealed class DeathBattleEvent : BattleEvent
    {
        private BattleEntity Entity = null;
        private Animation DeathAnim = null;
        private bool OverrideRevival = false;
        private float CurRotation = 0f;
        private float RotateAmount = 10f;

        public DeathBattleEvent(BattleEntity entity, bool overrideRevival)
        {
            Entity = entity;
            DeathAnim = Entity.AnimManager.GetAnimation(AnimationGlobals.DeathName);
            OverrideRevival = overrideRevival;

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

            Entity.Rotation = 0f;

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

            //If this BattleEntity is being revived, add the revival event and don't check for deaths
            Item revivalItem = null;

            //If we should override revival, don't check for a revival item
            if (OverrideRevival == false)
            {
                revivalItem = Entity.GetItemOfType(Item.ItemTypes.Revival);
            }

            if (revivalItem != null)
            {
                //Queue the revival event with the same priority as death so it occurs immediately
                BattleManager.Instance.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Death,
                    new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                    new RevivedBattleEvent(1000d, Entity, revivalItem));
            }
            else
            {
                //Handle death
                BattleManager.Instance.HandleEntityDeath(Entity);
            }
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

            //Play the death animation if it isn't being played
            if (Entity.AnimManager.CurrentAnim != DeathAnim)
            {
                DeathAnim.Play();
            }

            if (DeathAnim.Finished == true)
            {
                //NOTE: Implement the rotation after setting center origins so it looks better
                //CurRotation += RotateAmount;
                //
                //Entity.Rotation = UtilityGlobals.ToRadians(CurRotation);

                //if (CurRotation >= 360f)
                //{
                    //Play death sound if it's an enemy
                    if (Entity.EntityType == Enumerations.EntityTypes.Enemy)
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDeath);
                    }

                    End();
                //}
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
