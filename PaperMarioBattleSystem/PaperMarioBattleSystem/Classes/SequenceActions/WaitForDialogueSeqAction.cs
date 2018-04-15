using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A SequenceAction that waits for dialogue to finish.
    /// </summary>
    public class WaitForDialogueSeqAction : SequenceAction
    {
        private DialogueBubble Bubble = null;

        public WaitForDialogueSeqAction(DialogueBubble bubble)
        {
            Bubble = bubble;
        }

        protected override void OnUpdate()
        {
            //End if the bubble is done
            if (Bubble.IsDone == true)
            {
                End();
            }
        }
    }
}
