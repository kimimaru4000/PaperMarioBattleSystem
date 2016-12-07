using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The NoSkills Status Effect.
    /// The entity has one type of MoveCategory disabled for a number of turns.
    /// <para>The MoveCategory disabled should always be one that isn't currently disabled.</para>
    /// </summary>
    public sealed class NoSkillsStatus : StatusEffect
    {
        /// <summary>
        /// The MoveCategory to disable.
        /// </summary>
        private MoveCategories CategoryDisabled = MoveCategories.None;

        public NoSkillsStatus(MoveCategories categoryDisabled, int duration)
        {
            StatusType = StatusTypes.NoSkills;
            Alignment = StatusAlignments.Negative;

            Duration = duration;

            CategoryDisabled = categoryDisabled;
        }

        //NOTE: I tested on PM and it stacked this status for each command (Hammer, Jump, Items) and returned each one individually
        //We might need to have a different status for each command or somehow manage all of the disabled ones
        //Think about it more

        protected override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.DisableMoveCategory(CategoryDisabled);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.EntityProperties.EnableMoveCategory(CategoryDisabled);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.EntityProperties.EnableMoveCategory(CategoryDisabled);
        }

        protected override void OnResume()
        {
            EntityAfflicted.EntityProperties.DisableMoveCategory(CategoryDisabled);
        }

        public override StatusEffect Copy()
        {
            return new NoSkillsStatus(CategoryDisabled, Duration);
        }
    }
}
