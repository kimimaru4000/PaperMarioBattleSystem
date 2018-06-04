using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Utilities;

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
        private const float MaxRotation = 720f;

        private BattleEntity Entity = null;
        private Animation DeathAnim = null;
        private bool OverrideRevival = false;

        private bool PerformRotation = false;
        private float CurRotation = 0f;
        private float RotateAmount = 15f;

        public DeathBattleEvent(BattleEntity entity, bool overrideRevival, bool doRotation)
        {
            Entity = entity;
            DeathAnim = Entity.AnimManager.GetAnimation(AnimationGlobals.DeathName);
            OverrideRevival = overrideRevival;

            PerformRotation = doRotation;

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
                    Entity.BManager.TurnEnd();
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
                Entity.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Death,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                    new RevivedBattleEvent(1000d, Entity, revivalItem));
            }
            else
            {
                //Handle death
                Entity.BManager.HandleEntityDeath(Entity);
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
                //End after the death animation if we shouldn't perform the rotation
                if (PerformRotation == false)
                {
                    End();
                    return;
                }

                //Perform the rotation
                CurRotation += RotateAmount;
                
                Entity.Rotation = UtilityGlobals.ToRadians(CurRotation);

                //End after rotating a certain amount
                if (CurRotation >= MaxRotation)
                {
                    //Play death sound if it's an enemy
                    if (Entity.EntityType == Enumerations.EntityTypes.Enemy)
                    {
                        SoundManager.Instance.PlaySound(SoundManager.Sound.EnemyDeath);
                    }

                    End();
                }
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
