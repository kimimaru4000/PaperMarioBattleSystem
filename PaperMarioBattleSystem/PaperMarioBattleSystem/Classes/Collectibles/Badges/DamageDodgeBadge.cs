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
            //Add 1 to AdditionalGuardDefense
            //int additional = EntityEquipped.EntityProperties.GetMiscProperty(AdditionalProperty.AdditionalGuardDefense).IntValue + 1;

            //EntityEquipped.EntityProperties.AddMiscProperty(AdditionalProperty.AdditionalGuardDefense, new MiscValueHolder(additional));
        }

        protected sealed override void OnUnequip()
        {
            //Subtract 1 from AdditionalGuardDefense
            //int subtracted = EntityEquipped.EntityProperties.GetMiscProperty(AdditionalProperty.AdditionalGuardDefense).IntValue - 1;
            //EntityEquipped.EntityProperties.RemoveMiscProperty(AdditionalProperty.AdditionalGuardDefense);

            //if (subtracted > 0)
            //{
            //    EntityEquipped.EntityProperties.AddMiscProperty(AdditionalProperty.AdditionalGuardDefense, new MiscValueHolder(subtracted));
            //}
        }
    }
}
