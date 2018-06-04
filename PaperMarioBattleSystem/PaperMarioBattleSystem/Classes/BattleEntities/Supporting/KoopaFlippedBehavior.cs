using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Behavior for handling Koopa Troopas being flipped.
    /// </summary>
    public class KoopaFlippedBehavior : IFlippableBehavior
    {
        public BattleEntity Entity = null;

        public bool Flipped { get; set; } = false;

        public int FlippedTurns { get; set; } = 2;

        public int ElapsedFlippedTurns { get; set; } = 0;

        public Enumerations.DamageEffects FlippedOnEffects { get; set; } =
            (Enumerations.DamageEffects.FlipsShelled | Enumerations.DamageEffects.FlipsClefts);

        public int DefenseLoss { get; set; } = 0;

        public KoopaFlippedBehavior(BattleEntity entity, int flippedTurns, Enumerations.DamageEffects flippedOnEffects, int defenseLoss)
        {
            Entity = entity;
            FlippedTurns = flippedTurns;

            FlippedOnEffects = flippedOnEffects;
            DefenseLoss = defenseLoss;

            //Subscribe to taking damage so it can check the damage effects
            Entity.DamageTakenEvent -= OnDamageTaken;
            Entity.DamageTakenEvent += OnDamageTaken;
        }

        public virtual void CleanUp()
        {
            if (Entity != null)
            {
                Entity.DamageTakenEvent -= OnDamageTaken;
                Entity.TurnStartEvent -= OnFlippedTurnStart;
            }
        }

        public virtual void HandleFlipped()
        {
            if (Flipped == false)
            {
                Entity.AddIntAdditionalProperty(Enumerations.AdditionalProperty.Immobile, 1);

                //Flipped Koopas are immune to Fright
                Entity.AddRemoveStatusImmunity(Enumerations.StatusTypes.Fright, true);

                //Lower defense by an amount when flipped
                Entity.LowerDefense(DefenseLoss);
            }

            Flipped = true;

            //Don't play this animation if dead
            if (Entity.IsDead == false)
            {
                Entity.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Damage - 2,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn, BattleGlobals.BattleState.TurnEnd },
                    new PlayAnimBattleEvent(Entity, Entity.GetIdleAnim(), false));
            }

            //Getting hit again while flipped refreshes the flip timer
            ElapsedFlippedTurns = 0;

            //Handle being flipped
            Entity.TurnStartEvent -= OnFlippedTurnStart;
            Entity.TurnStartEvent += OnFlippedTurnStart;
        }

        public virtual void UnFlip()
        {
            if (Flipped == true)
            {
                Entity.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.Immobile, 1);

                //Remove the immunity to Fright after they get up
                Entity.AddRemoveStatusImmunity(Enumerations.StatusTypes.Fright, false);

                //Raise defense again after unflipping
                Entity.RaiseDefense(DefenseLoss);
            }

            Flipped = false;
            Entity.AnimManager.PlayAnimation(Entity.GetIdleAnim(), true);

            ElapsedFlippedTurns = 0;

            Entity.TurnStartEvent -= OnFlippedTurnStart;
        }

        private void OnFlippedTurnStart()
        {
            ElapsedFlippedTurns++;

            if (ElapsedFlippedTurns >= FlippedTurns)
            {
                //Get up; this should still use up a turn
                UnFlip();
            }

            //We don't make the entity actually do anything here
            //It should just use this data to decide how it wants to act
            //For example, Shady Koopas can attack when flipped, so they can choose to do so at the start of their turns
        }

        protected virtual void OnDamageTaken(in InteractionHolder damageInfo)
        {
            if (Entity.IsDead == true || damageInfo.Hit == false) return;

            //Check if the entity was hit with DamageEffects that flip it
            if (UtilityGlobals.DamageEffectHasFlag(FlippedOnEffects, damageInfo.DamageEffect) == true)
            {
                //Handle flipping the entity
                HandleFlipped();
            }
        }

        public virtual IFlippableBehavior CopyBehavior(BattleEntity entity)
        {
            return new KoopaFlippedBehavior(entity, FlippedTurns, FlippedOnEffects, DefenseLoss);
        }
    }
}
