using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Close Call P Badge - Increases Partner's Evasion by about 33% when he or she is in Danger or Peril.
    /// </summary>
    public sealed class CloseCallPBadge : CloseCallBadge
    {
        public CloseCallPBadge()
        {
            Name = "Close Call P";
            Description = "When an ally's in Danger, cause foes to sometimes miss.";

            BadgeType = BadgeGlobals.BadgeTypes.CloseCallP;
            AffectedType = BadgeGlobals.AffectedTypes.Partner;
        }
    }
}
