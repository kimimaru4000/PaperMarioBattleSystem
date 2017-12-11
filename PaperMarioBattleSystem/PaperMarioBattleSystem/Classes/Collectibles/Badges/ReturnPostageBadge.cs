using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.StatusGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Return Postage Badge - Grants Mario Payback.
    /// <para>This directly grants Payback rather than inflicting the Payback Status. As such, it can stack with the status.</para>
    /// </summary>
    public sealed class ReturnPostageBadge : Badge
    {
        /// <summary>
        /// The Payback granted.
        /// </summary>
        private readonly PaybackHolder PaybackGranted = new PaybackHolder(PaybackTypes.Half, Enumerations.Elements.Normal, 0, null);

        public ReturnPostageBadge()
        {
            Name = "Return Postage";
            Description = "Make direct-attackers take 1/2 the damage they do.";

            BPCost = 7;
            PriceValue = 500;

            BadgeType = BadgeGlobals.BadgeTypes.ReturnPostage;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.EntityProperties.AddPayback(PaybackGranted);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.EntityProperties.RemovePayback(PaybackGranted);
        }
    }
}
