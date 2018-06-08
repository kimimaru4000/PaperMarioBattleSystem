using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Right On! Badge - Causes most Action Commands and all Stylish Moves to automatically be successfully performed.
    /// </summary>
    public sealed class RightOnBadge : Badge
    {
        public RightOnBadge()
        {
            Name = "Right On!";
            Description = "Makes the action command work\nevery time Mario attacks.";

            BPCost = 8;

            BadgeType = BadgeGlobals.BadgeTypes.RightOn;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.AddIntAdditionalProperty(Enumerations.AdditionalProperty.AutoActionCommands, 1);
            EntityEquipped.AddIntAdditionalProperty(Enumerations.AdditionalProperty.AutoStylishMoves, 1);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.AutoActionCommands, 1);
            EntityEquipped.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.AutoStylishMoves, 1);
        }
    }
}
