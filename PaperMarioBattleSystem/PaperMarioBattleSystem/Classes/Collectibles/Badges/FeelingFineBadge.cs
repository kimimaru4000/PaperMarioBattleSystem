using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Feeling Fine Badge - Makes Mario immune to most negative StatusEffects.
    /// </summary>
    public class FeelingFineBadge : Badge
    {
        public FeelingFineBadge()
        {
            Name = "Feeling Fine";
            Description = "Make Mario immune to poison or dizziness.";

            BPCost = 4;
            PriceValue = 150;

            BadgeType = BadgeGlobals.BadgeTypes.FeelingFine;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected sealed override void OnEquip()
        {
            AddRemoveImmunity(Enumerations.StatusTypes.Electrified, true);
            AddRemoveImmunity(Enumerations.StatusTypes.Poison, true);
            AddRemoveImmunity(Enumerations.StatusTypes.Dizzy, true);
            AddRemoveImmunity(Enumerations.StatusTypes.Confused, true);
            AddRemoveImmunity(Enumerations.StatusTypes.Immobilized, true);
            AddRemoveImmunity(Enumerations.StatusTypes.Sleep, true);
            AddRemoveImmunity(Enumerations.StatusTypes.DEFDown, true);
            AddRemoveImmunity(Enumerations.StatusTypes.POWDown, true);
            AddRemoveImmunity(Enumerations.StatusTypes.Tiny, true);
            AddRemoveImmunity(Enumerations.StatusTypes.KO, true);
        }

        protected sealed override void OnUnequip()
        {
            AddRemoveImmunity(Enumerations.StatusTypes.Electrified, false);
            AddRemoveImmunity(Enumerations.StatusTypes.Poison, false);
            AddRemoveImmunity(Enumerations.StatusTypes.Dizzy, false);
            AddRemoveImmunity(Enumerations.StatusTypes.Confused, false);
            AddRemoveImmunity(Enumerations.StatusTypes.Immobilized, false);
            AddRemoveImmunity(Enumerations.StatusTypes.Sleep, false);
            AddRemoveImmunity(Enumerations.StatusTypes.DEFDown, false);
            AddRemoveImmunity(Enumerations.StatusTypes.POWDown, false);
            AddRemoveImmunity(Enumerations.StatusTypes.Tiny, true);
            AddRemoveImmunity(Enumerations.StatusTypes.KO, false);
        }

        /// <summary>
        /// Helper method to add and remove immunities.
        /// All StatusProperties on the BattleEntity are preserved.
        /// </summary>
        /// <param name="statusType">The StatusType to add or remove an immunity for.</param>
        /// <param name="immune">Whether the BattleEntity is immune to the StatusType or not.</param>
        private void AddRemoveImmunity(Enumerations.StatusTypes statusType, bool immune)
        {
            //Get the StatusProperty
            StatusPropertyHolder statusProperty = EntityEquipped.EntityProperties.GetStatusProperty(statusType);

            //Increase the immunity value by 1 if set to be immune, and decrease it by 1 when clearing immunity
            int immunity = statusProperty.Immunity;
            if (immune == true)
            {
                immunity += 1;
            }
            else
            {
                immunity -= 1;
            }

            //Fill in all of the existing StatusProperty's information to preserve it
            //The only difference is the immunity value
            EntityEquipped.EntityProperties.AddStatusProperty(statusType,
                new StatusPropertyHolder(statusProperty.StatusPercentage, statusProperty.AdditionalTurns, immunity));
        }
    }
}
