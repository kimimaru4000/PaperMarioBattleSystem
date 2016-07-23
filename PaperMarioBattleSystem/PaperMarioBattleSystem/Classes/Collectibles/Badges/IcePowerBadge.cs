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

        public IcePowerBadge()
        {
            Name = "Ice Power";
            Description = "Make Mario damage-proof when jumping on fire enemies. Attack power against fire enemies increases by 1, and damage taken from fire attacks drops by 1.";
            
            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.IcePower;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.AddContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.AddResistance(Enumerations.Elements.Fire, FireResistance);

            //NOTE: Add Strengths for dealing more damage to entities with specific PhysicalAttributes, as Ice Power
            //does NOT make your moves Ice but instead just deals more damage to Fiery entities
        }

        protected override void OnUnequip()
        {
            EntityEquipped.RemoveContactException(Enumerations.ContactTypes.JumpContact, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.RemoveResistance(Enumerations.Elements.Fire, FireResistance);
        }
    }
}
