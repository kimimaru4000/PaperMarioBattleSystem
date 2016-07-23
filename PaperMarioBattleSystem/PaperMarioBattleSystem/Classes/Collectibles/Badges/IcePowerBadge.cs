using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Ice Power Badge - Increases Attack and Defense against Fiery entities and Fire attacks by 1 and allows contact with them
    /// <para>This is the TTYD version of this Badge.</para>
    /// </summary>
    public sealed class IcePowerBadge : Badge
    {
        private readonly ResistanceHolder FireResistance = new ResistanceHolder(ResistanceTypes.MinusDamage, 1);
        private const int FireDamageMod = 1;

        public IcePowerBadge()
        {
            Name = "Ice Power";
            Description = "Make Mario damage-proof when jumping on fire enemies. Attack power against fire enemies increases by 1, and damage taken from fire attacks drops by 1.";
            
            BPCost = 1;
            TypeNumber = 59;

            BadgeType = BadgeGlobals.BadgeTypes.IcePower;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.AddContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.AddResistance(Enumerations.Elements.Fire, FireResistance);
            EntityEquipped.AddDamageMod(Enumerations.PhysicalAttributes.Fiery, FireDamageMod);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.RemoveContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.RemoveResistance(Enumerations.Elements.Fire, FireResistance);
            EntityEquipped.RemoveDamageMod(Enumerations.PhysicalAttributes.Fiery, FireDamageMod);
        }
    }
}
