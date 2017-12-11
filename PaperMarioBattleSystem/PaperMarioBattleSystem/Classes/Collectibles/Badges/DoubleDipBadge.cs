using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Double Dip Badge - Allows Mario to use two items in one turn.
    /// <para>If two of these are equipped, it allows Mario to use Triple Dip, which allows three items to be used in one turn.</para>
    /// </summary>
    public class DoubleDipBadge : Badge
    {
        public DoubleDipBadge()
        {
            Name = "Double Dip";
            Description = "Wear this to become able to use two items during Mario's turn in battle. " +
                "This move requires 4 FP. Wearing two of these increases the required FP, but allows Mario to use up to three items.";

            BPCost = 3;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.DoubleDip;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            ///Functionality currently handled in the <see cref="PlayerBattleMenu"/> and the <see cref="ItemDipSubMenu"/>
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
