using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Invisible Status Effect.
    /// The entity becomes transparent and its Evasion is set to the max value until it ends.
    /// </summary>
    public sealed class InvisibleStatus : StatusEffect
    {
        public InvisibleStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Invisible;
            Alignment = StatusAlignments.Positive;

            Duration = duration;

            AfflictedMessage = "Invisible! Attacks will now miss!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.ModifyEvasion(100);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.ModifyEvasion(-100);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            //Invisible is unique in that it's still active during suspension 
        }

        protected override void OnResume()
        {
           //Invisible is unique in that it's still active during suspension
        }

        public override StatusEffect Copy()
        {
            return new InvisibleStatus(Duration);
        }
    }
}
