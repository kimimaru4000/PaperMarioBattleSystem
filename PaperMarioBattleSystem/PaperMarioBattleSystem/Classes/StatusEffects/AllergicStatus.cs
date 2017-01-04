using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Allergic Status Effect.
    /// The Entity afflicted cannot be inflicted with any new Status Effects.
    /// </summary>
    public sealed class AllergicStatus : StatusEffect
    {
        public AllergicStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Allergic;
            Alignment = StatusAlignments.Neutral;

            Duration = duration;

            AfflictedMessage = "Status hasn't changed!";
        }

        protected override void OnAfflict()
        {
            //Make the entity immune to all StatusEffects
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.PositiveStatusImmune, true);
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.NeutralStatusImmune, true);
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune, true);
        }

        protected override void OnEnd()
        {
            //Remove the StatusEffect immunities
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.PositiveStatusImmune);
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.NeutralStatusImmune);
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            
        }

        protected override void OnResume()
        {
            
        }

        public override StatusEffect Copy()
        {
            return new AllergicStatus(Duration);
        }
    }
}
