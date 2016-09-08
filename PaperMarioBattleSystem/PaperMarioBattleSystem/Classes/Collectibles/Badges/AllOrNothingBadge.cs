using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The All or Nothing Badge - If the Action Command is performed successfully, increases damage by 1, otherwise no damage is dealt
    /// </summary>
    public class AllOrNothingBadge : Badge
    {
        public AllOrNothingBadge()
        {
            Name = "All or Nothing";
            Description = "Hit action commands, Attack rises. Fail, it drops to 0.";

            BPCost = 4;

            BadgeType = BadgeGlobals.BadgeTypes.AllOrNothing;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            int allOrNothingCount = EntityEquipped.EntityProperties.GetMiscProperty(Enumerations.MiscProperty.AllOrNothingCount).IntValue + 1;
            EntityEquipped.EntityProperties.RemoveMiscProperty(Enumerations.MiscProperty.AllOrNothingCount);
            EntityEquipped.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.AllOrNothingCount, new MiscValueHolder(allOrNothingCount));
        }

        protected override void OnUnequip()
        {
            int allOrNothingCount = EntityEquipped.EntityProperties.GetMiscProperty(Enumerations.MiscProperty.AllOrNothingCount).IntValue - 1;
            EntityEquipped.EntityProperties.RemoveMiscProperty(Enumerations.MiscProperty.AllOrNothingCount);
            if (allOrNothingCount > 0)
            {
                EntityEquipped.EntityProperties.AddMiscProperty(Enumerations.MiscProperty.AllOrNothingCount, new MiscValueHolder(allOrNothingCount));
            }
        }
    }
}
