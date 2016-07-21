using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Fire Shield Badge - Allows contact with Fiery entities and decreases received Fire damage by 1
    /// </summary>
    public sealed class FireShieldBadge : Badge
    {
        public FireShieldBadge()
        {
            Name = "Fire Shield";
            Description = "Makes the damage Mario takes from fire enemies go down by 1. Mario won't take damage when he jumps on a fire enemy.";

            BPCost = 2;

            BadgeType = BadgeGlobals.BadgeTypes.FireShield;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.AddContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            //NOTE: Handle adding and removing Resistances/Weaknesses later as they're currently not supported outside BattleEntity
        }

        protected override void OnUnequip()
        {
            EntityEquipped.RemoveContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);
        }
    }
}
