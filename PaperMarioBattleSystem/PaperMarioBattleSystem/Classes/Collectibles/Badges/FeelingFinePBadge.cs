using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Feeling Fine P Badge - Makes Partners immune to most negative StatusEffects.
    /// </summary>
    public sealed class FeelingFinePBadge : FeelingFineBadge
    {
        public FeelingFinePBadge()
        {
            Name = "Feeling Fine P";
            Description = "Make your partner immune to poison or dizziness.";

            BPCost = 4;
            PriceValue = 150;

            BadgeType = BadgeGlobals.BadgeTypes.FeelingFineP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
