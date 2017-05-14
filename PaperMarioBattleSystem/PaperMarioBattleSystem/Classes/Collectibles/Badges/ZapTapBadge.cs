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
    //NOTE: You can use a Volt Shroom with this Badge equipped. When that happens, the Volt Shroom inflicts Electrified for 5 turns,
    //then when it ends, it does the "status ended" event and still shows you with an infinite Electrified status.
    //The current implementation won't allow this, so find a way to make it work
    //My current thoughts are to inflict Electrified at the start of each phase cycle if the entity isn't afflicted with it
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
