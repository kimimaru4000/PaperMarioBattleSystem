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
        private readonly ResistanceHolder FireResistance = new ResistanceHolder(ResistanceTypes.MinusDamage, 1);

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
            EntityEquipped.EntityProperties.AddContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.EntityProperties.AddResistance(Enumerations.Elements.Fire, FireResistance);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.EntityProperties.RemoveContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.EntityProperties.RemoveResistance(Enumerations.Elements.Fire, FireResistance);
        }
    }
}
