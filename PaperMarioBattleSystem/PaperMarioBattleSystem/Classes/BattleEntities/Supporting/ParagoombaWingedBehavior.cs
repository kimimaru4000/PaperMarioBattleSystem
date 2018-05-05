using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Behavior for handling Paragoombas losing their wings.
    /// </summary>
    public class ParagoombaWingedBehavior : IWingedBehavior
    {
        public BattleEntity Entity = null;

        public bool Grounded { get; set; } = false;

        public int GroundedTurns { get; set; } = 2;

        public int ElapsedGroundedTurns { get; set; } = 0;

        public BattleEntity GroundedEntity { get; set; } = null;

        public Enumerations.DamageEffects GroundedOnEffects { get; set; } = Enumerations.DamageEffects.RemovesWings;

        protected Rectangle WingRectRegion = Rectangle.Empty;

        public ParagoombaWingedBehavior(BattleEntity entity, int groundedTurns, Enumerations.DamageEffects groundedOnEffects, BattleEntity groundedEntity)
        {
            Entity = entity;
            GroundedTurns = groundedTurns;
            GroundedOnEffects = groundedOnEffects;
            GroundedEntity = groundedEntity;

            WingRectRegion = new Rectangle(3, 166, 41, 18);

            Entity.DamageTakenEvent -= OnDamageTaken;
            Entity.DamageTakenEvent += OnDamageTaken;
        }

        public virtual void CleanUp()
        {
            if (Entity != null)
            {
                Entity.DamageTakenEvent -= OnDamageTaken;
            }

            GroundedEntity = null;
        }

        public virtual void HandleGrounded()
        {
            //Return if already grounded
            if (Grounded == true) return;

            Grounded = true;

            //Check if the winged entity and its grounded version have entries in the Tattle database
            bool wingedInTattle = TattleDatabase.HasTattleDescription(Entity.Name);
            bool groundedInTattle = TattleDatabase.HasTattleDescription(GroundedEntity?.Name);

            //If the winged entity has an entry and the grounded version doesn't, remove its ShowHP property
            if (wingedInTattle == true && groundedInTattle == false)
            {
                Entity.SubtractShowHPProperty();
            }
            //If the winged entity doesn't have an entry and the grounded version does, add its ShowHP property
            else if (wingedInTattle == false && groundedInTattle == true)
            {
                Entity.AddShowHPProperty();
            }

            if (GroundedEntity != null)
            {
                Entity.Name = GroundedEntity.Name;

                //Set the vulnerability to the same as the grounded entity. The grounded entity shouldn't have a winged vulnerabilty
                Entity.EntityProperties.SetVulnerableDamageEffects(GroundedEntity.EntityProperties.GetVulnerableDamageEffects());
            }

            //Queue the BattleEvent to remove the wings
            BattleManager.Instance.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Damage - 1,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new RemoveWingsBattleEvent(this, Entity));

            //Queue the BattleEvent to move the entity down
            BattleManager.Instance.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Damage - 1,
                new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                new GroundedBattleEvent(Entity, new Vector2(Entity.BattlePosition.X, BattleManager.Instance.EnemyStartPos.Y)));

            //Remove the damage event, since we don't need it anymore
            Entity.DamageTakenEvent -= OnDamageTaken;
        }

        public virtual void RemoveWings()
        {
            Vector2 wingPos = Entity.Position;
            Animation hurtAnim = Entity.AnimManager.GetAnimation(AnimationGlobals.HurtName);
            if (hurtAnim != null)
            {
                wingPos = hurtAnim.CurChildFrame.GetDrawnPosition(Entity.Position, Entity.SpriteFlip);
            }

            //Remove the wings from the hurt and death animations
            Animation[] animations = Entity.AnimManager.GetAnimations(AnimationGlobals.HurtName, AnimationGlobals.DeathName);

            //Clear all child frames with wings
            for (int i = 0; i < animations.Length; i++)
            {
                animations[i].SetChildFrames(null);
            }

            //Add VFX for the wings disappearing
            Texture2D spriteSheet = Entity.AnimManager.SpriteSheet;
            CroppedTexture2D wingSprite = new CroppedTexture2D(spriteSheet, WingRectRegion);

            //Put the wings in the same spot as they were in the Paragoomba's last animation
            WingsDisappearVFX wingsDisappear = new WingsDisappearVFX(wingSprite, wingPos,
                Entity.EntityType != Enumerations.EntityTypes.Enemy, .1f - .01f, 500d, 500d, (1d / 30d) * Time.MsPerS);

            BattleObjManager.Instance.AddBattleObject(wingsDisappear);

            //Copy the StatusProperties from the grounded entity
            //This happens here, as winged entities use their own status tables until they've been grounded
            if (GroundedEntity != null)
            {
                Entity.EntityProperties.CopyStatusProperties(GroundedEntity.EntityProperties);
            }

            //Don't modify Tattle information
            //The entity knows what its grounded version is
            //However, one thing we can do here is disable tattling if the grounded version doesn't support it
            ITattleableEntity gTattleable = GroundedEntity as ITattleableEntity;
            if (gTattleable == null)
            {
                //Disable tattling if the grounded version can't be Tattled
                ITattleableEntity entity = Entity as ITattleableEntity;
                if (entity != null)
                {
                    entity.CanBeTattled = false;
                }
            }

            //After all this set the GroundedEntity to null, as we don't need its information anymore
            GroundedEntity = null;
        }

        private void OnDamageTaken(InteractionHolder damageInfo)
        {
            if (Entity.IsDead == true || damageInfo.Hit == false) return;

            //Check if the entity was hit with DamageEffects that remove its wings
            if (UtilityGlobals.DamageEffectHasFlag(GroundedOnEffects, damageInfo.DamageEffect) == true)
            {
                //Handle grounding the entity
                HandleGrounded();
            }
        }
    }
}
