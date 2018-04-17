using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Mega Smash Badge - Gives Mario the ability to use Mega Smash.
    /// </summary>
    public sealed class MegaSmashBadge : Badge
    {
        public MegaSmashBadge()
        {
            Name = "Mega Smash";
            Description = "Lets you do a Mega Smash.\nUses 6 FP.\nHammers an enemy with a huge\namount of attack power.";

            BPCost = 3;

            BadgeType = BadgeGlobals.BadgeTypes.MegaSmash;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            
        }

        protected override void OnUnequip()
        {
            
        }
    }
}
