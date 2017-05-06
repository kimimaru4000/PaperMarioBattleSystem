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
        private readonly StrengthHolder FireStrength = new StrengthHolder(1);
        private const Enumerations.Elements IceElementOverride = Enumerations.Elements.Ice;

        public IcePowerBadge()
        {
            Name = "Ice Power";
            Description = "Make Mario damage-proof when jumping on fire enemies. Attack power against fire enemies increases by 1, "
                + "and damage taken from fire attacks drops by 1.";
            
            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.IcePower;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.EntityProperties.AddContactException(Enumerations.ContactTypes.TopDirect, Enumerations.PhysicalAttributes.Fiery);
            EntityEquipped.EntityProperties.AddContactException(Enumerations.ContactTypes.SideDirect, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.EntityProperties.AddResistance(Enumerations.Elements.Fire, FireResistance);
            EntityEquipped.EntityProperties.AddElementOverride(Enumerations.PhysicalAttributes.Fiery, IceElementOverride);

            //This is called after actually equipping the badge, so our count is accurate
            int equippedCount = EntityEquipped.GetEquippedBadgeCount(BadgeType);
            
            //If we have more than one equipped, that means we need to add a strength
            if (equippedCount > 1)
            {
                EntityEquipped.EntityProperties.AddStrength(Enumerations.PhysicalAttributes.Fiery, FireStrength);
            }
        }

        protected override void OnUnequip()
        {
            EntityEquipped.EntityProperties.RemoveContactException(Enumerations.ContactTypes.TopDirect, Enumerations.PhysicalAttributes.Fiery);
            EntityEquipped.EntityProperties.RemoveContactException(Enumerations.ContactTypes.SideDirect, Enumerations.PhysicalAttributes.Fiery);

            EntityEquipped.EntityProperties.RemoveResistance(Enumerations.Elements.Fire, FireResistance);
            EntityEquipped.EntityProperties.RemoveElementOverride(Enumerations.PhysicalAttributes.Fiery);

            //This is called before actually unequipping the badge, so the check for one less
            int equippedCount = EntityEquipped.GetEquippedBadgeCount(BadgeType) - 1;
            
            //If we still have at least one remaining, that means we added a strength, so remove it
            if (equippedCount >= 1)
            {
                EntityEquipped.EntityProperties.RemoveStrength(Enumerations.PhysicalAttributes.Fiery, FireStrength);
            }
        }
    }
}
