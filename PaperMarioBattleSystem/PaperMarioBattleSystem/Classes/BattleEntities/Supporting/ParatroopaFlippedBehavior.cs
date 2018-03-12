using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Behavior for flipping Paratroopas.
    /// They can only be flipped after landing.
    /// </summary>
    public class ParatroopaFlippedBehavior : KoopaFlippedBehavior
    {
        public ParatroopaFlippedBehavior(BattleEntity entity, int flippedTurns, Enumerations.DamageEffects flippedOnEffects, int defenseLoss)
            : base(entity, flippedTurns, flippedOnEffects, defenseLoss)
        {

        }

        protected override void OnDamageTaken(InteractionHolder damageInfo)
        {
            //If the Paratroopa is still in the air, it can't be flipped
            if (Entity.HeightState != Enumerations.HeightStates.Grounded) return;

            base.OnDamageTaken(damageInfo);
        }

        public override IFlippableBehavior CopyBehavior(BattleEntity entity)
        {
            return new ParatroopaFlippedBehavior(entity, FlippedTurns, FlippedOnEffects, DefenseLoss);
        }
    }
}
