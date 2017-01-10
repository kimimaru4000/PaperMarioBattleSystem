using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The DEFUp Status Effect.
    /// The entity's Defense is raised by a certain value until it ends.
    /// </summary>
    public class DEFUpStatus : StatusEffect
    {
        /// <summary>
        /// The amount to raise the entity's Defense by.
        /// </summary>
        protected int DefenseValue = 0;

        public DEFUpStatus(int defenseValue, int duration)
        {
            StatusType = Enumerations.StatusTypes.DEFUp;
            Alignment = StatusAlignments.Positive;

            DefenseValue = defenseValue;
            Duration = duration;

            AfflictedMessage = "Defense is boosted!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.RaiseDefense(DefenseValue);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.LowerDefense(DefenseValue);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.LowerDefense(DefenseValue);
        }

        protected override void OnResume()
        {
            EntityAfflicted.RaiseDefense(DefenseValue);
        }

        public override StatusEffect Copy()
        {
            return new DEFUpStatus(DefenseValue, Duration);
        }
    }
}