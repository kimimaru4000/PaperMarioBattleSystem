using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Jumpman Badge - Removes Mario's ability to use Hammer, but boosts the Attack of all Jump moves by 1.
    /// </summary>
    public sealed class JumpmanBadge : Badge
    {
        public JumpmanBadge()
        {
            Name = "Jumpman";
            Description = "Increase jump power by 1, but lose your hammer ability.";

            BPCost = 2;
            PriceValue = 150;

            BadgeType = BadgeGlobals.BadgeTypes.Jumpman;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.RaiseAttack(1);

            EntityEquipped.EntityProperties.DisableMoveCategory(Enumerations.MoveCategories.Hammer);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.LowerAttack(1);

            EntityEquipped.EntityProperties.EnableMoveCategory(Enumerations.MoveCategories.Hammer);
        }
    }
}
