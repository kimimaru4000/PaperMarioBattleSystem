using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// FPS Counter that displays the FPS
    /// </summary>
    public static class FPSCounter
    {
        /// <summary>
        /// The amount of time, in milliseconds, to wait before updating the FPS display
        /// </summary>
        public static double UpdateInterval = 100d;

        private static double FPSValue = 0d;
        private static double PrevUpdateVal = 0d;

        /// <summary>
        /// The number of frames that have been drawn.
        /// </summary>
        private static int Frames = 0;

        /// <summary>
        /// The amount of elapsed time spent drawing.
        /// </summary>
        private static double FrameTime = 0d;

        public static void Update()
        {
            PrevUpdateVal += Time.ElapsedMilliseconds;

            //Check if we should update the FPS value displayed
            if (PrevUpdateVal >= UpdateInterval)
            {
                PrevUpdateVal = 0d;

                if (UpdateInterval <= 0d)
                {
                    FPSValue = 0d;
                }
                else
                {
                    //Use our target FPS value for fixed time steps
                    if (Time.FixedTimeStep == true)
                    {
                        double diff = FrameTime / UpdateInterval;

                        //Use the target frame rate for fixed time step
                        FPSValue = diff * Time.FPS;
                    }
                    else
                    {
                        double msScale = Time.MsPerS / UpdateInterval;

                        //Calculate the FPS value displayed by checking if we drew the number of times we should have at the current FPS
                        FPSValue = Frames * msScale;
                    }
                }

                Frames = 0;
                FrameTime = 0d;
            }
        }

        public static void Draw()
        {
            string fpsText = FPSValue == 0 ? "Infinity" : FPSValue.ToString();
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, fpsText, Vector2.Zero, Color.White, .5f);
            if (Time.RunningSlowly == true)
            {
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"RUNNING SLOW!", new Vector2(0f, 20f), Color.Red, .5f);
            }

            Frames++;
            FrameTime += Time.ElapsedMilliseconds;
        }
    }
}
