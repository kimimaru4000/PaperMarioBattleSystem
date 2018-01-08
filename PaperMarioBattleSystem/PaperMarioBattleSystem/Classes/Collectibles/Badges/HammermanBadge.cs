using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Hammerman Badge - Removes Mario's ability to use Jump, but boosts the Attack of all Hammer moves by 1.
    /// </summary>
    public sealed class HammermanBadge : Badge
    {
        public HammermanBadge()
        {
            Name = "Hammerman";
            Description = "Increase hammer power by 1, but lose the ability to jump.";

            BPCost = 2;
            PriceValue = 150;

            BadgeType = BadgeGlobals.BadgeTypes.Hammerman;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.RaiseAttack(1);

            EntityEquipped.EntityProperties.DisableMoveCategory(Enumerations.MoveCategories.Jump);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.LowerAttack(1);

            EntityEquipped.EntityProperties.EnableMoveCategory(Enumerations.MoveCategories.Jump);
        }
    }
}
