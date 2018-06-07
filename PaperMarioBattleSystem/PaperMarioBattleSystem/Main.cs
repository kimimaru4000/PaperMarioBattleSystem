using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MonoGame.Utilities;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        //Delegate and event for losing window focus
        public delegate void OnLostFocus();

        /// <summary>
        /// The event invoked when the window loses focus. This is invoked at the start of the update loop.
        /// </summary>
        public event OnLostFocus LostFocusEvent = null;

        //Delegate and event for regaining window focus
        public delegate void OnRegainedFocus();

        /// <summary>
        /// The event invoked when the window regains focus. This is invoked at the start of the update loop.
        /// </summary>
        public event OnRegainedFocus RegainedFocusEvent = null;
        
        /// <summary>
        /// Tells if the game window was focused at the start of the update loop.
        /// </summary>
        public bool WasFocused { get; private set; } = false;

        private GraphicsDeviceManager graphics;
        private CrashHandler crashHandler = null;

        /// <summary>
        /// The game window.
        /// </summary>
        public GameWindow GameWindow => Window;

        private List<BattleEntity> BattleEntities = null;
        //A list of Charged BattleEntities for rendering
        private List<BattleEntity> ChargedEntities = null;

        private BattleManager battleManager = null;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            
            crashHandler = new CrashHandler();
            
            //false for variable timestep, true for fixed
            Time.FixedTimeStep = true;
            Time.VSyncEnabled = true;

            //MonoGame sets x32 MSAA by default if enabled
            //If enabled and we want a lower value, set the value in the PreparingDeviceSettings event
            graphics.SynchronizeWithVerticalRetrace = Time.VSyncEnabled;
            graphics.PreferMultiSampling = true;

            //Allows for borderless full screen on DesktopGL
            graphics.HardwareModeSwitch = false;

            graphics.PreparingDeviceSettings -= OnPreparingDeviceSettings;
            graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;

            WasFocused = IsActive;
        }

        private void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            //Prepare any graphics device settings here
            //Note that OpenGL does not provide a way to set the adapter; the driver is responsible for that
            graphics.PreparingDeviceSettings -= OnPreparingDeviceSettings;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            IsFixedTimeStep = Time.FixedTimeStep;

            SpriteRenderer.Instance.Initialize(graphics);
            SpriteRenderer.Instance.AdjustWindowSize(new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight));

            AssetManager.Instance.Initialize(Content);

            //FOR TESTING
            InitializeInventory();

            //Initialize the BattleManager
            battleManager = new BattleManager();
            battleManager.Initialize(new BattleGlobals.BattleProperties(BattleGlobals.BattleSettings.Normal, true),
                new BattleMario(new MarioStats(1, 50, 10, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal)),
                Inventory.Instance.partnerInventory.GetPartner(Enumerations.PartnerTypes.Goombario),
                new List<BattleEntity>() { new KoopaTroopa(), new SkyGuy() });

            BattleUIManager.Instance.battleHUD.SetBattleManager(battleManager);

            //Initialize helper objects
            //Check for the battle setting and add darkness if so
            if (battleManager.Properties.BattleSetting == BattleGlobals.BattleSettings.Dark)
            {
                BattleDarknessObj battleDarkness = new BattleDarknessObj(battleManager);
                LightingManager.Instance.Initialize(battleDarkness);
                BattleObjManager.Instance.AddBattleObject(battleDarkness);
            }

            //Add the HP bar manager
            BattleObjManager.Instance.AddBattleObject(new HPBarManagerObj(battleManager));

            //If you can't run from battle, show a message at the start saying so
            if (battleManager.Properties.Runnable == false)
            {
                battleManager.battleEventManager.QueueBattleEvent((int)BattleGlobals.BattleEventPriorities.Message,
                    new BattleGlobals.BattleState[] { BattleGlobals.BattleState.Turn },
                    new MessageBattleEvent(BattleGlobals.NoRunMessage, MessageBattleEvent.DefaultWaitDuration));
            }

            //Start the battle
            battleManager.StartBattle();

            //Initialize the lists with a capacity equal to the current number of BattleEntities in battle
            BattleEntities = new List<BattleEntity>(battleManager.TotalEntityCount);
            ChargedEntities = new List<BattleEntity>(battleManager.TotalEntityCount);
            
            base.Initialize();

            WasFocused = IsActive;
        }

        /// <summary>
        /// FOR TESTING
        /// </summary>
        private void InitializeInventory()
        {
            Inventory.Instance.partnerInventory.AddPartner(new Goompa());
            Inventory.Instance.partnerInventory.AddPartner(new Goombario());
            Inventory.Instance.partnerInventory.AddPartner(new Kooper());
            Inventory.Instance.partnerInventory.AddPartner(new Bow());
            Inventory.Instance.partnerInventory.AddPartner(new Watt());

            Inventory.Instance.AddItem(new LuckyStar());

            Inventory.Instance.AddBadge(new DefendPlusBadge());
            Inventory.Instance.AddBadge(new PowerPlusBadge());
            Inventory.Instance.AddBadge(new SpikeShieldBadge());
            Inventory.Instance.AddBadge(new FireShieldBadge());
            Inventory.Instance.AddBadge(new IcePowerBadge());
            Inventory.Instance.AddBadge(new AllOrNothingBadge());
            Inventory.Instance.AddBadge(new DoublePainBadge());
            Inventory.Instance.AddBadge(new LastStandBadge());

            Inventory.Instance.AddBadge(new DamageDodgeBadge());
            Inventory.Instance.AddBadge(new DamageDodgeBadge());
            Inventory.Instance.AddBadge(new DamageDodgeBadge());
            Inventory.Instance.AddBadge(new DamageDodgeBadge());

            Inventory.Instance.AddBadge(new PowerBounceBadge());
            Inventory.Instance.AddBadge(new MultibounceBadge());
            Inventory.Instance.AddBadge(new IceSmashBadge());
            Inventory.Instance.AddBadge(new HeadRattleBadge());
            Inventory.Instance.AddBadge(new TornadoJumpBadge());

            Inventory.Instance.AddBadge(new FeelingFineBadge());
            Inventory.Instance.AddBadge(new FeelingFinePBadge());

            Inventory.Instance.AddBadge(new ChargeBadge());
            Inventory.Instance.AddBadge(new ChargePBadge());

            Inventory.Instance.AddBadge(new QuickChangeBadge());

            Inventory.Instance.AddBadge(new FlowerSaverBadge());
            Inventory.Instance.AddBadge(new FlowerSaverPBadge());

            Inventory.Instance.AddBadge(new DoubleDipBadge());
            Inventory.Instance.AddBadge(new DoubleDipBadge());
            Inventory.Instance.AddBadge(new DoubleDipPBadge());
            Inventory.Instance.AddBadge(new DoubleDipPBadge());
            Inventory.Instance.AddBadge(new TripleDipBadge());

            Inventory.Instance.AddBadge(new DeepFocusBadge());
            Inventory.Instance.AddBadge(new DeepFocusBadge());
            Inventory.Instance.AddBadge(new DeepFocusBadge());
            Inventory.Instance.AddBadge(new GroupFocusBadge());

            Inventory.Instance.AddBadge(new CloseCallBadge());
            Inventory.Instance.AddBadge(new PrettyLuckyBadge());
            Inventory.Instance.AddBadge(new PrettyLuckyBadge());
            Inventory.Instance.AddBadge(new PrettyLuckyBadge());
            Inventory.Instance.AddBadge(new LuckyDayBadge());

            Inventory.Instance.AddBadge(new ZapTapBadge());
            Inventory.Instance.AddBadge(new ReturnPostageBadge());

            Inventory.Instance.AddBadge(new HPPlusBadge());
            Inventory.Instance.AddBadge(new HPPlusPBadge());
            Inventory.Instance.AddBadge(new FPPlusBadge());

            Inventory.Instance.AddBadge(new PeekabooBadge());

            Inventory.Instance.AddBadge(new LEmblemBadge());
            Inventory.Instance.AddBadge(new WEmblemBadge());

            Inventory.Instance.AddBadge(new PityFlowerBadge());
            Inventory.Instance.AddBadge(new HPDrainBadge());
            Inventory.Instance.AddBadge(new HPDrainBadge());
            Inventory.Instance.AddBadge(new HPDrainBadge());
            Inventory.Instance.AddBadge(new FPDrainBadge());
            Inventory.Instance.AddBadge(new FPDrainBadge());

            Inventory.Instance.AddBadge(new SimplifierBadge());
            Inventory.Instance.AddBadge(new SimplifierBadge());
            Inventory.Instance.AddBadge(new UnsimplifierBadge());
            Inventory.Instance.AddBadge(new UnsimplifierBadge());

            Inventory.Instance.AddBadge(new DDownPoundBadge());
            Inventory.Instance.AddBadge(new PiercingBlowBadge());
            Inventory.Instance.AddBadge(new DDownJumpBadge());

            Inventory.Instance.AddBadge(new JumpmanBadge());
            Inventory.Instance.AddBadge(new JumpmanBadge());
            Inventory.Instance.AddBadge(new HammermanBadge());
            Inventory.Instance.AddBadge(new HammermanBadge());

            Inventory.Instance.AddBadge(new LuckyStartBadge());

            Inventory.Instance.AddBadge(new PowerSmashBadge());
            Inventory.Instance.AddBadge(new PowerSmashBadge());
            Inventory.Instance.AddBadge(new MegaSmashBadge());
            Inventory.Instance.AddBadge(new QuakeHammerBadge());
            Inventory.Instance.AddBadge(new TimingTutorBadge());

            Inventory.Instance.AddBadge(new AttackFXBBadge(true));
            Inventory.Instance.AddBadge(new AttackFXCBadge());
            Inventory.Instance.AddBadge(new AttackFXDBadge());
            Inventory.Instance.AddBadge(new AttackFXEBadge());
            Inventory.Instance.AddBadge(new AttackFXFBadge());
            Inventory.Instance.AddBadge(new AttackFXBBadge(true));
            Inventory.Instance.AddBadge(new AttackFXCBadge());
            Inventory.Instance.AddBadge(new AttackFXDBadge());
            Inventory.Instance.AddBadge(new AttackFXEBadge());
            Inventory.Instance.AddBadge(new AttackFXFBadge());

            //Debug Badge - Right On!
            Inventory.Instance.AddBadge(new RightOnBadge());
            Inventory.Instance.AddBadge(new RightOnBadge());

            //Items
            Inventory.Instance.AddItem(new Mushroom());
            Inventory.Instance.AddItem(new Mystery());
            Inventory.Instance.AddItem(new SuperShroom());
            Inventory.Instance.AddItem(new ThunderRage());
            Inventory.Instance.AddItem(new ThunderBolt());
            Inventory.Instance.AddItem(new TastyTonic());
            Inventory.Instance.AddItem(new FireFlower());
            Inventory.Instance.AddItem(new VoltShroom());
            Inventory.Instance.AddItem(new DizzyDial());
            Inventory.Instance.AddItem(new StoneCap());
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            graphics.PreparingDeviceSettings -= OnPreparingDeviceSettings;

            if (LightingManager.HasInstance == true)
                LightingManager.Instance.CleanUp();

            AssetManager.Instance.CleanUp();
            SoundManager.Instance.CleanUp();
            SpriteRenderer.Instance.CleanUp();
            BattleUIManager.Instance.CleanUp();
            BattleObjManager.Instance.CleanUp();
            battleManager.CleanUp();
            battleManager = null;

            crashHandler.CleanUp();

            LostFocusEvent = null;
            RegainedFocusEvent = null;
        }

        /// <summary>
        /// Any update logic that should occur immediately before the main Update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PreUpdate(GameTime gameTime)
        {
            //Tell if we change window focus state
            bool focused = IsActive;

            //Lost focus
            if (focused == false && WasFocused == true)
            {
                //Debug.LogError("LOST FOCUS");
                LostFocusEvent?.Invoke();
            }
            //Regained focus
            else if (focused == true && WasFocused == false)
            {
                //Debug.LogError("REGAINED FOCUS");
                RegainedFocusEvent?.Invoke();
            }

            //Set focus state
            WasFocused = focused;

            Time.UpdateTime(gameTime);
            Debug.DebugUpdate(battleManager);
        }

        /// <summary>
        /// The main update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void MainUpdate(GameTime gameTime)
        {
            battleManager.Update();
            BattleUIManager.Instance.Update();
            BattleObjManager.Instance.Update();

            DialogueManager.Instance.Update();

            /*if (Input.GetKeyDown(Keys.Y))
            {
                //string thing = "<dynamic value=\"2\"><wave>Hello</wave> <color value=\"FFFF0000\">World!</color> <key><p>\n<scale value=\"1.3\">This</scale> <speed value=\"500\"><scale value=\".7\">is a </scale><shake>t</shake>est\n<speed value=\"25\">I ho<wave><shake><color value=\"FF0000FF\">pe you</color></shake></wave> enjoy it!\n<k><p>Thank you <color value=\"FFFF0000\">very much</color>\n<wait value=\"800\"><p>for lo<wave>ok</wave>ing!<k></dynamic>";
                //string thing2 = "Hello World! \n e \ntest2\n<p>test";
                
                //Thorough test of Dialogue Bubble features
                string thing3 = "<wave>Hello</wave> <color value=\"FFFF0000\">World!\n</color><key><p><shake>Shaky</shake> text <shake>a</shake>nd <wave><color value=\"FF0000FF\">wa</color>vy</wave> " +
                    "text a<wave>r</wave>e\n<speed value=\"250\"><dynamic value=\"2\">COOL!\n</dynamic><k><p><speed value=\"25\">Let's <wait value=\"500\"><wave><shake><scale value=\"1.6\"><dynamic value=\".3\">" +
                    "<color value=\"ffffbf00\">co</color><color value=\"ff003ce7\">mb</color><color value=\"ff008000\">ine</color> </dynamic></scale>them!</shake></wave>\n<k><p>" +
                    "<speed value=\"0\">Print really fast <key><speed value=\"25\">and now <speed value=\"300\">really\nslow!<speed value=\"25\">\n<k><p>" +
                    "<dynamic value=\"2.5\">Dynamic <scale value=\".8\">text</scale></dynamic> <scale value=\"1.5\"><dynamic value=\"3\">is</dynamic> also<wait value=\"300\"></scale><dynamic value=\"2\"> pretty\n<color value=\"ffff00ff\">awesome!</color></dynamic><k>";

                //string thing4 = "test...\n\n<scale value=\"3\">     KABOOM!</scale><k>";

                DialogueManager.Instance.CreateBubble(thing3, AssetManager.Instance.TTYDFont, null);//new string[] { "Hello World!", "This is a test!", "Not too shabby...\nwhat?\nOh well, let's continue working\non this!", "test more" }, null);
            }*/
        }
        
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            PreUpdate(gameTime);

            //This conditional is for enabling frame advance debugging
            if (Debug.DebugPaused == false || Debug.AdvanceNextFrame == true)
                MainUpdate(gameTime);
            
            PostUpdate(gameTime);
        }

        /// <summary>
        /// Any update logic that should occur immediately after the main Update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PostUpdate(GameTime gameTime)
        {
            SoundManager.Instance.Update();

            //Frame advance debugging for input
            if (Debug.DebugPaused == false || Debug.AdvanceNextFrame == true)
                Input.UpdateInputState();

            //Calculate transformation for use when rendering
            Camera.Instance.CalculateTransformation();

            base.Update(gameTime);

            //Set time step and VSync settings
            IsFixedTimeStep = Time.FixedTimeStep;
            graphics.SynchronizeWithVerticalRetrace = Time.VSyncEnabled;

            //This should always be at the end of PostUpdate()
            if (Time.UpdateFPS == true)
            {
                //Set the FPS - TimeSpan normally rounds, so to be precise we'll create them from ticks
                double val = Math.Round(Time.FPS <= 0d ? 0d : (1d / Time.FPS), 7);
                TargetElapsedTime = TimeSpan.FromTicks((long)(TimeSpan.TicksPerSecond * val));
            }
        }

        /// <summary>
        /// Any drawing that should occur immediately before the main Draw method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PreDraw(GameTime gameTime)
        {
            Time.UpdateFrames();

            //Create a lightmap before drawing the scene, if we should
            //This is for Dark battles
            if (LightingManager.HasInstance == true)
            {
                LightingManager.Instance.CreateLightMap();
            }

            //Set up drawing to the render target
            SpriteRenderer.Instance.SetupDrawing();
        }

        /// <summary>
        /// The main Draw method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void MainDraw(GameTime gameTime)
        {
            //Don't render if the battle didn't start yet
            if (battleManager.State == BattleGlobals.BattleState.Init) return;

            battleManager.GetAllBattleEntities(BattleEntities, null);

            //NOTE: These can be optimized to minimize GPU state change. We might also want to directly use the depth buffer so
            //that depth is preserved among objects rendered with different shaders (Ex. Charged BattleEntities)
            //This is not high priority, since this is just a demonstration of how to use the system

            //Potentially both
            RenderBattleObjects();
            RenderBattleInfo();

            //Sprite
            RenderBattleEntities(BattleEntities);

            //Render the generated lightmap for Dark battles, if it exists
            if (LightingManager.HasInstance == true)
            {
                LightingManager.Instance.RenderLightMap();
            }

            //UI
            RenderStatusInfo(BattleEntities);
            RenderUI();

            //Clear the lists
            BattleEntities.Clear();
            ChargedEntities.Clear();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            PreDraw(gameTime);

            MainDraw(gameTime);

            PostDraw(gameTime);
        }

        /// <summary>
        /// Any drawing that should occur immediately after the main Draw method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PostDraw(GameTime gameTime)
        {
            //Draw debug info
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.uiBatch, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            Debug.DebugDraw();

            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.uiBatch);

            //End drawing and render the RenderTarget's contents to the screen
            SpriteRenderer.Instance.ConcludeDrawing();

            base.Draw(gameTime);
        }

        #region Separated Rendering Methods

        private void RenderBattleEntities(IList<BattleEntity> allEntities)
        {
            //Render all BattleEntities normally
            for (int i = allEntities.Count - 1; i >= 0; i--)
            {
                if (allEntities[i].HasCharge() == true)
                {
                    ChargedEntities.Add(allEntities[i]);

                    continue;
                }

                allEntities[i].Draw();
            }

            //End batch for the set drawn
            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);

            if (UtilityGlobals.IListIsNullOrEmpty(ChargedEntities) == false)
            {
                Effect chargeEffect = AssetManager.Instance.ChargeShader;
                Texture2D chargeShaderTex = AssetManager.Instance.ChargeShaderTex;

                //Render the Charged BattleEntities with the Charge shader
                for (int i = 0; i < ChargedEntities.Count; i++)
                {
                    BattleEntity chargedEntity = ChargedEntities[i];
                    Texture2D spriteSheet = chargedEntity.AnimManager.CurrentAnim.SpriteSheet;

                    //Set effect information
                    Vector2 dimensionRatio = new Vector2(chargeShaderTex.Width, chargeShaderTex.Height) / new Vector2(spriteSheet.Width, spriteSheet.Height);

                    chargeEffect.Parameters["chargeTex"].SetValue(chargeShaderTex);
                    chargeEffect.Parameters["chargeAlpha"].SetValue(RenderingGlobals.ChargeShaderAlphaVal);
                    chargeEffect.Parameters["chargeOffset"].SetValue(new Vector2(0f, RenderingGlobals.ChargeShaderTexOffset));
                    chargeEffect.Parameters["chargeTexRatio"].SetValue(dimensionRatio.Y);
                    chargeEffect.Parameters["objFrameOffset"].SetValue(spriteSheet.GetTexCoordsAt(chargedEntity.AnimManager.GetAnimation<Animation>(chargedEntity.AnimManager.CurrentAnim.Key).CurFrame.DrawRegion));

                    //Render with the shader
                    SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, BlendState.AlphaBlend, null, chargeEffect, Camera.Instance.Transform);

                    chargedEntity.Draw();

                    SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);
                }
            }
        }

        private void RenderBattleInfo()
        {
            //Draw the action the current BattleEntity is performing
            if (battleManager.EntityTurn != null)
            {
                battleManager.EntityTurn.LastAction?.Draw();

                //Show current turn debug text
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Current turn: {battleManager.EntityTurn.Name}", new Vector2(250, 4), Color.White, 0f, Vector2.Zero, 1.3f, .2f);
            }
        }

        private void RenderStatusInfo(IList<BattleEntity> allEntities)
        {
            //Ignore if we shouldn't show this UI
            if (battleManager.ShouldShowPlayerTurnUI() == true && UtilityGlobals.IListIsNullOrEmpty(allEntities) == false)
            {
                List<StatusEffect> statuses = (allEntities.Count == 0) ? null : new List<StatusEffect>();

                for (int i = 0; i < allEntities.Count; i++)
                {
                    BattleEntity entity = allEntities[i];

                    Vector2 statusIconPos = new Vector2(entity.Position.X + 10, entity.Position.Y - 40);
                    entity.EntityProperties.GetStatuses(statuses);
                    int index = 0;

                    for (int j = 0; j < statuses.Count; j++)
                    {
                        StatusEffect status = statuses[j];
                        CroppedTexture2D texture = status.StatusIcon;

                        //Don't draw the status if it doesn't have an icon or if it's Icon suppressed
                        if (texture == null || texture.Tex == null || status.IsSuppressed(Enumerations.StatusSuppressionTypes.Icon) == true)
                        {
                            continue;
                        }

                        float yOffset = ((index + 1) * StatusGlobals.IconYOffset);
                        Vector2 iconPos = Camera.Instance.SpriteToUIPos(new Vector2(statusIconPos.X, statusIconPos.Y - yOffset));

                        float depth = .35f - (index * .01f);
                        float turnStringDepth = depth + .0001f;

                        status.DrawStatusInfo(iconPos, depth, turnStringDepth);

                        index++;
                    }

                    statuses.Clear();
                }
            }
        }

        private void RenderUI()
        {
            BattleUIManager.Instance.Draw();

            DialogueBubble bubble = DialogueManager.Instance.CurDialogueBubble;

            //Draw Dialogue Bubble
            if (bubble != null)
            {
                bubble.Draw();

                SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.uiBatch);

                //We need a new batch to use the bubble's Rasterizer State, which enables the ScissorRectangle
                SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.uiBatch, SpriteSortMode.FrontToBack, BlendState.AlphaBlend,
                    SamplerState.PointClamp, null, bubble.BubbleRasterizerState, null, null);

                //Set the ScissorRectangle to the bubble's bounds
                graphics.GraphicsDevice.ScissorRectangle = new Rectangle((int)bubble.Position.X, (int)bubble.Position.Y, (int)bubble.Scale.X, (int)bubble.Scale.Y);

                bubble.DrawText();
            }

            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.uiBatch);

            //Reset the ScissorRectangle
            graphics.GraphicsDevice.ScissorRectangle = graphics.GraphicsDevice.Viewport.Bounds;
        }

        private void RenderBattleObjects()
        {
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, BlendState.AlphaBlend, null, null, Camera.Instance.Transform);
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.uiBatch, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            BattleObjManager.Instance.Draw();
        }

        #endregion
    }
}
