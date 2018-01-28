using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// VFX showing an <see cref="IWingedObj"/>'s wings disappearing when it has been grounded.
    /// </summary>
    public sealed class WingsDisappearVFX : VFXElement, ITintable
    {
        /// <summary>
        /// The color to draw the wings.
        /// </summary>
        public Color TintColor { get; set; } = Color.White;

        /// <summary>
        /// The wings to render.
        /// </summary>
        private CroppedTexture2D Wings = null;

        /// <summary>
        /// The position to draw the wings.
        /// </summary>
        private Vector2 DrawPos = Vector2.Zero;

        /// <summary>
        /// Whether to flip the wings or not.
        /// </summary>
        private bool Flip = false;

        /// <summary>
        /// The layer to draw the wings on.
        /// </summary>
        private float Layer = 0f;

        /// <summary>
        /// The initial delay before the wings start flickering.
        /// </summary>
        private double DisappearDelay = 0d;

        /// <summary>
        /// How long the wings flicker.
        /// </summary>
        private double DisappearDur = 0d;

        /// <summary>
        /// The rate at which the wings flicker.
        /// </summary>
        private double VisibleRate = 0d;

        /// <summary>
        /// Whether the wings are past their initial delay and have started flickering.
        /// </summary>
        private bool StartDisappear = false;

        /// <summary>
        /// The elapsed time.
        /// </summary>
        private double ElapsedTime = 0d;

        public WingsDisappearVFX(CroppedTexture2D wings, Vector2 drawPos, bool flip, float layer,
            double disappearDelay, double disappearDur, double visibleRate)
        {
            Wings = wings;
            DrawPos = drawPos;
            Flip = flip;
            Layer = layer;

            DisappearDelay = disappearDelay;
            DisappearDur = disappearDur;
            VisibleRate = visibleRate;

            //If there's no delay to start disappearing, move onto the next phase immediately
            if (DisappearDelay <= 0d)
            {
                StartDisappear = true;
            }
        }

        public override void Update()
        {
            //Don't update if this is done
            if (ReadyForRemoval == true) return;

            //Progress time
            ElapsedTime += Time.ElapsedMilliseconds;

            //Wait out the initial delay
            if (StartDisappear == false)
            {
                if (ElapsedTime >= DisappearDelay)
                {
                    StartDisappear = true;
                    ElapsedTime = 0d;
                }
            }
            else
            {
                if (VisibleRate > 0d)
                {
                    //Multiply the visibility duration by two
                    //If the remainder is greater than the visibility duration, then it's invisible
                    double visibilityTimesTwo = (VisibleRate * 2d);
                    double visibility = ElapsedTime % visibilityTimesTwo;

                    if (visibility <= VisibleRate)
                    {
                        if (TintColor.A < 1f)
                            TintColor = Color.White;
                    }
                    else
                    {
                        if (TintColor.A > 0f)
                            TintColor = Color.Transparent;
                    }
                }

                //End and mark the effect for removal when the duration is up
                if (ElapsedTime >= DisappearDur)
                {
                    ReadyForRemoval = true;
                    ElapsedTime = 0d;
                }
            }
        }

        public override void Draw()
        {
            //Save a draw call if the alpha is 0
            if (TintColor.A > 0f)
                SpriteRenderer.Instance.Draw(Wings.Tex, DrawPos, Wings.SourceRect, TintColor, Flip, false, Layer);
        }
    }
}
