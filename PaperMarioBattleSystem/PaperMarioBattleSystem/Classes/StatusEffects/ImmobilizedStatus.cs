using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Immobilized Status Effect.
    /// Entities afflicted with this cannot move until it wears off.
    /// <para>Mario and his Partner cannot Guard or Superguard when afflicted with this Status Effect.</para>
    /// </summary>
    public class ImmobilizedStatus : StatusEffect
    {
        public ImmobilizedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Immobilized;
            Alignment = StatusAlignments.Negative;

            Duration = duration;

            AfflictedMessage = "Immobilized! Movement will\nbe impossible for a while!";
        }

        protected override void OnAfflict()
        {
            //Prevent the entity from moving on affliction and mark it as using up all of its turns
            EntityAfflicted.SetMaxTurns(0);
            EntityAfflicted.SetTurnsUsed(EntityAfflicted.MaxTurns);

            //Specify that this status makes the entity Immobile
            int immobile = EntityAfflicted.EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.Immobile) + 1;
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Immobile, immobile);

            Debug.Log($"{StatusType} set MaxTurns to 0 for {EntityAfflicted.Name}");
        }

        protected override void OnEnd()
        {
            if (EntityAfflicted.MaxTurns > 0 && EntityAfflicted.MaxTurns < EntityAfflicted.BaseTurns)
            {
                EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);
                Debug.Log($"{StatusType} set MaxTurns to {EntityAfflicted.BaseTurns} for {EntityAfflicted.Name}");
            }

            //Remove the Immobile property after getting its count
            int immobile = EntityAfflicted.EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.Immobile) - 1;
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Immobile);

            if (immobile > 0)
            {
                EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Immobile, immobile);
            }
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
            if (IsFinished == false)
            {
                EntityAfflicted.SetMaxTurns(0);
                Debug.Log($"{StatusType} set MaxTurns to 0 for {EntityAfflicted.Name}");
            }
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.SetMaxTurns(EntityAfflicted.BaseTurns);

            //Remove the Immobile property
            int immobile = EntityAfflicted.EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.Immobile) - 1;
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Immobile);

            if (immobile > 0)
            {
                EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Immobile, immobile);
            }
        }

        protected override void OnResume()
        {
            EntityAfflicted.SetMaxTurns(0);

            //Re-add the Immobile property
            int immobile = EntityAfflicted.EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.Immobile) + 1;
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Immobile, immobile);
        }

        public override StatusEffect Copy()
        {
            return new ImmobilizedStatus(Duration);
        }
    }
}
