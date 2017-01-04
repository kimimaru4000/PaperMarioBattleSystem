using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Electrified Status Effect.
    /// This grants the Electrified PhysicalAttribute to the entity, causing direct contact from non-Electrified entities to hurt the attacker.
    /// </summary>
    public sealed class ElectrifiedStatus : StatusEffect
    {
        public ElectrifiedStatus(int duration)
        {
            StatusType = Enumerations.StatusTypes.Electrified;
            //Despite having positive effects, Electrified is classified as a Negative StatusEffect.
            //Stone Caps suppress Electrified
            Alignment = StatusAlignments.Negative;

            Duration = duration;

            AfflictedMessage = "Electrified! Enemies that make contact will get hurt!";
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnEnd()
        {
            EntityAfflicted.EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnPhaseCycleStart()
        {
            IncrementTurns();
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.EntityProperties.RemovePhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        protected override void OnResume()
        {
            EntityAfflicted.EntityProperties.AddPhysAttribute(Enumerations.PhysicalAttributes.Electrified);
        }

        public override StatusEffect Copy()
        {
            return new ElectrifiedStatus(Duration);
        }
    }
}
