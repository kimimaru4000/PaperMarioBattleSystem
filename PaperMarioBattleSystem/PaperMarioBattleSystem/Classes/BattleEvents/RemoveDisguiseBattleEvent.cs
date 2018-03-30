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

            IsUnique = true;
        }

        protected override void OnUpdate()
        {
            DuplighostRef.RemoveDisguise();

            End();
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            DuplighostRef = null;
        }

        public override bool AreContentsEqual(BattleEvent other)
        {
            if (base.AreContentsEqual(other) == true) return true;

            //Compare the Duplighost references
            RemoveDisguiseBattleEvent disguiseEvent = other as RemoveDisguiseBattleEvent;

            if (disguiseEvent == null || disguiseEvent.DuplighostRef != DuplighostRef) return false;

            return true;
        }
    }
}
