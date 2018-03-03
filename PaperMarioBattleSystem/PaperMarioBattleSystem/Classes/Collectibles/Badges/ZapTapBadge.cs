using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Zap Tap Badge - When equipped, makes Mario Electrified.
    /// </summary>
    public sealed class ZapTapBadge : Badge
    {
        public ZapTapBadge()
        {
            Name = "Zap Tap";
            Description = "Do damage to enemies that touch Mario in battle.";

            BPCost = 3;

            BadgeType = BadgeGlobals.BadgeTypes.ZapTap;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.PhaseCycleStartEvent -= InflictElectrified;
            EntityEquipped.PhaseCycleStartEvent += InflictElectrified;

            InflictElectrified();
        }

        protected override void OnUnequip()
        {
            EntityEquipped.PhaseCycleStartEvent -= InflictElectrified;

            RemoveElectrified();
        }

        /// <summary>
        /// Causes Zap Tap to inflict Electrified if the BattleEntity equipped isn't afflicted with it.
        /// This bypasses any immunities or other conditions that would prevent it from being afflicted (Ex. Allergic).
        /// </summary>
        private void InflictElectrified()
        {
            if (EntityEquipped.EntityProperties.HasStatus(Enumerations.StatusTypes.Electrified) == false)
            {
                EntityEquipped.EntityProperties.AfflictStatus(new ElectrifiedStatus(StatusGlobals.InfiniteDuration), false);
            }
        }

        /// <summary>
        /// Removes Electrified the BattleEntity is afflicted with it. This is called when removing the badge.
        /// </summary>
        private void RemoveElectrified()
        {
            if (EntityEquipped.EntityProperties.HasStatus(Enumerations.StatusTypes.Electrified) == true)
            {
                EntityEquipped.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Electrified, false);
            }
        }
    }
}
