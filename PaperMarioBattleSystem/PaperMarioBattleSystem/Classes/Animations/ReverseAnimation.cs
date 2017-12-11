using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A loopable animation that plays the animation forwards then backwards to complete a loop
    /// </summary>
    public sealed class ReverseAnimation : LoopAnimation
    {
        private bool Reversed = false;

        public ReverseAnimation(Texture2D spriteSheet, int maxLoops, params Frame[] frames) : this(spriteSheet, maxLoops, false, frames)
        {

        }

        public ReverseAnimation(Texture2D spriteSheet, int maxLoops, bool isUIAnim, params Frame[] frames) : this(spriteSheet, maxLoops, 1f, isUIAnim, frames)
        {

        }

        public ReverseAnimation(Texture2D spriteSheet, int maxLoops, float speed, bool isUIAnim, params Frame[] frames) : base(spriteSheet, maxLoops, speed, isUIAnim, frames)
        {
            
        }

        protected override void Progress()
        {
            if (Reversed == false) CurFrameNum++;
            else CurFrameNum--;

            //Done with a loop if we're reversed
            if (Reversed == true && CurFrameNum < 0)
            {
                Loops++;

                //If the animation goes on forever or we're not done, reset back to the second frame
                if (MaxLoops <= AnimationGlobals.InfiniteLoop || Loops < MaxLoops)
                {
                    CurFrameNum = (MaxFrameIndex >= 1) ? 1 : MaxFrameIndex;
                }
                //Otherwise stop on the first frame
                else
                {
                    Loops = MaxLoops;
                    End();
                    CurFrameNum = 0;
                }

                //Mark it as not reversed as we're now playing it forwards again
                Reversed = false;
            }
            //Start playing it in reverse now
            else if (Reversed == false && CurFrameNum >= MaxFrames)
            {
                CurFrameNum = MaxFrameIndex - 1;

                //Mark it as reversed
                Reversed = true;
            }

            ResetFrameDur();
        }

        protected override void DerivedReset()
        {
            base.DerivedReset();

            Reversed = false;
        }
    }
}
