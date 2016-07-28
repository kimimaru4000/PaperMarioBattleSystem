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
        }

        protected override void OnAfflict()
        {
            //Make the entity immune to all StatusEffects
            EntityAfflicted.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.PositiveStatusImmune, new MiscValueHolder(true));
            EntityAfflicted.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.NeutralStatusImmune, new MiscValueHolder(true));
            EntityAfflicted.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.NegativeStatusImmune, new MiscValueHolder(true));
        }

        protected override void OnEnd()
        {
            //Remove the StatusEffect immunities
            EntityAfflicted.EntityProperties.RemoveMiscProperty(Enumerations.MiscProperty.PositiveStatusImmune);
            EntityAfflicted.EntityProperties.RemoveMiscProperty(Enumerations.MiscProperty.NegativeStatusImmune);
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
