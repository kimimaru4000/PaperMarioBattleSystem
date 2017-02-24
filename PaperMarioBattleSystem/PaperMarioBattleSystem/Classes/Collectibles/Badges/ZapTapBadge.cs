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
            EntityEquipped.EntityProperties.AfflictStatus(new ElectrifiedStatus(StatusGlobals.InfiniteDuration));
        }

        protected override void OnUnequip()
        {
            EntityEquipped.EntityProperties.RemoveStatus(Enumerations.StatusTypes.Electrified);
        }
    }
}
