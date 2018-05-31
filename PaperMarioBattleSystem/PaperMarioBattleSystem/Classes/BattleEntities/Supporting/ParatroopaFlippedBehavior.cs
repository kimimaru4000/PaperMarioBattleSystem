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
    /// Behavior for flipping Paratroopas.
    /// They can only be flipped after landing.
    /// </summary>
    public class ParatroopaFlippedBehavior : KoopaFlippedBehavior
    {
        private int NumTimesHit = 0;

        public ParatroopaFlippedBehavior(BattleEntity entity, int flippedTurns, Enumerations.DamageEffects flippedOnEffects, int defenseLoss)
            : base(entity, flippedTurns, flippedOnEffects, defenseLoss)
        {

        }

        protected override void OnDamageTaken(in InteractionHolder damageInfo)
        {
            //If it's on the ground, act like a normal Koopa
            if (Entity.HeightState == Enumerations.HeightStates.Grounded)
            {
                base.OnDamageTaken(damageInfo);
                return;
            }

            //If the Paratroopa is in the air, it can't be flipped
            //Track how many times it was hit: if hit at least 2 times, queue a Battle Event to flip it after it lands
            if (NumTimesHit < 2 && UtilityGlobals.DamageEffectHasFlag(FlippedOnEffects, damageInfo.DamageEffect) == true)
            {
                NumTimesHit++;

                if (NumTimesHit >= 2)
                {
                    //Queue the event
                    Entity.BManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Damage - 1,
                        new BattleManager.BattleState[] { BattleManager.BattleState.Turn, BattleManager.BattleState.TurnEnd },
                        new FlippedBattleEvent(Entity as IFlippableEntity));
                }
            }
        }

        public override IFlippableBehavior CopyBehavior(BattleEntity entity)
        {
            return new ParatroopaFlippedBehavior(entity, FlippedTurns, FlippedOnEffects, DefenseLoss);
        }
    }
}
