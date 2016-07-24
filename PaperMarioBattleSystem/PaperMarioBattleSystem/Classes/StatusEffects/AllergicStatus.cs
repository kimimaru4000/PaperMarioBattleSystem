using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Allergic Status Effect.
    /// All Status Effects affecting the entity are suspended, and the entity cannot be inflicted with any new Status Effects
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
            //Suspend all entity's StatusEffects aside from this one
            EntityAfflicted.SuspendOrResumeStatuses(true, StatusType);

            //Make the entity immune to all StatusEffects
            EntityAfflicted.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.PositiveStatusImmune, new MiscValueHolder(true));
            EntityAfflicted.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.NegativeStatusImmune, new MiscValueHolder(true));

            Debug.Log($"{StatusType} has been inflicted and Suspended all StatusEffects on {EntityAfflicted.Name}!");
        }

        protected override void OnEnd()
        {
            //Remove the StatusEffect immunities
            EntityAfflicted.EntityProperties.RemoveMiscProperty(Enumerations.MiscProperty.PositiveStatusImmune);
            EntityAfflicted.EntityProperties.RemoveMiscProperty(Enumerations.MiscProperty.NegativeStatusImmune);

            //Resume all of the entity's StatusEffects
            EntityAfflicted.SuspendOrResumeStatuses(false, StatusType);

            Debug.Log($"{StatusType} has ended and Resumed all StatusEffects on {EntityAfflicted.Name}!");
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            Debug.LogError($"The {StatusType} Status CANNOT be Suspended. Remove any cases where it is suspended");
        }

        protected override void OnResume()
        {
            Debug.LogError($"The {StatusType} Status CANNOT be Resumed because it CANNOT be Suspended. Remove any cases where it is suspended");
        }

        public override StatusEffect Copy()
        {
            return new AllergicStatus(Duration);
        }
    }
}
