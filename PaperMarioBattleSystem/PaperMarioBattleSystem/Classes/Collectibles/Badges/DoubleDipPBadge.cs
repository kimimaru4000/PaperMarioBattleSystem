using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem.Classes.Collectibles.Badges
{
    /// <summary>
    /// The Double Dip P Badge - Allows Mario's Partner to use two items in one turn.
    /// <para>If two of these are equipped, it allows Mario's Partner to use Triple Dip, which allows three items to be used in one turn.</para>
    /// </summary>
    public sealed class DoubleDipPBadge : DoubleDipBadge
    {
        public DoubleDipPBadge()
        {
            Name = "Double Dip P";
            Description = "Wear this to allow your partner to use two items in one turn in battle. " +
                "This move requires 4 FP. Wearing two of these increases the required FP, but allows Mario's partner to use up to three items.";

            BadgeType = BadgeGlobals.BadgeTypes.DoubleDipP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
