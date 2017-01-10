using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The POWDown Status Effect.
    /// The entity's Attack is reduced by a certain value until it ends.
    /// </summary>
    public class POWDownStatus : StatusEffect
    {
        /// <summary>
        /// The amount to reduce the entity's Attack by.
        /// </summary>
        protected int AttackValue = 0;

        public POWDownStatus(int attackValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.POWDown;
            Alignment = StatusAlignments.Negative;

            AttackValue = attackValue;
            Duration = duration;

            AfflictedMessage = "Attack has dropped!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.LowerAttack(AttackValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RaiseAttack(AttackValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.RaiseAttack(AttackValue);
        }

        protected override void OnResume()
        {
            EntityAfflicted.LowerAttack(AttackValue);
        }

        public override StatusEffect Copy()
        {
            return new POWDownStatus(AttackValue, Duration);
        }
    }
}
