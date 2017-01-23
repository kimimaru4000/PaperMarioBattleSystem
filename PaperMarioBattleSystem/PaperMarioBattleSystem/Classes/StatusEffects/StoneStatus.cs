using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Stone Status Effect.
    /// The entity afflicted with it is immobilized and immune to damage and negative StatusEffects.
    /// The entity also has its negative StatusEffects suppressed until it ends.
    /// <para>This has a Positive Alignment because entities that attack an entity afflicted with this basically waste their turns.</para>
    /// </summary>
    public sealed class StoneStatus : ImmobilizedStatus
    {
        public StoneStatus(int duration) : base(duration)
        {
            StatusType = Enumerations.StatusTypes.Stone;
            Alignment = StatusAlignments.Positive;

            //Stone doesn't have a Battle Message when used
            AfflictedMessage = string.Empty;
        }

        protected sealed override void OnAfflict()
        {
            base.OnAfflict();

            //Suspend all entity's Negative StatusEffects
            EntityAfflicted.SuspendOrResumeAlignmentStatuses(true, StatusAlignments.Negative, StatusType);

            //Add the NegativeStatusImmune and Invincible MiscProperties
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune, true);
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Invincible, true);

            EntityAfflicted.PlayAnimation(AnimationGlobals.StatusBattleAnimations.StoneName);

            Debug.Log($"{StatusType} has been inflicted and Suspended all negative StatusEffects on {EntityAfflicted.Name}!");
        }

        protected sealed override void OnEnd()
        {
            base.OnEnd();

            //Remove the NegativeStatusImmune and Invincible MiscProperties
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune);
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Invincible);

            //Resume all of the entity's Negative StatusEffects
            EntityAfflicted.SuspendOrResumeAlignmentStatuses(false, StatusAlignments.Negative, StatusType);

            EntityAfflicted.PlayAnimation(EntityAfflicted.GetIdleAnim());

            Debug.Log($"{StatusType} has ended and Resumed all negative StatusEffects on {EntityAfflicted.Name}!");
        }

        protected sealed override void OnSuspend()
        {
            base.OnSuspend();

            //Remove Invincibility and don't do anything else to avoid Suspending/Resuming conflicts with other StatusEffects
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.Invincible);

            EntityAfflicted.PlayAnimation(EntityAfflicted.GetIdleAnim());
        }

        protected sealed override void OnResume()
        {
            base.OnResume();

            //Suspend all entity's Negative StatusEffects once again
            EntityAfflicted.SuspendOrResumeAlignmentStatuses(true, StatusAlignments.Negative, StatusType);

            //Add back the NegativeStatusImmune and Invincible MiscProperties
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.NegativeStatusImmune, true);
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.Invincible, true);

            EntityAfflicted.PlayAnimation(AnimationGlobals.StatusBattleAnimations.StoneName);
        }

        public sealed override StatusEffect Copy()
        {
            return new StoneStatus(Duration);
        }
    }
}
