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
    public sealed class AttackFXEBadge : Badge
    {
        public AttackFXEBadge()
        {
            Name = "Attack FX E";
            Description = "Changes the sound effects when Mario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXE;
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
