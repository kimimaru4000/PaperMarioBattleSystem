using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Close Call Badge - Increases Mario's Evasion by 33% (30% in PM) when he's in Danger or Peril.
    /// </summary>
    public class CloseCallBadge : Badge
    {
        protected const double EvasionValue = .66d;

        /// <summary>
        /// Tells whether the Evasion bonus was granted or not.
        /// </summary>
        private bool GaveBonus = false;

        public CloseCallBadge()
        {
            Name = "Close Call";
            Description = "When Mario's in Danger, cause enemies to sometimes miss.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.CloseCall;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected sealed override void OnEquip()
        {
            EntityEquipped.HealthStateChangedEvent -= OnEntityHealthStateChange;
            EntityEquipped.HealthStateChangedEvent += OnEntityHealthStateChange;

            //Add the Evasion bonus on equip if the BattleEntity is in Danger or Peril
            if (EntityEquipped.IsInDanger == true)
                AddEvasionBonus();
        }

        protected sealed override void OnUnequip()
        {
            EntityEquipped.HealthStateChangedEvent -= OnEntityHealthStateChange;

            RemoveEvasionBonus();
        }

        private void OnEntityHealthStateChange(Enumerations.HealthStates newHealthState)
        {
            if (EntityEquipped.IsInDanger == true)
            {
                AddEvasionBonus();
            }
            else
            {
                RemoveEvasionBonus();
            }
        }

        /// <summary>
        /// Adds Close Call's Evasion value if it wasn't already added.
        /// </summary>
        private void AddEvasionBonus()
        {
            if (GaveBonus == false)
            {
                EntityEquipped.AddEvasionMod(EvasionValue);
                GaveBonus = true;
            }
        }

        /// <summary>
        /// Removes Close Call's Evasion value if it was added.
        /// </summary>
        private void RemoveEvasionBonus()
        {
            if (GaveBonus == true)
            {
                EntityEquipped.RemoveEvasionMod(EvasionValue);
                GaveBonus = false;
            }
        }
    }
}
