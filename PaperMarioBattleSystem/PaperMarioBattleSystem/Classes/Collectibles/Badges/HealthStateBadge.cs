using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Any type of badge that depends on the equipped BattleEntity's HealthState.
    /// This abstract class handles all non-badge specific logic.
    /// </summary>
    public abstract class HealthStateBadge : Badge
    {
        /// <summary>
        /// Tells whether the Badge's effects were applied or not.
        /// </summary>
        private bool EffectsApplied = false;

        /// <summary>
        /// Tells whether the badge can activate or not based on the BattleEntity's HealthState.
        /// </summary>
        protected abstract bool CanActivate { get; }

        protected sealed override void OnEquip()
        {
            EntityEquipped.HealthStateChangedEvent -= OnEntityHealthStateChange;
            EntityEquipped.HealthStateChangedEvent += OnEntityHealthStateChange;

            //Add the effects on equip if the BattleEntity has the required HealthState(s)
            if (CanActivate == true)
                TryAddEffects();
        }

        protected sealed override void OnUnequip()
        {
            EntityEquipped.HealthStateChangedEvent -= OnEntityHealthStateChange;

            TryRemoveEffects();
        }

        /// <summary>
        /// Event handler for the equipped BattleEntity's <see cref="BattleEntity.HealthStateChangedEvent"/>.
        /// </summary>
        /// <param name="newHealthState">The new HealthState of the BattleEntity.</param>
        private void OnEntityHealthStateChange(Enumerations.HealthStates newHealthState)
        {
            if (CanActivate == true)
            {
                TryAddEffects();
            }
            else
            {
                TryRemoveEffects();
            }
        }

        private void TryAddEffects()
        {
            if (EffectsApplied == false)
            {
                ApplyEffects();
                EffectsApplied = true;
            }
        }

        private void TryRemoveEffects()
        {
            if (EffectsApplied == true)
            {
                RemoveEffects();
                EffectsApplied = false;
            }
        }

        /// <summary>
        /// Applies the Badge's effects if they were not already applied.
        /// </summary>
        protected abstract void ApplyEffects();

        /// <summary>
        /// Removes the Badge's effects if they were already applied.
        /// </summary>
        protected abstract void RemoveEffects();
    }
}
