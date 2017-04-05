using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Damage Dodge Badge - Adds 1 to Mario's Defense when successfully performing a Guard.
    /// </summary>
    public class DamageDodgeBadge : Badge
    {
        public DamageDodgeBadge()
        {
            Name = "Damage Dodge";
            Description = "Decrease damage by 1 with a Guard Action Command.";

            BPCost = 2;
            PriceValue = 75;

            BadgeType = BadgeGlobals.BadgeTypes.DamageDodge;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected sealed override void OnEquip()
        {
            ///Functionality currently handled in the damage calculation itself <see cref="Interactions.VictimDefensiveStep"/>
        }

        protected sealed override void OnUnequip()
        {
            
        }
    }
}
