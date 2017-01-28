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
            EntityEquipped.EntityProperties.AfflictStatus(new ElectrifiedStatus(StatusGlobals.InfiniteDuration));
        }

        protected override void OnUnequip()
        {
            EntityEquipped.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Electrified);
        }
    }
}
