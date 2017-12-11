using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Dizzy Attack Badge - Allows Mario to inflict Dizzy on the first enemy in battle if he Spin Dashes into it on the field.
    /// </summary>
    public sealed class DizzyAttackBadge : Badge
    {
        public DizzyAttackBadge()
        {
            Name = "Dizzy Attack";
            Description = "Delivers a blow that makes an enemy dizzy and unable to move.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.DizzyAttack;
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
