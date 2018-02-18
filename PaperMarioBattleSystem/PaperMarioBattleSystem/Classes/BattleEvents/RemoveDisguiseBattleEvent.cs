using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Battle Event for removing a Duplighost's disguise.
    /// </summary>
    public sealed class RemoveDisguiseBattleEvent : BattleEvent
    {
        private Duplighost DuplighostRef = null;

        public RemoveDisguiseBattleEvent(Duplighost duplighost)
        {
            DuplighostRef = duplighost;
        }

        protected override void OnUpdate()
        {
            DuplighostRef.RemoveDisguise();

            End();
        }
    }
}
