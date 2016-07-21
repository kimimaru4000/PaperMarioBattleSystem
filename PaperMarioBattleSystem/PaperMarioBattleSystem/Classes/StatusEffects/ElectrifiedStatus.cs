using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public sealed class ElectrifiedStatus : StatusEffect
    {
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
            EntityAfflicted.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnResume()
        {
            EntityAfflicted.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        public override StatusEffect Copy()
        {
            return new ElectrifiedStatus(Duration);
        }
    }
}
