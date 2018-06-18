using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// FPS Counter that displays the FPS.
    /// </summary>
    public static class FPSCounter
    {
        /// <summary>
        /// The amount of time, in milliseconds, to wait before updating the FPS display.
        /// </summary>
        private static double UpdateInterval = 1000d;

        /// <summary>
        /// The current frame rate.
        /// </summary>
        private static double FPSValue = 0d;
        private static double PrevUpdateVal = 0d;

        /// <summary>
        /// The number of updates that have been performed.
        /// </summary>
        private static int Updates = 0;

        /// <summary>
        /// The number of frames that have been drawn.
        /// </summary>
        private static int Frames = 0;

        public static void Update()
        {
            PrevUpdateVal += Time.ElapsedMilliseconds;

            //Check if we should update the FPS value displayed
            if (PrevUpdateVal >= UpdateInterval)
            {
                if (UpdateInterval <= 0d)
                {
                    FPSValue = 0d;
                }
                else
                {
                    //Our FPS is how many frames passed in the update interval
                    FPSValue = (Frames / UpdateInterval) * Time.MsPerS;
                }

                Updates = 0;
                Frames = 0;
                PrevUpdateVal = 0d;
            }

            //Count each update
            Updates++;
        }

        public static void Draw()
        {
            string fpsText = FPSValue == 0 ? "Infinity" : FPSValue.ToString();
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, fpsText, Vector2.Zero, Color.White, .5f);
            if (Time.RunningSlowly == true)
            {
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"RUNNING SLOW!", new Vector2(0f, 20f), Color.Red, .5f);
            }

            //Count each frame
            Frames++;
        }
    }
}
