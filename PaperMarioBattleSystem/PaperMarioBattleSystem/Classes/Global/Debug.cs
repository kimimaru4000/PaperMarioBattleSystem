using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Static class for debugging
    /// </summary>
    public static class Debug
    {
        /// <summary>
        /// The types of Debug logs.
        /// <para>This is a bit field.</para>
        /// </summary>
        [Flags]
        public enum DebugLogTypes
        {
            None = 0,
            Information = 1 << 0,
            Warning = 1 << 1,
            Error = 1 << 2,
            Assert = 1 << 3
        }

        public static bool DebugEnabled { get; private set; } = false;
        public static bool LogsEnabled { get; private set; } = false;

        public static bool DebugPaused { get; private set; } = false;
        public static bool AdvanceNextFrame { get; private set; } = false;

        /// <summary>
        /// The level of logs to output. By default, it logs all types of logs.
        /// </summary>
        public static DebugLogTypes LogLevels = DebugLogTypes.Information | DebugLogTypes.Warning | DebugLogTypes.Error | DebugLogTypes.Assert;

        public static StringBuilder LogDump { get; private set; } = new StringBuilder();

        public static bool DebuggerAttached => Debugger.IsAttached;

        private static KeyboardState DebugKeyboard = default(KeyboardState);

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

        private static string GetStackInfo(int skipFrames)
        {
            StackFrame trace = new StackFrame(skipFrames, true);
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

        private static string GetStackInfo()
        {
            return GetStackInfo(3);
        }

        private static void DebugWriteLine(string value)
        {
            //Write to the log dump
            LogDump.Append(value);
            LogDump.Append("\n");

            WriteLine(value);
        }

        public static void Log(object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Information) == false) return;
            DebugWriteLine($"Information: {GetStackInfo()} {value}");
        }

        public static void LogWarning(object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Warning) == false) return;
            DebugWriteLine($"Warning: {GetStackInfo()} {value}");
        }

        public static void LogError(object value)
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Error) == false) return;
            DebugWriteLine($"Error: {GetStackInfo()} {value}");
        }

        private static void LogAssert()
        {
            if (LogsEnabled == false || DebugLogTypesHasFlag(LogLevels, DebugLogTypes.Assert) == false) return;
            string stackInfo = GetStackInfo(3);
            stackInfo = stackInfo.Remove(stackInfo.Length - 1);

            DebugWriteLine($"ASSERT FAILURE AT: {stackInfo}");
        }

        public static void Assert(bool condition)
        {
            if (condition == false)
                LogAssert();
        }

        public static void DebugUpdate(BattleManager bManager)
        {
            #if DEBUG
                //Toggle debug
                if (Input.GetKey(Keys.LeftControl, DebugKeyboard) && Input.GetKeyDown(Keys.D, DebugKeyboard))
                {
                    ToggleDebug();
                }
            #endif

            //Return if debug isn't enabled
            if (DebugEnabled == false)
            {
                if (Time.InGameTimeEnabled == false)
                    Time.ToggleInGameTime(true);

                //Update the input state if debug is disabled so that the toggle functions properly
                Input.UpdateInputState(ref DebugKeyboard);
                return;
            }

            //Reset frame advance
            AdvanceNextFrame = false;

            //Debug controls
            if (Input.GetKey(Keys.LeftControl, DebugKeyboard))
            {
                //Toggle pause
                if (Input.GetKeyDown(Keys.P, DebugKeyboard))
                {
                    DebugPaused = !DebugPaused;
                }
                //Toggle frame advance
                else if (Input.GetKeyDown(Keys.OemSemicolon, DebugKeyboard))
                {
                    AdvanceNextFrame = true;
                }
                //Toggle logs
                else if (Input.GetKeyDown(Keys.L, DebugKeyboard))
                {
                    ToggleLogs();
                }
                //Take screenshot
                else if (Input.GetKeyDown(Keys.S, DebugKeyboard))
                {
                    TakeScreenshot();
                }
                else if (Input.GetKeyDown(Keys.M, DebugKeyboard))
                {
                    //Log dump
                    DumpLogs();
                }
            }

            //Camera controls
            if (Input.GetKey(Keys.LeftShift, DebugKeyboard))
            {
                if (Input.GetKeyDown(Keys.Space, DebugKeyboard))
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
                    if (Input.GetKey(Keys.Left, DebugKeyboard)) translation.X -= 2;
                    if (Input.GetKey(Keys.Right, DebugKeyboard)) translation.X += 2;
                    if (Input.GetKey(Keys.Down, DebugKeyboard)) translation.Y += 2;
                    if (Input.GetKey(Keys.Up, DebugKeyboard)) translation.Y -= 2;

                    //Rotation
                    if (Input.GetKey(Keys.OemComma, DebugKeyboard)) rotation -= .1f;
                    if (Input.GetKey(Keys.OemPeriod, DebugKeyboard)) rotation += .1f;

                    //Scale
                    if (Input.GetKey(Keys.OemMinus, DebugKeyboard)) zoom -= .1f;
                    if (Input.GetKey(Keys.OemPlus, DebugKeyboard)) zoom += .1f;

                    if (translation != Vector2.Zero) Camera.Instance.Translate(translation);
                    if (rotation != 0f) Camera.Instance.Rotate(rotation);
                    if (zoom != 0f) Camera.Instance.Zoom(zoom);
                }
            }

            //Battle Debug
            if (Input.GetKey(Keys.RightShift, DebugKeyboard) == true)
            {
                DebugBattle(bManager);
            }

            //Unit Tests
            if (Input.GetKey(Keys.U, DebugKeyboard) == true)
            {
                DebugUnitTests();
            }

            if (Input.GetKey(Keys.Tab, DebugKeyboard) == true)
            {
                //Damage Mario
                if (Input.GetKeyDown(Keys.H) == true)
                {
                    //Make sure we damage the Shell instead if it's over Mario
                    BattleEntity entity = bManager.Mario.GetTrueTarget();

                    entity.TakeDamage(Enumerations.Elements.Normal, 1, true);
                }
                //Reload all animations
                //This can break Sequences that rely on animation timings
                else if (Input.GetKeyDown(Keys.R) == true)
                {
                    Debug.Log("Reloading all BattleEntity animations. Things may break if in a Sequence!");

                    List<BattleEntity> entities = new List<BattleEntity>();
                    bManager.GetAllBattleEntities(entities, null);

                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].LoadAnimations();
                    }
                }
                //Reverse flip state of all entities
                else if (Input.GetKeyDown(Keys.F) == true)
                {
                    List<BattleEntity> entities = new List<BattleEntity>();
                    bManager.GetAllBattleEntities(entities, null);

                    for (int i = 0; i < entities.Count; i++)
                    {
                        entities[i].SpriteFlip = !entities[i].SpriteFlip;
                    }
                }
            }

            //If a pause is eventually added that can be performed normally, put a check for it in here to
            //prevent the in-game timer from turning on when it shouldn't
            Time.ToggleInGameTime(DebugPaused == false || AdvanceNextFrame == true);

            FPSCounter.Update();
            Input.UpdateInputState(ref DebugKeyboard);
        }

        public static void DebugBattle(BattleManager bManager)
        {
            //Default to Players - if holding 0, switch to Enemies, and if holding 9, switch to Neutral
            Enumerations.EntityTypes entityType = Enumerations.EntityTypes.Player;
            if (Input.GetKey(Keys.D0, DebugKeyboard) == true) entityType = Enumerations.EntityTypes.Enemy;
            else if (Input.GetKey(Keys.D9, DebugKeyboard) == true) entityType = Enumerations.EntityTypes.Neutral;

            int turnCount = 3;

            StatusEffect status = null;

            //Inflict NoSkills
            if (Input.GetKey(Keys.N, DebugKeyboard) == true)
            {
                //Disable Jump
                if (Input.GetKeyDown(Keys.J, DebugKeyboard) == true)
                    status = new NoSkillsStatus(Enumerations.MoveCategories.Jump, turnCount);
                //Disable Hammer
                else if (Input.GetKeyDown(Keys.H, DebugKeyboard) == true) status = new NoSkillsStatus(Enumerations.MoveCategories.Hammer, turnCount);
                //Disable Items
                else if (Input.GetKeyDown(Keys.I, DebugKeyboard) == true) status = new NoSkillsStatus(Enumerations.MoveCategories.Item, turnCount);
                //Disable Tactics
                else if (Input.GetKeyDown(Keys.T, DebugKeyboard) == true) status = new NoSkillsStatus(Enumerations.MoveCategories.Tactics, turnCount);
                //Disable Partner moves
                else if (Input.GetKeyDown(Keys.P, DebugKeyboard) == true) status = new NoSkillsStatus(Enumerations.MoveCategories.Partner, turnCount);
                //Disable Special moves
                else if (Input.GetKeyDown(Keys.S, DebugKeyboard) == true) status = new NoSkillsStatus(Enumerations.MoveCategories.Special, turnCount);

                if (status != null) DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Poison, Payback, or Paralyzed
            else if (Input.GetKeyDown(Keys.P, DebugKeyboard) == true)
            {
                //Inflict Payback
                if (Input.GetKey(Keys.B, DebugKeyboard) == true) status = new PaybackStatus(turnCount);
                //Inflict Paralyzed
                else if (Input.GetKey(Keys.Z, DebugKeyboard) == true) status = new ParalyzedStatus(turnCount);
                else status = new PoisonStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Invisible or Injured
            else if (Input.GetKeyDown(Keys.I, DebugKeyboard) == true)
            {
                //Inflict Injured
                if (Input.GetKey(Keys.J, DebugKeyboard) == true) status = new InjuredStatus(turnCount);
                else status = new InvisibleStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Electrified
            else if (Input.GetKeyDown(Keys.E, DebugKeyboard) == true)
            {
                DebugInflictStatus(bManager, new ElectrifiedStatus(turnCount), entityType);
            }
            //Inflict Fast, Frozen, FPRegen, or Fright
            else if (Input.GetKeyDown(Keys.F, DebugKeyboard) == true)
            {
                //Inflict Frozen
                if (Input.GetKey(Keys.R, DebugKeyboard) == true) status = new FrozenStatus(turnCount);
                //Inflict FPRegen
                else if (Input.GetKey(Keys.P, DebugKeyboard) == true) status = new FPRegenStatus(2, turnCount);
                else if (Input.GetKey(Keys.I, DebugKeyboard) == true) status = new FrightStatus();
                else status = new FastStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Dizzy or Dodgy
            else if (Input.GetKeyDown(Keys.D, DebugKeyboard) == true)
            {
                //Inflict Dodgy
                if (Input.GetKey(Keys.O, DebugKeyboard) == true) status = new DodgyStatus(turnCount);
                else status = new DizzyStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Sleep, Stone, Slow, or Stop
            else if (Input.GetKeyDown(Keys.S, DebugKeyboard) == true)
            {
                //Inflict Stone
                if (Input.GetKey(Keys.T, DebugKeyboard) == true) status = new StoneStatus(turnCount);
                //Inflict Slow
                else if (Input.GetKey(Keys.L, DebugKeyboard) == true) status = new SlowStatus(turnCount);
                //Inflict Stop
                else if (Input.GetKey(Keys.P, DebugKeyboard) == true) status = new StopStatus(turnCount);
                else status = new SleepStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Confused or Charged
            else if (Input.GetKeyDown(Keys.C, DebugKeyboard) == true)
            {
                //Inflict Charged
                if (Input.GetKey(Keys.H, DebugKeyboard) == true) status = new ChargedStatus(1);
                else status = new ConfusedStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Burn or Blown
            else if (Input.GetKeyDown(Keys.B, DebugKeyboard) == true)
            {
                if (Input.GetKey(Keys.L, DebugKeyboard) == true) status = new BlownStatus();
                else status = new BurnStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Tiny
            else if (Input.GetKeyDown(Keys.T, DebugKeyboard) == true)
            {
                status = new TinyStatus(turnCount);
                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Huge or HPRegen
            else if (Input.GetKeyDown(Keys.H, DebugKeyboard) == true)
            {
                //Inflict HPRegen
                if (Input.GetKey(Keys.P, DebugKeyboard) == true) status = new HPRegenStatus(2, turnCount);
                //Inflict Hold Fast
                else if (Input.GetKey(Keys.O, DebugKeyboard) == true) status = new HoldFastStatus(turnCount);
                else status = new HugeStatus(turnCount);

                DebugInflictStatus(bManager, status, entityType);
            }
            //Inflict Allergic
            else if (Input.GetKeyDown(Keys.A, DebugKeyboard) == true)
            {
                status = new AllergicStatus(turnCount);
                DebugInflictStatus(bManager, status, entityType);
            }
        }

        private static void DebugInflictStatus(BattleManager bManager, StatusEffect status, Enumerations.EntityTypes entityType)
        {
            BattleEntity[] entities = bManager.GetEntities(entityType, null);
            
            for (int i = 0; i < entities.Length; i++)
            {
                entities[i].EntityProperties.AfflictStatus(status);
            }
        }

        /// <summary>
        /// Takes a screenshot of the screen.
        /// </summary>
        public static void TakeScreenshot()
        {
            //Wrap the Texture2D in the using so it's guaranteed to get disposed
            using (Texture2D screenshotTex = GetScreenshot())
            {
                //Open the file dialogue so you can name the file and place it wherever you want
                System.Windows.Forms.SaveFileDialog dialogue = new System.Windows.Forms.SaveFileDialog();
                dialogue.FileName = string.Empty;
                dialogue.Filter = "PNG (*.png)|*.png";

                if (dialogue.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (FileStream fstream = new FileStream(dialogue.FileName, FileMode.Create))
                    {
                        Vector2 size = SpriteRenderer.Instance.WindowSize;
                        screenshotTex.SaveAsPng(fstream, (int)size.X, (int)size.Y);
                    }
                }
            }
        }

        /// <summary>
        /// Gets what is currently rendered on the backbuffer and returns it in a Texture2D.
        /// <para>IMPORTANT: Dispose the Texture2D when you're done with it.</para>
        /// </summary>
        /// <returns>A Texture2D of what's currently rendered on the screen.</returns>
        private static Texture2D GetScreenshot()
        {
            GraphicsDevice graphicsDevice = SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice;

            int width = graphicsDevice.PresentationParameters.BackBufferWidth;
            int height = graphicsDevice.PresentationParameters.BackBufferHeight;

            //Present what's drawn
            graphicsDevice.Present();

            //Fill an array with the back buffer data that's the same size as the screen
            int[] backbuffer = new int[width * height];
            graphicsDevice.GetBackBufferData(backbuffer);

            //Create a new Texture2D and set the data
            Texture2D screenshot = new Texture2D(graphicsDevice, width, height, false, graphicsDevice.PresentationParameters.BackBufferFormat);
            screenshot.SetData(backbuffer);

            return screenshot;
        }

        public static void DumpLogs()
        {
            string initFileName = "Log Dump " + DebugGlobals.GetFileFriendlyTimeStamp();

            //Open the file dialogue so you can name the file and place it wherever you want
            System.Windows.Forms.SaveFileDialog dialogue = new System.Windows.Forms.SaveFileDialog();
            dialogue.FileName = initFileName;
            dialogue.Filter = "TXT (*.txt)|*.txt";

            if (dialogue.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Dump the logs to the file
                using (StreamWriter writer = File.CreateText(dialogue.FileName))
                {
                    writer.Write($"Log Dump:\n{Debug.LogDump.ToString()}");

                    writer.Flush();
                }
            }
        }

        /// <summary>
        /// Tells whether a set of DebugLogTypes has any of the flags in another DebugLogTypes set.
        /// </summary>
        /// <param name="debugLogTypes">The DebugLogTypes value.</param>
        /// <param name="debugLogTypesFlags">The flags to test.</param>
        /// <returns>true if any of the flags in debugLogTypes are in debugLogTypesFlags, otherwise false.</returns>
        public static bool DebugLogTypesHasFlag(Debug.DebugLogTypes debugLogTypes, Debug.DebugLogTypes debugLogTypesFlags)
        {
            Debug.DebugLogTypes flags = (debugLogTypes & debugLogTypesFlags);

            return (flags != 0);
        }

        #region Debug Unit Tests

        private static void DebugUnitTests()
        {
            if (Input.GetKeyDown(Keys.D0, DebugKeyboard))
            {
                UnitTests.InteractionUnitTests.ElementOverrideInteractionUT1();
            }
            else if (Input.GetKeyDown(Keys.D1, DebugKeyboard))
            {
                UnitTests.InteractionUnitTests.PaybackInteractionUT1();
            }
            else if (Input.GetKeyDown(Keys.D2, DebugKeyboard))
            {
                UnitTests.InteractionUnitTests.PaybackInteractionUT2();
            }
            else if (Input.GetKeyDown(Keys.D3, DebugKeyboard))
            {
                UnitTests.InteractionUnitTests.PaybackInteractionUT3();
            }
            //Status unit tests
            else if (Input.GetKeyDown(Keys.D4, DebugKeyboard))
            {
                UnitTests.RunStatusUnitTests();
            }
            //Badge unit tests
            else if (Input.GetKeyDown(Keys.D5, DebugKeyboard))
            {
                UnitTests.RunBadgeUnitTests();
            }
        }

        #endregion

        #region Debug Drawing Methods

        /// <summary>
        /// Draws a line.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        /// <param name="color">The color of the line.</param>
        /// <param name="layer">The layer of the line.</param>
        /// <param name="thickness">The thickness of the line.</param>
        /// <param name="uiBatch">Whether to draw the line in the UI layer or not.</param>
        public static void DebugDrawLine(Vector2 start, Vector2 end, Color color, float layer, int thickness, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");

            //Get rotation with the angle between the start and end vectors
            float lineRotation = (float)UtilityGlobals.TangentAngle(start, end);

            //Get the scale; use the X as the length and the Y as the width
            Vector2 diff = end - start;
            Vector2 lineScale = new Vector2(diff.Length(), thickness);
            
            if (uiBatch == false)
                SpriteRenderer.Instance.Draw(box, start, null, color, lineRotation, new Vector2(0f, 0f), lineScale, false, false, layer);
            else
                SpriteRenderer.Instance.DrawUI(box, start, null, color, lineRotation, new Vector2(0f, 0f), lineScale, false, false, layer);
        }

        /// <summary>
        /// Draws a rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the rectangle.</param>
        /// <param name="layer">The layer of the rectangle.</param>
        /// <param name="uiBatch">Whether to draw the rectangle in the UI layer or not.</param>
        public static void DebugDrawRect(Rectangle rect, Color color, float layer, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");

            if (uiBatch == false)
                SpriteRenderer.Instance.Draw(box, new Vector2(rect.X, rect.Y), null, color, 0f, Vector2.Zero, new Vector2(rect.Width, rect.Height), false, false, layer);
            else
                SpriteRenderer.Instance.DrawUI(box, new Vector2(rect.X, rect.Y), null, color, 0f, Vector2.Zero, new Vector2(rect.Width, rect.Height), false, false, layer);
        }

        /// <summary>
        /// Draws a hollow rectangle.
        /// </summary>
        /// <param name="rect">The Rectangle to draw.</param>
        /// <param name="color">The color of the hollow rectangle.</param>
        /// <param name="layer">The layer of the hollow rectangle.</param>
        /// <param name="thickness">The thickness of the hollow rectangle.</param>
        /// <param name="uiBatch">Whether to draw the hollow rectangle in the UI layer or not.</param>
        public static void DebugDrawHollowRect(Rectangle rect, Color color, float layer, int thickness, bool uiBatch)
        {
            if (DebugEnabled == false) return;

            Rectangle[] rects = new Rectangle[4]
            {
                new Rectangle(rect.X, rect.Y, rect.Width, thickness),
                new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
                new Rectangle(rect.X, rect.Y, thickness, rect.Height),
                new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness)
            };

            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");

            for (int i = 0; i < rects.Length; i++)
            {
                if (uiBatch == false)
                    SpriteRenderer.Instance.Draw(box, new Vector2(rects[i].X, rects[i].Y), null, color, 0f, Vector2.Zero, new Vector2(rects[i].Width, rects[i].Height), false, false, layer);
                else
                    SpriteRenderer.Instance.DrawUI(box, new Vector2(rects[i].X, rects[i].Y), null, color, 0f, Vector2.Zero, new Vector2(rects[i].Width, rects[i].Height), false, false, layer);
            }
        }

        /// <summary>
        /// Draws a circle.
        /// </summary>
        /// <param name="circle">The circle to draw.</param>
        /// <param name="color">The color of the circle.</param>
        /// <param name="layer">The layer of the circle.</param>
        /// <param name="uiBatch">Whether to draw the circle in the UI layer or not.</param>
        /// <remarks>Brute force algorithm obtained from here: https://stackoverflow.com/a/1237519 
        /// This seems to gives a more full looking circle than Bresenham's algorithm.
        /// </remarks>
        public static void DebugDrawCircle(Circle circle, Color color, float layer, bool uiBatch)
        {
            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
            float radius = (float)circle.Radius;
            Vector2 origin = circle.Center;
            float radiusSquared = radius * radius;
            float radiusSquaredPlusRadius = radiusSquared + radius;

            for (float y = -radius; y <= radius; y++)
            {
                for (float x = -radius; x <= radius; x++)
                {
                    float xSquared = x * x;
                    float ySquared = y * y;

                    if ((xSquared + ySquared) < radiusSquaredPlusRadius)
                    {
                        if (uiBatch == false)
                            SpriteRenderer.Instance.Draw(box, new Vector2(origin.X + x, origin.Y + y), color, false, false, layer);
                        else
                            SpriteRenderer.Instance.DrawUI(box, new Vector2(origin.X + x, origin.Y + y), color, false, false, layer);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a hollow circle.
        /// </summary>
        /// <param name="circle">The hollow circle to draw.</param>
        /// <param name="color">The color of the hollow circle.</param>
        /// <param name="layer">The layer of the hollow circle.</param>
        /// <param name="uiBatch">Whether to draw the hollow circle in the UI layer or not.</param>
        /// <remarks>Brute force algorithm obtained from here: https://stackoverflow.com/a/1237519 
        /// This seems to gives a more full looking circle than Bresenham's algorithm.
        /// </remarks>
        public static void DebugDrawHollowCircle(Circle circle, Color color, float layer, bool uiBatch)
        {
            Texture2D box = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.UIRoot}/Box.png");
            float radius = (float)circle.Radius;
            Vector2 origin = circle.Center;
            float radiusSquared = radius * radius;
            float radiusSqMinusRadius = radiusSquared - radius;
            float radiusSqPlusRadius = radiusSquared + radius;

            for (float y = -radius; y <= radius; y++)
            {
                for (float x = -radius; x <= radius; x++)
                {
                    float xSquared = x * x;
                    float ySquared = y * y;

                    if ((xSquared + ySquared) > radiusSqMinusRadius && (xSquared + ySquared) < radiusSqPlusRadius)
                    {
                        if (uiBatch == false)
                            SpriteRenderer.Instance.Draw(box, new Vector2(origin.X + x, origin.Y + y), color, false, false, layer);
                        else
                            SpriteRenderer.Instance.DrawUI(box, new Vector2(origin.X + x, origin.Y + y), color, false, false, layer);
                    }
                }
            }
        }

        #endregion

        public static void DebugDraw()
        {
            if (DebugEnabled == false) return;

            //FPS counter
            FPSCounter.Draw();

            //Memory usage
            Vector2 memBasePos = new Vector2(250, 570);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Managed Mem: {Math.Round(GC.GetTotalMemory(false) / 1024f / 1024f, 2)} MB", memBasePos, Color.White, 0f, Vector2.Zero, 1f, .1f);

            //Camera info
            Vector2 cameraBasePos = new Vector2(0, 510);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, "Camera:", cameraBasePos, Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Pos: {Camera.Instance.Position}", cameraBasePos + new Vector2(0, 20), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Rot: {Camera.Instance.Rotation}", cameraBasePos + new Vector2(0, 40), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
            SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Zoom: {Camera.Instance.Scale}", cameraBasePos + new Vector2(0, 60), Color.White, 0f, Vector2.Zero, 1.2f, .1f);
        }

        #region Classes

        /// <summary>
        /// Global values regarding debugging.
        /// </summary>
        public static class DebugGlobals
        {
            /// <summary>
            /// Gets the path for crash log files.
            /// </summary>
            /// <returns>A string with the full name of the crash log file.</returns>
            public static string GetCrashLogPath()
            {
                string time = GetFileFriendlyTimeStamp();

                string path = $"{System.IO.Directory.GetCurrentDirectory()}\\PMBattleSystem Crash Log - {time}.txt";

                return path;
            }

            /// <summary>
            /// Returns a file friendly time stamp of the current time.
            /// </summary>
            /// <returns>A string representing current time.</returns>
            public static string GetFileFriendlyTimeStamp()
            {
                string time = DateTime.Now.ToUniversalTime().ToString();
                time = time.Replace(':', '-');
                time = time.Replace('/', '-');

                return time;
            }

            /// <summary>
            /// Gets the name of the assembly.
            /// </summary>
            /// <returns>A string representing the name of the assembly.</returns>
            public static string GetAssemblyName()
            {
                //Get the name from the assembly information
                System.Reflection.Assembly assembly = typeof(Debug).Assembly;
                System.Reflection.AssemblyName asm = assembly.GetName();

                return asm.Name;
            }

            /// <summary>
            /// Gets the full build number as a string.
            /// </summary>
            /// <returns>A string representing the full build number.</returns>
            public static string GetBuildNumber()
            {
                //Get the build number from the assembly information
                System.Reflection.Assembly assembly = typeof(Debug).Assembly;
                System.Reflection.AssemblyName asm = assembly.GetName();

                return asm.Version.Major + "." + asm.Version.Minor + "." + asm.Version.Build + "." + asm.Version.Revision;
            }

            /// <summary>
            /// Gets the name, version, and word size of the operating system as a string.
            /// </summary>
            /// <returns>A string representing the OS name, version, and word size.</returns>
            public static string GetOSInfo()
            {
                string osVersion = string.Empty;

                //Getting the OS version can fail if the user is running an extremely uncommon or old OS and/or it can't retrieve the information
                try
                {
                    osVersion = Environment.OSVersion.ToString();

                    //Check for a Linux OS and get more detailed info
                    if (osVersion.ToLower().StartsWith("unix") == true)
                    {
                        string detailedLinux = GetDetailedLinuxOSInfo();

                        //If we got the info, set the OS string to the detailed version
                        if (string.IsNullOrEmpty(detailedLinux) == false)
                        {
                            osVersion = detailedLinux;
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    if (string.IsNullOrEmpty(osVersion) == true)
                        osVersion = "N/A";
                }

                //Get word size
                string osBit = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";

                return $"{osVersion} {osBit}";
            }

            /// <summary>
            /// Retrieves more detailed information about a Linux OS by reading from its lsb-release file.
            /// </summary>
            /// <returns>A string representing the Linux ID and Release number. If the file isn't found or accessible, then null.</returns>
            private static string GetDetailedLinuxOSInfo()
            {
                //Try to find the OS info in the "/etc/lsb-release" file
                //"/" is the root
                const string lsbRelease = "/etc/lsb-release";

                //Check if the file exists and we have permission to access it
                if (File.Exists(lsbRelease) == true)
                {
                    try
                    {
                        //Get all the text in the file
                        string releaseText = File.ReadAllText(lsbRelease);

                        //The DISTRIB_DESCRIPTION is the only thing we need, as it includes both the ID and Release number
                        const string findString = "DISTRIB_DESCRIPTION=\"";

                        //Find the location of the description in the file
                        int index = releaseText.IndexOf(findString) + findString.Length;

                        //The OS description will be here
                        //The description is in quotation marks, so exclude the last character
                        string osDescription = releaseText.Substring(index, releaseText.Length - index - 2);
                        return osDescription;
                    }
                    catch (Exception)
                    {
                        //If we ran into an error, there's nothing we can really do, so exit
                    }
                }

                return null;
            }
        }

        #endregion
    }
}
