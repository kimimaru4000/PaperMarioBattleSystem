using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Turbo Charge Status Effect.
    /// The entity's Attack is raised by 1.
    /// <para>This Status Effect is inflicted with Watt's Turbo Charge move.</para>
    /// </summary>
    public sealed class TurboChargeStatus : POWUpStatus
    {
        /// <summary>
        /// The amount Turbo Charge increases the entity's Attack by.
        /// </summary>
        private const int DamageBoost = 1;

        public TurboChargeStatus(int duration) : base(DamageBoost, duration)
        {
            StatusType = Enumerations.StatusTypes.TurboCharge;

            AfflictedMessage = "Your attack power will go up for a short time!";
            RemovedMessage = "Your attack power has returned to normal!";
        }

        public override StatusEffect Copy()
        {
            return new TurboChargeStatus(Duration);
        }
    }
}
