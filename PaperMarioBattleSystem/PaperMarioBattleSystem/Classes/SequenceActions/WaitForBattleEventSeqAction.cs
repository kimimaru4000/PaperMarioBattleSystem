using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that waits for a BattleEvent to complete.
    /// </summary>
    public class WaitForBattleEventSeqAction : SequenceAction
    {
        private BattleEvent BEvent = null;

        public WaitForBattleEventSeqAction(BattleEvent battleEvent)
        {
            BEvent = battleEvent;
        }

        protected override void OnUpdate()
        {
            //If the BattleEvent doesn't exist, end immediately
            if (BEvent == null)
            {
                End();
                return;
            }

            if (BEvent.IsDone == true)
            {
                End();
            }
        }
    }
}
