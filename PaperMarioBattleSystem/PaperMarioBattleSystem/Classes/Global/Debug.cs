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

        public static bool DebugPaused { get; private set; } = false;
        public static bool AdvanceNextFrame { get; private set; } = false;

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

            string traceFileName = trace.GetFileName();
            if (string.IsNullOrEmpty(traceFileName) == true)
                traceFileName = "N/A";

            string[] file = traceFileName.Split('\\');
            string fileName = file?[file.Length - 1];
            if (string.IsNullOrEmpty(fileName) == true)
                fileName = "N/A FileName";

            line = trace.GetFileLineNumber();
            method = trace.GetMethod()?.Name;
            if (string.IsNullOrEmpty(method) == true)
                method = "N/A MethodName";

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
                if (Input.GetKey(Keys.LeftControl) && Input.GetKeyDown(Keys.D))
                {
                    ToggleDebug();
                }
            #endif

            //Return if debug isn't enabled
            if (DebugEnabled == false) return;

            //Reset frame advance
            AdvanceNextFrame = false;

            //Debug controls
            if (Input.GetKey(Keys.LeftControl))
            {
                //Toggle pause
                if (Input.GetKeyDown(Keys.P))
                {
                    DebugPaused = !DebugPaused;
                }
                //Toggle frame advance
                else if (Input.GetKeyDown(Keys.OemSemicolon))
                {
                    AdvanceNextFrame = true;
                }
                //Toggle logs
                else if (Input.GetKeyDown(Keys.L))
                {
                    ToggleLogs();
                }
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

            //Battle Debug
            if (Input.GetKey(Keys.RightShift) == true)
            {
                DebugBattle();
            }

            //Unit Tests
            if (Input.GetKey(Keys.U) == true)
            {
                DebugUnitTests();
            }

            FPSCounter.Update();
        }

        public static void DebugBattle()
        {
            //Default to Players - if holding 0, switch to Enemies
            Enumerations.EntityTypes entityType = Enumerations.EntityTypes.Player;
            if (Input.GetKey(Keys.D0) == true) entityType = Enumerations.EntityTypes.Enemy;

            int turnCount = 3;

            //Inflict Poison, Payback, or Paralyzed
            if (Input.GetKeyDown(Keys.P) == true)
            {
                StatusEffect status = new PoisonStatus(turnCount);
                //Inflict Payback
                if (Input.GetKey(Keys.B) == true) status = new PaybackStatus(turnCount);
                //Inflict Paralyzed
                else if (Input.GetKey(Keys.Z) == true) status = new ParalyzedStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Invisible or Immobilized
            else if (Input.GetKeyDown(Keys.I) == true)
            {
                StatusEffect status = new InvisibleStatus(turnCount);
                //Inflict Immobilized
                if (Input.GetKey(Keys.M) == true) status = new ImmobilizedStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Electrified
            else if (Input.GetKeyDown(Keys.E) == true)
            {
                DebugInflictStatus(new ElectrifiedStatus(turnCount), entityType);
            }
            //Inflict Fast, Frozen, or FPRegen
            else if (Input.GetKeyDown(Keys.F) == true)
            {
                StatusEffect status = new FastStatus(turnCount);
                //Inflict Frozen
                if (Input.GetKey(Keys.R) == true) status = new FrozenStatus(turnCount);
                //Inflict FPRegen
                else if (Input.GetKey(Keys.P) == true) status = new FPRegenStatus(2, turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Dizzy or Dodgy
            else if (Input.GetKeyDown(Keys.D) == true)
            {
                StatusEffect status = new DizzyStatus(turnCount);
                //Inflict Dodgy
                if (Input.GetKey(Keys.O) == true) status = new DodgyStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Sleep, Stone, or Slow
            else if (Input.GetKeyDown(Keys.S) == true)
            {
                StatusEffect status = new SleepStatus(turnCount);
                //Inflict Stone
                if (Input.GetKey(Keys.T) == true) status = new StoneStatus(turnCount);
                //Inflict Slow
                else if (Input.GetKey(Keys.L) == true) status = new SlowStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Confused
            else if (Input.GetKeyDown(Keys.C) == true)
            {
                StatusEffect status = new ConfusedStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Burn
            else if (Input.GetKeyDown(Keys.B) == true)
            {
                StatusEffect status = new BurnStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Tiny
            else if (Input.GetKeyDown(Keys.T) == true)
            {
                StatusEffect status = new TinyStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Huge or HPRegen
            else if (Input.GetKeyDown(Keys.H) == true)
            {
                StatusEffect status = new HugeStatus(turnCount);
                //Inflict HPRegen
                if (Input.GetKey(Keys.P) == true) status = new HPRegenStatus(2, turnCount);
                DebugInflictStatus(status, entityType);
            }
            //Inflict Allergic
            else if (Input.GetKeyDown(Keys.A) == true)
            {
                StatusEffect status = new AllergicStatus(turnCount);
                DebugInflictStatus(status, entityType);
            }
        }

        private static void DebugInflictStatus(StatusEffect status, Enumerations.EntityTypes entityType)
        {
            BattleEntity[] entities = BattleManager.Instance.GetEntities(entityType, null);

            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].EntityProperties.AfflictStatus(status);
            }
        }

        #region Debug Unit Tests

        private static void DebugUnitTests()
        {
            if (Input.GetKeyDown(Keys.D1))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT1();
            }
            else if (Input.GetKeyDown(Keys.D2))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT2();
            }
            else if (Input.GetKeyDown(Keys.D3))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT3();
            }
            else if (Input.GetKeyDown(Keys.D4))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT4();
            }
            else if (Input.GetKeyDown(Keys.D5))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT5();
            }
            else if (Input.GetKeyDown(Keys.D6))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT6();
            }
            else if (Input.GetKeyDown(Keys.D7))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT7();
            }
            else if (Input.GetKeyDown(Keys.D8))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT8();
            }
            else if (Input.GetKeyDown(Keys.D9))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT9();
            }
            else if (Input.GetKeyDown(Keys.D0))
            {
                UnitTests.InteractionUnitTests.NewInteractionUT10();
            }
        }

        #endregion

        public static void DebugDraw()
        {
            if (DebugEnabled == false) return;

            //FPS counter
            FPSCounter.Draw();

            //Camera info
            Vector2 cameraBasePos = new Vector2(0, 510);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, "Camera:", cameraBasePos, Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Pos: {Camera.Instance.Position}", cameraBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Rot: {Camera.Instance.Rotation}", cameraBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"Zoom: {Camera.Instance.Scale}", cameraBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
        }
    }
}
