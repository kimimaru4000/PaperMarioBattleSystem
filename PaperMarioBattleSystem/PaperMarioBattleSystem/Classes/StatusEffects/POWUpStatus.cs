using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The POWUp Status Effect.
    /// The entity's Attack is raised by a certain value until it ends.
    /// </summary>
    public class POWUpStatus : StatusEffect
    {
        /// <summary>
        /// The amount to raise the entity's Attack by.
        /// </summary>
        protected int AttackValue = 0;

        public POWUpStatus(int attackValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.POWUp;
            Alignment = StatusAlignments.Positive;

            AttackValue = attackValue;
            Duration = duration;
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.RaiseAttack(AttackValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.LowerAttack(AttackValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.LowerAttack(AttackValue);
        }

        protected override void OnResume()
        {
            EntityAfflicted.RaiseAttack(AttackValue);
        }

        public override StatusEffect Copy()
        {
            return new POWUpStatus(AttackValue, Duration);
        }
    }
}
