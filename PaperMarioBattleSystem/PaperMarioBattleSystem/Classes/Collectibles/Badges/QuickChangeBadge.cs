using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Quick Change Badge - Allows Mario or his Partner to switch Partners without using up a turn
    /// </summary>
    public sealed class QuickChangeBadge : Badge
    {
        public QuickChangeBadge()
        {
            Name = "Quick Change";
            Description = "During battle, lets you change your party members... and still use the new member without losing a turn.";

            BPCost = 7;

            BadgeType = BadgeGlobals.BadgeTypes.QuickChange;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            ///Functionality currently handled in the <see cref="ChangePartnerAction"/> action
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
