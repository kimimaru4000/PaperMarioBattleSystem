using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX A Badge - Changes the sounds of Mario's attacks to the sound of a whistle.
    /// </summary>
    public sealed class AttackFXABadge : AttackFXBadge
    {
        public AttackFXABadge()
        {
            Name = "Attack FX A";
            Description = "Changes the sound effects when\nMario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXA;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }
    }
}
