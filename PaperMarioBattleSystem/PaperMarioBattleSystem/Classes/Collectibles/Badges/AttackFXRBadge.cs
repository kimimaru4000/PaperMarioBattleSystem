using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX R Badge - Changes the sounds of Mario's attacks to the sound of a cricket.
    /// </summary>
    public sealed class AttackFXRBadge : Badge
    {
        public AttackFXRBadge()
        {
            Name = "Attack FX R";
            Description = "Change the sound effects of Mario's attacks.";

            BPCost = 0;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXR;
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
