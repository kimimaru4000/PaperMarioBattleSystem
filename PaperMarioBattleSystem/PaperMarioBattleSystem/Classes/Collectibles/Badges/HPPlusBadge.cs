using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The HP Plus Badge - Raises Mario's Max HP by 5.
    /// </summary>
    public class HPPlusBadge : Badge
    {
        public const int HPValue = 5;

        public HPPlusBadge()
        {
            Name = "HP Plus";
            Description = $"Increases maximum HP by {HPValue}.";

            BPCost = 3;
            PriceValue = 75;

            BadgeType = BadgeGlobals.BadgeTypes.HPPlus;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.RaiseMaxHP(HPValue);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.LowerMaxHP(HPValue);
        }
    }
}
