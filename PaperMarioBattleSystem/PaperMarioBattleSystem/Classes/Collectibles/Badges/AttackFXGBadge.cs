using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX G Badge - Changes the sounds of Mario's attacks to the sound of a giggle.
    /// </summary>
    public sealed class AttackFXGBadge : Badge
    {
        public AttackFXGBadge()
        {
            Name = "Attack FX G";
            Description = "Change the sound effects of Mario's attacks.";

            BPCost = 0;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXG;
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
