using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Last Stand - Cuts the damage Mario takes in half (rounded up) when in Danger or Peril.
    /// </summary>
    public class LastStandBadge : Badge
    {
        public LastStandBadge()
        {
            Name = "Last Stand";
            Description = "Drop damage Mario receives by 1/2 when he is in Danger.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.LastStand;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            ///Functionality currently handled in the damage calculation itself <see cref="Interactions.VictimLastStandStep"/>
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
