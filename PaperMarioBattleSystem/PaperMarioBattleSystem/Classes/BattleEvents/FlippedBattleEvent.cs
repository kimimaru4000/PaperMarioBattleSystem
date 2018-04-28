using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles flipping a flippable BattleEntity.
    /// This is used in cases where flipping immediately produces undesirable behavior.
    /// </summary>
    public sealed class FlippedBattleEvent : BattleEvent
    {
        private IFlippableEntity FlippableEntity = null;

        public FlippedBattleEvent(IFlippableEntity flippableEntity)
        {
            FlippableEntity = flippableEntity;
        }

        protected override void OnUpdate()
        {
            //Flip and end
            FlippableEntity.FlippedBehavior?.HandleFlipped();

            End();
        }
    }
}
