using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public class LastStandBadge : Badge
    {
        public LastStandBadge()
        {
            Name = "Last Stand";
            Description = "Drop damage Mario receives by 1/2 when he is in Danger.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.LastStand;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            //int divider = EntityEquipped.EntityProperties.GetMiscProperty(Enumerations.AdditionalProperty.DangerDamageDivider).IntValue + 1;
            //EntityEquipped.EntityProperties.AddMiscProperty(Enumerations.AdditionalProperty.DangerDamageDivider, new MiscValueHolder(divider));
        }

        protected override void OnUnequip()
        {
            //int divider = EntityEquipped.EntityProperties.GetMiscProperty(Enumerations.AdditionalProperty.DangerDamageDivider).IntValue - 1;
            //EntityEquipped.EntityProperties.RemoveMiscProperty(Enumerations.AdditionalProperty.DangerDamageDivider);
            //if (divider > 0)
            //{
            //    EntityEquipped.EntityProperties.AddMiscProperty(Enumerations.AdditionalProperty.DangerDamageDivider, new MiscValueHolder(divider));
            //}
        }
    }
}
