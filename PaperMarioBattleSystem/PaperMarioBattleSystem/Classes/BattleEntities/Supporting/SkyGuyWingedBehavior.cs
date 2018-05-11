﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class SkyGuyWingedBehavior : IWingedBehavior
    {
        public BattleEntity Entity = null;

        public bool Grounded { get; set; } = false;

        public int GroundedTurns { get; set; } = 2;

        public int ElapsedGroundedTurns { get; set; } = 0;

        public BattleEntity GroundedEntity { get; set; } = null;

        public Enumerations.DamageEffects GroundedOnEffects { get; set; } = Enumerations.DamageEffects.None;

        public SkyGuyWingedBehavior(BattleEntity skyGuy)
        {
            Entity = skyGuy;

            GroundedEntity = new ShyGuy();
        }

        public void CleanUp()
        {
            GroundedEntity = null;
        }

        public void HandleGrounded()
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
                new GroundedBattleEvent(Entity, new Vector2(Entity.BattlePosition.X, BattleGlobals.EnemyStartPos.Y)));
        }

        public void RemoveWings()
        {
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

            //Copy all of the grounded entity's animations, as Sky Guys have completely unique animations
            Entity.AnimManager.ClearAllAnimations();
            Animation[] allAnims = GroundedEntity.AnimManager.GetAllAnimations();
            for (int i = 0; i < allAnims.Length; i++)
            {
                Animation anim = allAnims[i];
                Entity.AnimManager.AddAnimation(anim.Key, anim);
            }

            //After all this, set the GroundedEntity to null, as we don't need its information anymore
            GroundedEntity = null;
        }
    }
}
