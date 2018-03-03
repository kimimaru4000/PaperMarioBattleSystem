using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX D Badge - Changes the sounds of Mario's attacks to the sound of a bell.
    /// </summary>
    public sealed class AttackFXDBadge : AttackFXBadge
    {
        public AttackFXDBadge()
        {
            Name = "Attack FX D";
            Description = "Changes the sound effects when\nMario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXD;
            AffectedType = BadgeGlobals.AffectedTypes.Self;

            SoundToPlay = SoundManager.Sound.AttackFXD;
        }
    }
}
