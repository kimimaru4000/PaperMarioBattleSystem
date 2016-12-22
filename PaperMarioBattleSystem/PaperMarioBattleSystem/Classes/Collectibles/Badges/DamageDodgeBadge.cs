using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    public sealed class DamageDodgeBadge : Badge
    {
        public DamageDodgeBadge()
        {
            Name = "Damage Dodge";
            Description = "Decrease damage by 1 with a Guard Action Command.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.DamageDodge;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            //Add 1 to AdditionalGuardDefense
            //int additional = EntityEquipped.EntityProperties.GetMiscProperty(AdditionalProperty.AdditionalGuardDefense).IntValue + 1;

            //EntityEquipped.EntityProperties.AddMiscProperty(AdditionalProperty.AdditionalGuardDefense, new MiscValueHolder(additional));
        }

        protected override void OnUnequip()
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
