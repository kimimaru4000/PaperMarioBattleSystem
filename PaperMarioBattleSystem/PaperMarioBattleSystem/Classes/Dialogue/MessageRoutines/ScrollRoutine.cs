using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Scrolls the Dialogue Bubble's text.
    /// </summary>
    public sealed class ScrollRoutine : MessageRoutine
    {
        private float CurScrollSpeed = DialogueBubble.TextScrollSpeed;

        private float OffsetToScroll = 0f;

        public ScrollRoutine(DialogueBubble bubble, float offsetToScroll) : base(bubble)
        {
            OffsetToScroll = offsetToScroll;
        }

        public override void OnStart()
        {
            if (DBubble.TextYOffset < OffsetToScroll)
            {
                DBubble.CurParagraphIndex--;
                
                //When going back a paragraph, adjust all the previous text to be grey and lose some effects, such as shake and wave
            }
            else if (DBubble.TextYOffset > OffsetToScroll)
            {
                DBubble.CurParagraphIndex++;
            }
        }

        public override void OnEnd()
        {
            
        }

        public override void Update()
        {
            //Speed up scrolling
            if (Input.GetKeyDown(Microsoft.Xna.Framework.Input.Keys.O) == true)
            {
                CurScrollSpeed = DialogueBubble.FastTextScrollSpeed;
            }

            //Scroll text upwards or downwards depending on the new scroll value to go to
            if (DBubble.TextYOffset < OffsetToScroll)
            {
                //Scroll up
                DBubble.TextYOffset -= CurScrollSpeed;
                if (DBubble.TextYOffset > OffsetToScroll)
                {
                    DBubble.TextYOffset = OffsetToScroll;
                    Complete = true;
                }
            }
            else
            {
                //Scroll down
                DBubble.TextYOffset += CurScrollSpeed;
                if (DBubble.TextYOffset < OffsetToScroll)
                {
                    DBubble.TextYOffset = OffsetToScroll;
                    Complete = true;
                }
            }
        }
    }
}
