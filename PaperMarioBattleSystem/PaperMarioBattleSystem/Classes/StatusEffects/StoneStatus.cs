using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stone Status Effect.
    /// The entity afflicted with it is immobilized and immune to damage and negative StatusEffects.
    /// The entity also has several StatusEffects suppressed in several ways until it ends.
    /// <para>This has a Positive Alignment because entities that attack an entity afflicted with this basically waste their turns.</para>
    /// </summary>
    public sealed class StoneStatus : StopStatus
    {
        public StoneStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Stone;
            Alignment = StatusAlignments.Positive;

            StatusIcon = null;

            //Stone doesn't have a Battle Message when used
            AfflictedMessage = string.Empty;
        }

        protected sealed override void OnAfflict()
        {
            base.OnAfflict();

            //Stone suppresses Electrified, Poison, Invisible, and Tiny's turn counts
            //It suppresses the effects of Electrified and Poison, it and suppresses the VFX and Icon of Electrified
            EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.TurnCount, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Invisible, StatusTypes.Tiny);
            EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.Effects, StatusTypes.Electrified, StatusTypes.Poison);
            EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.VFX, StatusTypes.Electrified);
            EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.Icon, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Tiny);

            //Add the Invincible AdditionalProperty
            EntityAfflicted.AddIntAdditionalProperty(Enumerations.AdditionalProperty.Invincible, 1);

            EntityAfflicted.AnimManager.PlayAnimation(AnimationGlobals.StatusBattleAnimations.StoneName);

            HandleStatusImmunities(true);

            Debug.Log($"{StatusType} has been inflicted on {EntityAfflicted.Name}!");
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();

            //Remove the Invincible AdditionalProperty
            EntityAfflicted.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.Invincible, 1);

            //Unsuppress the statuses it suppressed in this way
            EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.TurnCount, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Invisible, StatusTypes.Tiny);
            EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.Effects, StatusTypes.Electrified, StatusTypes.Poison);
            EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.VFX, StatusTypes.Electrified);
            EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.Icon, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Tiny);

            EntityAfflicted.AnimManager.PlayAnimation(EntityAfflicted.GetIdleAnim());

            HandleStatusImmunities(false);

            Debug.Log($"{StatusType} has ended on {EntityAfflicted.Name}!");
        }

        protected sealed override void OnSuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            base.OnSuppress(statusSuppressionType);

            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                //Remove the Invincible AdditionalProperty
                EntityAfflicted.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.Invincible, 1);

                //Unsuppress the statuses it suppressed in this way
                EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.TurnCount, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Invisible, StatusTypes.Tiny);
                EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.Effects, StatusTypes.Electrified, StatusTypes.Poison);
                EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.VFX, StatusTypes.Electrified);
                EntityAfflicted.EntityProperties.UnsuppressStatuses(StatusSuppressionTypes.Icon, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Tiny);

                EntityAfflicted.AnimManager.PlayAnimation(EntityAfflicted.GetIdleAnim());

                HandleStatusImmunities(false);

                Debug.Log($"{StatusType} has ended on {EntityAfflicted.Name}!");
            }
        }

        protected sealed override void OnUnsuppress(Enumerations.StatusSuppressionTypes statusSuppressionType)
        {
            base.OnUnsuppress(statusSuppressionType);

            if (statusSuppressionType == Enumerations.StatusSuppressionTypes.Effects)
            {
                //Resume suppressing the statuses again
                EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.TurnCount, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Invisible, StatusTypes.Tiny);
                EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.Effects, StatusTypes.Electrified, StatusTypes.Poison);
                EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.VFX, StatusTypes.Electrified);
                EntityAfflicted.EntityProperties.SuppressStatuses(StatusSuppressionTypes.Icon, StatusTypes.Electrified, StatusTypes.Poison, StatusTypes.Tiny);

                //Add the Invincible AdditionalProperty
                EntityAfflicted.AddIntAdditionalProperty(Enumerations.AdditionalProperty.Invincible, 1);

                EntityAfflicted.AnimManager.PlayAnimation(AnimationGlobals.StatusBattleAnimations.StoneName);

                HandleStatusImmunities(true);

                Debug.Log($"{StatusType} has been inflicted on {EntityAfflicted.Name}!");
            }
        }

        public sealed override StatusEffect Copy()
        {
            return new StoneStatus(Duration);
        }

        /// <summary>
        /// Handles adding/removing Stone's Status Effect immunities.
        /// </summary>
        /// <param name="immune">Whether to add or remove the immunity.</param>
        private void HandleStatusImmunities(bool immune)
        {
            EntityAfflicted.AddRemoveStatusImmunity(StatusTypes.Poison, immune);
            EntityAfflicted.AddRemoveStatusImmunity(StatusTypes.Dizzy, immune);
            EntityAfflicted.AddRemoveStatusImmunity(StatusTypes.Sleep, immune);
            EntityAfflicted.AddRemoveStatusImmunity(StatusTypes.Tiny, immune);
            EntityAfflicted.AddRemoveStatusImmunity(StatusTypes.Frozen, immune);
            EntityAfflicted.AddRemoveStatusImmunity(StatusTypes.NoSkills, immune);
        }
    }
}
