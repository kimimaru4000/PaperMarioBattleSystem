using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX E Badge - Changes the sounds of Mario's attacks to the sound of a Yoshi.
    /// </summary>
    public sealed class AttackFXEBadge : AttackFXBadge
    {
        public AttackFXEBadge()
        {
            Name = "Attack FX E";
            Description = "Changes the sound effects when\nMario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXE;
            AffectedType = BadgeGlobals.AffectedTypes.Self;

            SoundToPlay = SoundManager.Sound.AttackFXE;
        }
    }
}
