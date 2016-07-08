using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public sealed class ElectrifiedStatus : StatusEffect
    {
        /// <summary>
        /// Indicates whether the entity was Electrified via the status or not
        /// </summary>
        private bool ElectrifiedEntity = false;

        public ElectrifiedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Electrified;
            //Despite having positive effects, Electrified is classified as a Negative StatusEffect.
            //Stone Caps suppress Electrified
            Alignment = StatusAlignments.Negative;

            Duration = duration;
        }

        protected override void OnAfflict()
        {
            //If the entity is already Electrified, remove the status as it's useless
            if (EntityAfflicted.HasPhysAttributes(true, Enumerations.PhysicalAttributes.Electrified) == true)
            {
                EntityAfflicted.RemoveStatus(Enumerations.StatusTypes.Electrified);
            }
            else
            {
                EntityAfflicted.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);

                //Mark the entity as having received Electrified via this status
                ElectrifiedEntity = true;
            }
        }

        protected override void OnEnd()
        {
            //If the entity was given Electrified via the status, remove it
            if (ElectrifiedEntity == true)
            {
                EntityAfflicted.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            }

            ElectrifiedEntity = false;
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            if (ElectrifiedEntity == true)
            {
                EntityAfflicted.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            }
        }

        protected override void OnResume()
        {
            if (ElectrifiedEntity == true)
            {
                EntityAfflicted.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
            }
        }

        public override StatusEffect Copy()
        {
            return new ElectrifiedStatus(Duration);
        }
    }
}
