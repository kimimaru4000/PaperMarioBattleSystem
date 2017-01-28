using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Close Call Badge - Increases Mario's Evasion by about 33% when he's in Danger or Peril.
    /// </summary>
    public class CloseCallBadge : Badge
    {
        public CloseCallBadge()
        {
            Name = "Close Call";
            Description = "When Mario's in Danger, cause enemies to sometimes miss.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.CloseCall;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected sealed override void OnEquip()
        {
            
        }

        protected sealed override void OnUnequip()
        {
            
        }
    }
}
