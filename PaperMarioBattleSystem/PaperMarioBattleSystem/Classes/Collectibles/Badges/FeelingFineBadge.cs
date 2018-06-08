using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Extensions;

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
            HandleStatusImmunities(true);
        }

        protected sealed override void OnUnequip()
        {
            HandleStatusImmunities(false);
        }

        /// <summary>
        /// Handles adding/removing Feeling Fine's Status Effect immunities.
        /// </summary>
        /// <param name="immune">Whether to add or remove the immunity.</param>
        private void HandleStatusImmunities(bool immune)
        {
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Electrified, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Poison, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Dizzy, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Confused, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Stop, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Sleep, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.DEFDown, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.POWDown, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.Tiny, immune);
            EntityEquipped.AddRemoveStatusImmunity(Enumerations.StatusTypes.KO, immune);
        }
    }
}
