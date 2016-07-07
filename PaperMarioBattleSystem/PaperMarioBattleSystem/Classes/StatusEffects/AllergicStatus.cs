using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Allergic Status Effect.
    /// All Status Effects affecting the entity are suspended and the entity cannot be inflicted with any new Status Effects
    /// </summary>
    public sealed class AllergicStatus : StatusEffect
    {
        /// <summary>
        /// The list of StatusEffects that are suspended until Allergic ends
        /// </summary>
        private List<StatusEffect> SuspendedStatuses = null;

        public AllergicStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Allergic;
            Alignment = StatusAlignments.Neutral;

            Duration = duration;
        }

        protected override void OnAfflict()
        {
            //Get all the StatusEffects afflicted on the entity and suspend them
            SuspendedStatuses = EntityAfflicted.GetStatuses().ToList();

            //Don't suspend Allergic, as it was just inflicted
            bool removed = SuspendedStatuses.Remove(this);

            for (int i = 0; i < SuspendedStatuses.Count; i++)
            {
                SuspendedStatuses[i].Suspended = true;
            }

            //Add the StatusImmune MiscProperty
            EntityAfflicted.AddMiscProperty(Enumerations.MiscProperty.StatusImmune, new MiscValueHolder(true));

            Debug.Log($"{StatusType} has been inflicted and suspended all StatusEffects on {EntityAfflicted.Name}!");
        }

        protected override void OnEnd()
        {
            //Remove the StatusImmune MiscProperty
            EntityAfflicted.RemoveMiscProperty(Enumerations.MiscProperty.StatusImmune);

            //Unsuspend all of the entity's StatusEffects
            for (int i = 0; i < SuspendedStatuses.Count; i++)
            {
                SuspendedStatuses[i].Suspended = false;
            }

            Debug.Log($"{StatusType} has ended and unsuspended all StatusEffects on {EntityAfflicted.Name}!");

            SuspendedStatuses = null;
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
