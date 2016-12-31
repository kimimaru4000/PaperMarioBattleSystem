using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Group Focus Badge - Allows Mario's Partners to use Focus.
    /// </summary>
    public sealed class GroupFocusBadge : Badge
    {
        public GroupFocusBadge()
        {
            Name = "Group Focus";
            Description = "Lets Mario's partners use the Focus command, as well.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.GroupFocus;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }

        protected override void OnEquip()
        {
            
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
