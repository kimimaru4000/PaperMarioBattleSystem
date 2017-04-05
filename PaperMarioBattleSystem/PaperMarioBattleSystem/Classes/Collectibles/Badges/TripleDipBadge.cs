using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Triple Dip Badge - Allows Mario to use three items in one turn.
    /// </summary>
    public sealed class TripleDipBadge : Badge
    {
        public TripleDipBadge()
        {
            Name = "Triple Dip";
            Description = "During battle, lets you use three items during one turn.";

            BPCost = 3;

            BadgeType = BadgeGlobals.BadgeTypes.TripleDip;
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
