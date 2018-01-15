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
    /// Displays a star and the amount of damage dealt inside. The star scales to its max size over time, then stays onscreen briefly.
    /// <para>If the damage is 0, the star's scale will be very small to indicate no damage.</para>
    /// </summary>
    public class DamageStarVFX : VFXElement
    {
        /// <summary>
        /// The types of StarStates for the damage star.
        /// </summary>
        protected enum StarState
        {
            Expanding, Waiting, Shrinking
        }

        /// <summary>
        /// How long the star stays displayed after it finishes scaling.
        /// </summary>
        protected const double DisplayTime = 800d;

        /// <summary>
        /// The rate the star scales.
        /// </summary>
        protected const float ScaleRate = .18f;

        /// <summary>
        /// The max scale of the star when it grows.
        /// </summary>
        protected Vector2 MaxScale = Vector2.One;

        /// <summary>
        /// The amount of damage the star displays.
        /// </summary>
        protected int DamageDisplayed = 0;

        /// <summary>
        /// The position to render the star.
        /// </summary>
        protected Vector2 Position = Vector2.Zero;

        /// <summary>
        /// The current scale of the star.
        /// </summary>
        protected Vector2 CurScale = Vector2.Zero;

        protected CroppedTexture2D StarHalfLeft = null;
        protected CroppedTexture2D StarHalfRight = null;
        
        /// <summary>
        /// The StarState of the damage star.
        /// </summary>
        protected StarState CurStarState = StarState.Expanding;

        private double ElapsedTime = 0d;

        public DamageStarVFX(int damageDisplayed, Vector2 position)
        {
            DamageDisplayed = UtilityGlobals.Clamp(damageDisplayed, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);
            Position = position;

            if (DamageDisplayed <= 0)
            {
                MaxScale = new Vector2(.3f, .3f);
            }

            StarHalfLeft = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Battle/BattleGFX.png"),
                new Rectangle(305, 809, 31, 58));
            StarHalfRight = StarHalfLeft.Copy();
        }

        public override void Update()
        {
            if (CurStarState == StarState.Expanding)
            {
                CurScale.X = UtilityGlobals.Clamp(CurScale.X + ScaleRate, 0f, MaxScale.X);
                CurScale.Y = UtilityGlobals.Clamp(CurScale.Y + ScaleRate, 0f, MaxScale.Y);

                if (CurScale == MaxScale)
                {
                    CurStarState = StarState.Waiting;
                }
            }
            else if (CurStarState == StarState.Waiting)
            {
                //Increment time and check if it should start shrinking
                ElapsedTime += Time.ElapsedMilliseconds;
                if (ElapsedTime >= DisplayTime)
                {
                    CurStarState = StarState.Shrinking;
                }
            }
            else
            {
                //Shrink the star
                CurScale.X = UtilityGlobals.Clamp(CurScale.X - ScaleRate, 0f, MaxScale.X);
                CurScale.Y = UtilityGlobals.Clamp(CurScale.Y - ScaleRate, 0f, MaxScale.Y);

                if (CurScale == Vector2.Zero)
                {
                    ReadyForRemoval = true;
                }
            }
        }

        public override void Draw()
        {
            int width = StarHalfLeft.SourceRect.Value.Width;
            int height = StarHalfLeft.SourceRect.Value.Height;
            float depth = .35f;

            Vector2 origin = new Vector2(width - 1, height);

            //Merge the pieces into one
            //Flip the right half and make it scale with the left one, positioning it to create a full star
            SpriteRenderer.Instance.Draw(StarHalfLeft.Tex, Position, StarHalfLeft.SourceRect, Color.Yellow, 0f, origin, CurScale, false, false, depth, true);
            SpriteRenderer.Instance.Draw(StarHalfRight.Tex, Position + new Vector2((width - 1) * CurScale.X, 0f), StarHalfRight.SourceRect, Color.Yellow, 0f, origin, CurScale, true, false, depth, true);

            //Show the damage text in the middle of the star
            if (DamageDisplayed > 0)
            {
                string text = DamageDisplayed.ToString();

                //Fade the text based on how much the star is scaled
                float colorAlpha = (MaxScale.X) == 0 ? 0f : (CurScale.X / MaxScale.X);

                //Center the text
                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, text, Position + new Vector2(0f, 0f), Color.Black * colorAlpha, 0f, new Vector2(.5f, .85f), 1f, depth + .01f);
            }
        }
    }
}
