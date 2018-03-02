using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Attack FX B Badge - Changes the sounds of Mario's attacks to the sound of a slide whistle or mouse's squeak.
    /// <para>There is a different Attack FX B in each PM game. There's a parameter in the constructor telling which to use.</para>
    /// </summary>
    public sealed class AttackFXBBadge : Badge
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pmAttackFXB">Whether to use the PM version of the badge or not. true for PM's version and false for TTYD's version.</param>
        public AttackFXBBadge(bool pmAttackFXB)
        {
            Name = "Attack FX B";

            if (pmAttackFXB == true)
            {
                Description = "Changes the sound effects when Mario's attacking.";
            }
            else
            {
                Description = "Change the sound effects of Mario's attacks.";
                PriceValue = 50;
            }

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.AttackFXB;
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
