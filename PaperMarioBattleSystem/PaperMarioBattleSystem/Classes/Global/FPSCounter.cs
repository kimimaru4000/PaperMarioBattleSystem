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
        public static float UpdateInterval = 100f;

        private static double FPSValue = 0d;
        private static float PrevUpdateVal = 0f;

        public static void Update()
        {
            if (Time.TotalMilliseconds >= PrevUpdateVal)
            {
                PrevUpdateVal = (float)Time.TotalMilliseconds + UpdateInterval;

                //Handle division by 0
                if (Time.ElapsedMilliseconds == 0d)
                {
                    FPSValue = 0d;
                }
                else
                {
                    FPSValue = Math.Round(1000d / Time.ElapsedMilliseconds, 2);
                }
            }
        }

        public static void Draw()
        {
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"{FPSValue}", Vector2.Zero, Color.White, .5f);
            if (Time.RunningSlowly == true)
            {
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"RUNNING SLOW!", new Vector2(0f, 20f), Color.Red, .5f);
            }
        }
    }
}
