using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Multibounce Badge - Gives Mario the ability to use Multibounce
    /// </summary>
    public sealed class MultibounceBadge : Badge
    {
        public MultibounceBadge()
        {
            Name = "Multibounce";
            Description = "Wear this to use Multibounce. 2 FP are required to use this attack, which lets you attack multiple enemies"
                + " in order until you miss an Action Command. Wearing two or more of these badges requires more FP for the Attack power.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.Multibounce;
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
