using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Double Pain Badge - The BattleEntity wearing this takes 2x more damage from attacks targeting it.
    /// Stacking multiple of this increases the damage multiplier by 1 (Ex. having 2 equipped results in taking 3x more damage).
    /// <para>This does not affect Payback damage.</para>
    /// </summary>
    public class DoublePainBadge : Badge
    {
        public DoublePainBadge()
        {
            Name = "Double Pain";
            Description = "Double the damage Mario takes.";

            BPCost = 0;

            BadgeType = BadgeGlobals.BadgeTypes.DoublePain;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            //int multiplier = EntityEquipped.EntityProperties.GetMiscProperty(Enumerations.AdditionalProperty.DamageTakenMultiplier).IntValue + 1;
            //EntityEquipped.EntityProperties.AddMiscProperty(Enumerations.AdditionalProperty.DamageTakenMultiplier, new MiscValueHolder(multiplier));
        }

        protected override void OnUnequip()
        {
            //int multiplier = EntityEquipped.EntityProperties.GetMiscProperty(Enumerations.AdditionalProperty.DamageTakenMultiplier).IntValue - 1;
            //EntityEquipped.EntityProperties.RemoveMiscProperty(Enumerations.AdditionalProperty.DamageTakenMultiplier);
            //if (multiplier > 0)
            //{
            //    EntityEquipped.EntityProperties.AddMiscProperty(Enumerations.AdditionalProperty.DamageTakenMultiplier, new MiscValueHolder(multiplier));
            //}
        }
    }
}
