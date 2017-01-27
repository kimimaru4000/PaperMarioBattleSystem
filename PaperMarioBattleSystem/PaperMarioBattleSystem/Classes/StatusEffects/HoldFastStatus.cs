using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Hold Fast Status Effect.
    /// When direct contact is made with the entity afflicted, the attacker receives half the damage dealt in Explosive damage.
    /// <para>This Status Effect is inflicted with Bobbery's Hold Fast move.</para>
    /// </summary>
    public sealed class HoldFastStatus : PaybackStatus
    {
        public HoldFastStatus(int duration) : base(duration,
            new StatusGlobals.PaybackHolder(StatusGlobals.PaybackTypes.Half, Enumerations.Elements.Explosion, null))
        {
            StatusType = Enumerations.StatusTypes.HoldFast;
        }

        public override StatusEffect Copy()
        {
            return new HoldFastStatus(Duration);
        }
    }
}
