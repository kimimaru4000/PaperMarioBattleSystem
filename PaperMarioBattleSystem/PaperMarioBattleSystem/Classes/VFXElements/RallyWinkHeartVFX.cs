using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The heart that shows up after successfully performing Rally Wink.
    /// It fades in, then fades out after the text box is dismissed.
    /// </summary>
    public sealed class RallyWinkHeartVFX : VFXElement, ITintable, IPosition
    {
        public enum HeartStates
        {
            FadeIn, Paused, FadeOut
        }

        /// <summary>
        /// The start color of the heart.
        /// </summary>
        private readonly Color StartColor = Color.Transparent;

        /// <summary>
        /// The end color of the heart.
        /// </summary>
        private readonly Color EndColor = Color.White * .8f;

        /// <summary>
        /// The time it takes for the heart to fade in.
        /// </summary>
        public double FadeInTime = 500d;

        /// <summary>
        /// The time it takes for the heart to fade out.
        /// </summary>
        public double FadeOutTime = 500d;

        /// <summary>
        /// The position to show the heart.
        /// </summary>
        public Vector2 Position { get; set; } = Vector2.Zero;

        /// <summary>
        /// The current color of the heart.
        /// </summary>
        public Color TintColor { get; set; } = Color.White;

        public HeartStates HeartState { get; private set; } = HeartStates.FadeIn;

        /// <summary>
        /// The texture and region of the texture for the heart.
        /// </summary>
        private CroppedTexture2D HeartTex = null;

        private float Layer = .6f;

        private double ElapsedTime = 0d;

        public RallyWinkHeartVFX(Vector2 position, double fadeInTime, double fadeOutTime, float layer)
        {
            Position = position;
            FadeInTime = fadeInTime;
            FadeOutTime = fadeOutTime;
            Layer = layer;

            HeartTex = new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"),
                new Rectangle(324, 407, 61, 58));
        }

        /// <summary>
        /// Resets the heart, causing it to fade in again.
        /// </summary>
        public void Reset()
        {
            HeartState = HeartStates.FadeIn;
            TintColor = StartColor;
            ElapsedTime = 0d;
        }

        /// <summary>
        /// Starts fading out the heart.
        /// </summary>
        public void FadeOut()
        {
            HeartState = HeartStates.FadeOut;
            TintColor = EndColor;
            ElapsedTime = 0d;
        }

        public override void Update()
        {
            //Don't do anything if it's paused
            if (HeartState == HeartStates.Paused)
                return;

            double timer = FadeInTime;
            Color startColor = StartColor;
            Color endColor = EndColor;

            if (HeartState != HeartStates.FadeIn)
            {
                timer = FadeOutTime;
                startColor = EndColor;
                endColor = StartColor;
            }

            ElapsedTime += Time.ElapsedMilliseconds;

            TintColor = Color.Lerp(startColor, endColor, (float)(ElapsedTime / timer));

            //Check if we're done
            if (ElapsedTime >= timer)
            {
                //If the heart finished fading out, it's done
                if (HeartState == HeartStates.FadeOut)
                {
                    ReadyForRemoval = true;    
                }
                
                TintColor = endColor;

                //Pause the heart so it doesn't update any longer
                //If it was fading in, it will stay paused until told to fade out
                HeartState = HeartStates.Paused;
            }
        }

        public override void Draw()
        {
            SpriteRenderer.Instance.Draw(HeartTex.Tex, Position, HeartTex.SourceRect, TintColor, false, false, Layer);
        }
    }
}
