using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that waits for all SequenceActions being performed on the side to complete.
    /// This should be used for the main SequenceAction.
    /// </summary>
    public sealed class WaitSideActionsCompleteSeqAction : SequenceAction
    {
        private Sequence SequenceToCheck = null;

        public WaitSideActionsCompleteSeqAction(Sequence sequenceToCheck)
        {
            SequenceToCheck = sequenceToCheck;
        }

        protected override void OnUpdate()
        {
            //End when there are no more side SequenceActions in effect
            if (SequenceToCheck.SideSeqActionCount == 0)
            {
                End();
            }
        }
    }
}
