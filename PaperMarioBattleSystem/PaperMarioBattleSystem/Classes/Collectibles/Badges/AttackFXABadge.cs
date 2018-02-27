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
    public sealed class AttackFXABadge : Badge
    {
        public AttackFXABadge()
        {
            Name = "Attack FX A";
            Description = "Changes the sound effects when Mario's attacking.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXA;
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
