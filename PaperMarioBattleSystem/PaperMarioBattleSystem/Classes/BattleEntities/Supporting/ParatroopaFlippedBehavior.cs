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
            if (Entity.IsDead == true || damageInfo.Hit == false || Entity.HeightState != Enumerations.HeightStates.Grounded) return;

            //Check if the entity was hit with DamageEffects that flip it
            if (UtilityGlobals.DamageEffectHasFlag(FlippedOnEffects, damageInfo.DamageEffect) == true)
            {
                //Handle flipping the entity
                HandleFlipped();
            }
        }
    }
}
