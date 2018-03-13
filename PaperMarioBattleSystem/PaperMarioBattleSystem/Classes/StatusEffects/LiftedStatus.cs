using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Lifted Status Effect.
    /// Entities afflicted with it are lifted out of battle, essentially dying.
    /// <para>This Status Effect isn't technically used, as it correlates to Parakarry's Air Lift move, which handles removing the enemy from battle.
    /// However, its percentage of being afflicted on the victim determines if the move is successful or not.</para>
    /// </summary>
    public abstract class LiftedStatus : StatusEffect
    {
        public LiftedStatus()
        {
            StatusType = Enumerations.StatusTypes.Lifted;
            Alignment = StatusAlignments.Negative;

            //Lifted doesn't have an icon
            StatusIcon = null;

            //Lifted doesn't have a duration
            Duration = 1;
        }

        protected override void OnAfflict()
        {
            
        }

        protected override void OnEnd()
        {

        }

        protected override void OnPhaseCycleStart()
        {

        }

        protected sealed override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {

        }

        protected sealed override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {

        }
    }
}
