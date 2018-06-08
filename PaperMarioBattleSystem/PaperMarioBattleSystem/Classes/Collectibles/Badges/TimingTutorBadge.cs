using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Timing Tutor Badge - teaches the timing of Stylish Moves by showing a "!" above the character's head when they should press A.
    /// </summary>
    public sealed class TimingTutorBadge : Badge
    {
        public TimingTutorBadge()
        {
            Name = "Timing Tutor";
            Description = "Learn the timing for Stylish commands.";

            BPCost = 1;
            PriceValue = 120;

            BadgeType = BadgeGlobals.BadgeTypes.TimingTutor;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            //Flag that this BattleEntity should have Stylish Move timings shown
            EntityEquipped.AddIntAdditionalProperty(Enumerations.AdditionalProperty.ShowStylishTimings, 1);
        }

        protected override void OnUnequip()
        {
            //Remove the flag that Stylish Move timings should be shown
            EntityEquipped.SubtractIntAdditionalProperty(Enumerations.AdditionalProperty.ShowStylishTimings, 1);
        }
    }
}
