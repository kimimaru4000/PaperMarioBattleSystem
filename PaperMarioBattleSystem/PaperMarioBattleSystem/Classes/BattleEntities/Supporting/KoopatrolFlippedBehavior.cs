using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Behavior for handling Koopatrols being flipped.
    /// </summary>
    public class KoopatrolFlippedBehavior : KoopaFlippedBehavior
    {
        private StatusGlobals.PaybackHolder PaybackRemoved;
        private Enumerations.PhysicalAttributes AttributeRemoved;

        public KoopatrolFlippedBehavior(BattleEntity entity, int flippedTurns, Enumerations.DamageEffects flippedOnEffects, int defenseLoss,
            StatusGlobals.PaybackHolder paybackRemoved, Enumerations.PhysicalAttributes attributeRemoved)
            : base(entity, flippedTurns, flippedOnEffects, defenseLoss)
        {
            PaybackRemoved = paybackRemoved;
            AttributeRemoved = attributeRemoved;
        }

        public override void HandleFlipped()
        {
            if (Flipped == false)
            {
                //Remove these when being flipped
                Entity.EntityProperties.RemovePayback(PaybackRemoved);
                Entity.EntityProperties.RemovePhysAttribute(AttributeRemoved);
            }

            base.HandleFlipped();
        }

        public override void UnFlip()
        {
            if (Flipped == true)
            {
                //Add them back when unflipping
                Entity.EntityProperties.AddPayback(PaybackRemoved);
                Entity.EntityProperties.AddPhysAttribute(AttributeRemoved);
            }

            base.UnFlip();
        }

        public override IFlippableBehavior CopyBehavior(BattleEntity entity)
        {
            return new KoopatrolFlippedBehavior(entity, FlippedTurns, FlippedOnEffects, DefenseLoss, PaybackRemoved, AttributeRemoved);
        }
    }
}
