using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The KO Status Effect.
    /// Entities afflicted with it instantly die.
    /// </summary>
    public class KOStatus : StatusEffect
    {
        public KOStatus()
        {
            StatusType = Enumerations.StatusTypes.KO;
            Alignment = StatusAlignments.Negative;

            //KO doesn't have a duration, as once it's inflicted, the entity dies
            Duration = 1;
        }

        protected sealed override void OnAfflict()
        {
            //Instantly kill entities afflicted with KO
            EntityAfflicted.Die();
        }

        protected sealed override void OnEnd()
        {
            
        }

        protected sealed override void OnPhaseCycleStart()
        {
            
        }

        //KO cannot be suspended, as it instantly kills any entity afflicted with it
        protected sealed override void OnSuspend()
        {
            
        }

        protected sealed override void OnResume()
        {
            
        }

        public override StatusEffect Copy()
        {
            return new KOStatus();
        }
    }
}
