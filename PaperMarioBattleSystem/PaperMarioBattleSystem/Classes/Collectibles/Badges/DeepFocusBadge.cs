using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Deep Focus Badge - Gives Mario more Star Power when using Focus.
    /// </summary>
    public sealed class DeepFocusBadge : Badge
    {
        public DeepFocusBadge()
        {
            Name = "Deep Focus";
            Description = "When using Focus, charges Star Energy more than usual.";

            BPCost = 1;

            BadgeType = BadgeGlobals.BadgeTypes.DeepFocus;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            ///Functionality currently handled in the <see cref="FocusSequence"/>
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
