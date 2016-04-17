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
    /// The base class for sprite animation.
    /// It plays an animation forwards once, stopping on the last frame
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// The sprite sheet containing the sprites to be used in the animation
        /// </summary>
        public Texture2D SpriteSheet = null;
        protected Frame[] Frames = null;

        /// <summary>
        /// The max number of frames in the animation
        /// </summary>
        protected int MaxFrames = 1;
        protected int CurFrameNum = 0;

        /// <summary>
        /// Whether the animation is complete or not. This is a separate value to maximize flexibility
        /// </summary>
        protected bool AnimDone = false;
        protected double PrevFrameTimer { get; private set; } = 0d;

        /// <summary>
        /// Specifies whether the animation should be rendered on the UI layer or not
        /// </summary>
        public bool IsUIAnim = false;

        public bool AnimFinished => AnimDone;
        protected int MaxFrameIndex => MaxFrames - 1;
        protected Frame CurFrame => Frames[CurFrameNum];

        public Animation(Texture2D spriteSheet, params Frame[] frames)
        {
            SpriteSheet = spriteSheet;
            Frames = frames;

            MaxFrames = Frames.Length;
        }

        public Animation (Texture2D spriteSheet, bool isUIAnim, params Frame[] frames) : this(spriteSheet, frames)
        {
            IsUIAnim = isUIAnim;
        }

        /// <summary>
        /// Progresses through the animation
        /// </summary>
        protected virtual void Progress()
        {
            CurFrameNum++;
            if (CurFrameNum >= MaxFrames)
            {
                CurFrameNum = MaxFrameIndex;

                AnimDone = true;
            }

            ResetFrameDur();
        }

        /// <summary>
        /// Resets the animation to the start
        /// </summary>
        public virtual void Reset()
        {
            AnimDone = false;
            CurFrameNum = 0;
            ResetFrameDur();
        }

        /// <summary>
        /// Resets the frame timer to the start of the current frame
        /// </summary>
        protected void ResetFrameDur()
        {
            PrevFrameTimer = Time.ActiveMilliseconds + CurFrame.Duration;
        }

        public void Update()
        {
            if (AnimDone == false)
            {
                //Progress if it's over 
                if (Time.ActiveMilliseconds >= PrevFrameTimer)
                {
                    Progress();
                }
            }
        }

        public void Draw(Vector2 position, Color color, bool flipped, float layer)
        {
            CurFrame.Draw(SpriteSheet, position, color, flipped, layer, IsUIAnim);
        }

        /// <summary>
        /// An animation frame
        /// </summary>
        public sealed class Frame
        {
            /// <summary>
            /// A rectangle designating the region on the sprite sheet to draw
            /// </summary>
            public Rectangle DrawRegion = Rectangle.Empty;

            /// <summary>
            /// How long the frame lasts, in milliseconds
            /// </summary>
            public double Duration = 0f;

            public Frame(Rectangle drawRegion, double duration)
            {
                DrawRegion = drawRegion;
                Duration = duration;
            }

            public void Draw(Texture2D spriteSheet, Vector2 position, Color color, bool flipped, float layer, bool uibatch)
            {
                SpriteRenderer.Instance.Draw(spriteSheet, position, DrawRegion, color, flipped, layer, uibatch);
            }
        }
    }
}
