using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Static class for debugging
    /// </summary>
    public static class Debug
    {
        public static bool DebugEnabled { get; private set; } = false;
        public static bool LogsEnabled { get; private set; } = false;

        static Debug()
        {
            #if DEBUG
                DebugEnabled = true;
                LogsEnabled = true;
            #else
                DebugEnabled = false;
            #endif
        }

        private static void ToggleDebug()
        {
            #if DEBUG
                DebugEnabled = !DebugEnabled;
            //Failsafe
            #else
                DebugEnabled = false;
            #endif
        }

        private static void ToggleLogs()
        {
            LogsEnabled = !LogsEnabled;
        }

        private static string GetStackInfo()
        {
            StackFrame trace = new StackFrame(2, true);
            int line = 0;
            string method = "";

            string[] file = trace.GetFileName().Split('\\');
            string fileName = file?[file.Length - 1];

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;

            return $"{fileName}->{method}({line}):";
        }

        public static void Log(object value)
        {
            if (LogsEnabled == false) return;
            WriteLine($"Information: {GetStackInfo()} {value}");
        }

        public static void LogWarning(object value)
        {
            if (LogsEnabled == false) return;
            WriteLine($"Warning: {GetStackInfo()} {value}");
        }

        public static void LogError(object value)
        {
            if (LogsEnabled == false) return;
            WriteLine($"Error: {GetStackInfo()} {value}");
        }

        public static void DebugUpdate()
        {
            #if DEBUG
                //Toggle debug
                if (Input.GetKey(Keys.LeftControl) && Input.GetKeyDown(Keys.P))
                {
                    ToggleDebug();
                }
            #endif

            //Return if debug isn't enabled
            if (DebugEnabled == false) return;

            //Toggle logs
            if (Input.GetKey(Keys.LeftControl) && Input.GetKeyDown(Keys.L))
            {
                ToggleLogs();
            }

            //Camera controls
            if (Input.GetKey(Keys.LeftShift))
            {
                if (Input.GetKeyDown(Keys.Space))
                {
                    //Reset camera coordinates
                    Camera.Instance.SetTranslation(Vector2.Zero);
                    Camera.Instance.SetRotation(0f);
                    Camera.Instance.SetZoom(1f);
                }
                else
                {
                    Vector2 translation = Vector2.Zero;
                    float rotation = 0f;
                    float zoom = 0f;

                    //Translation
                    if (Input.GetKey(Keys.Left)) translation.X -= 2;
                    if (Input.GetKey(Keys.Right)) translation.X += 2;
                    if (Input.GetKey(Keys.Down)) translation.Y += 2;
                    if (Input.GetKey(Keys.Up)) translation.Y -= 2;

                    //Rotation
                    if (Input.GetKey(Keys.OemComma)) rotation -= .1f;
                    if (Input.GetKey(Keys.OemPeriod)) rotation += .1f;

                    //Scale
                    if (Input.GetKey(Keys.OemMinus)) zoom -= .1f;
                    if (Input.GetKey(Keys.OemPlus)) zoom += .1f;

                    if (translation != Vector2.Zero) Camera.Instance.Translate(translation);
                    if (rotation != 0f) Camera.Instance.Rotate(rotation);
                    if (zoom != 0f) Camera.Instance.Zoom(zoom);
                }
            }

            FPSCounter.Update();
        }

        public static void DebugDraw()
        {
            if (DebugEnabled == false) return;

            //FPS counter
            FPSCounter.Draw();

            //Camera info
            Vector2 cameraBasePos = new Vector2(0, 510);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, "Camera:", cameraBasePos, Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, $"Pos: {Camera.Instance.Position}", cameraBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, $"Rot: {Camera.Instance.Rotation}", cameraBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.Font, $"Zoom: {Camera.Instance.Scale}", cameraBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
        }
    }
}
