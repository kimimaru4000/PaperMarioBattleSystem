using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX C Badge - Changes the sounds of Mario's attacks to the sound of a mechanical noise.
    /// </summary>
    public sealed class AttackFXCBadge : Badge
    {
        public AttackFXCBadge()
        {
            Name = "Attack FX C";
            Description = "Changes the sound effects when Mario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXC;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {

        }

        protected override void OnUnequip()
        {

        }
    }
}
