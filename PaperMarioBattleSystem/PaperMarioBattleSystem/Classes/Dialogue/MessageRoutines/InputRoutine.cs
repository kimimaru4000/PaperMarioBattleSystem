using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A MessageRoutine that waits for input.
    /// </summary>
    public sealed class InputRoutine : MessageRoutine
    {
        private Keys PreviousButton;
        private Keys ProgressButton;

        public InputRoutine(DialogueBubble bubble, Keys previousButton, Keys progressButton) : base(bubble)
        {
            PreviousButton = previousButton;
            ProgressButton = progressButton;
        }

        public override void OnStart()
        {
            DBubble.ProgressTextStar.Disabled = false;
            DBubble.SpeakerEndTalk();
        }

        public override void OnEnd()
        {
            DBubble.ProgressTextStar.Disabled = true;
        }

        public override void Update()
        {
            //If you can go back, check for input
            if (DBubble.CurParagraphIndex > 0)
            {
                //Go back
                if (Input.GetKeyDown(DialogueBubble.PreviousParagraphButton) == true)
                {
                    OnEnd();
                    HasStarted = false;

                    //Add scroll routine for going down to this offset
                    DBubble.AddMessageRoutine(new ScrollRoutine(DBubble, DBubble.TextYOffset));

                    //Add an input routine
                    DBubble.AddMessageRoutine(new InputRoutine(DBubble, PreviousButton, ProgressButton));

                    //Add scroll routine for going up
                    DBubble.AddMessageRoutine(new ScrollRoutine(DBubble, DBubble.TextYOffset + DBubble.YMoveAmount));

                    return;
                }
            }

            if (Input.GetKeyDown(ProgressButton) == true)
            {
                Complete = true;
            }
        }
    }
}
