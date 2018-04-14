using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame;
using MonoGame.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Main : Game
    {
        private GraphicsDeviceManager graphics;
        private CrashHandler crashHandler = null;

        /// <summary>
        /// The game window.
        /// </summary>
        public GameWindow GameWindow => Window;

        private List<BattleEntity> BattleEntities = null;
        //A list of Charged BattleEntities for rendering
        private List<BattleEntity> ChargedEntities = null;

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
            BattleManager.Instance.Initialize(new BattleGlobals.BattleProperties(BattleGlobals.BattleSettings.Normal, true),
                new BattleMario(new MarioStats(1, 50, 10, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal)),
                Inventory.Instance.partnerInventory.GetPartner(Enumerations.PartnerTypes.Goombario),
                new List<BattleEntity>() { new Duplighost() });

            //Start the battle
            BattleManager.Instance.StartBattle();

            //Initialize the lists with a capacity equal to the current number of BattleEntities in battle
            BattleEntities = new List<BattleEntity>(BattleManager.Instance.TotalEntityCount);
            ChargedEntities = new List<BattleEntity>(BattleManager.Instance.TotalEntityCount);
            
            base.Initialize();
        }

        /// <summary>
        /// FOR TESTING
        /// </summary>
        private void InitializeInventory()
        {
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
            Inventory.Instance.AddItem(new HoneySyrup());
            Inventory.Instance.AddItem(new Mushroom());
            Inventory.Instance.AddItem(new ShootingStar());
            Inventory.Instance.AddItem(new SleepySheep());
            Inventory.Instance.AddItem(new LifeShroom());
            Inventory.Instance.AddItem(new TastyTonic());
            Inventory.Instance.AddItem(new VoltShroom());
            Inventory.Instance.AddItem(new Mystery());
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

            AssetManager.Instance.CleanUp();
            SoundManager.Instance.CleanUp();
            SpriteRenderer.Instance.CleanUp();
            BattleUIManager.Instance.CleanUp();
            BattleObjManager.Instance.CleanUp();
            BattleManager.Instance.CleanUp();

            crashHandler.CleanUp();
        }

        /// <summary>
        /// Any update logic that should occur immediately before the main Update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void PreUpdate(GameTime gameTime)
        {
            Time.UpdateTime(gameTime);
            Debug.DebugUpdate();
        }

        /// <summary>
        /// The main update loop
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void MainUpdate(GameTime gameTime)
        {
            BattleManager.Instance.Update();
            BattleUIManager.Instance.Update();
            BattleObjManager.Instance.Update();

            if (bubble != null)
            {
                bubble.Update();
                if (bubble.IsDone == true)
                {
                    bubble.CleanUp();
                    bubble = null;
                }
            }

            if (bubble == null && Input.GetKeyDown(Keys.Y))
            {
                bubble = new DialogueBubble(new string[] { "Hello World!", "This is a test!", "Not too shabby...\nwhat?\nOh well, let's continue working\non this!" }, 34d);
            }
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
            if (BattleManager.Instance.State == BattleManager.BattleState.Init) return;

            BattleManager.Instance.GetAllBattleEntities(BattleEntities, null);

            //Potentially both
            RenderBattleObjects();
            RenderBattleInfo();

            //Sprite
            RenderBattleEntities(BattleEntities);

            //UI
            RenderStatusInfo(BattleEntities);
            RenderUI();

            //Clear the lists
            BattleEntities.Clear();
            ChargedEntities.Clear();

            //BattleManager.Instance.Draw();
            //BattleUIManager.Instance.Draw();
            //BattleObjManager.Instance.Draw();
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
        DialogueBubble bubble = null;
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
                    chargeEffect.Parameters["objFrameOffset"].SetValue(spriteSheet.GetTexCoordsAt(chargedEntity.AnimManager.CurrentAnim.CurFrame.DrawRegion));

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
            if (BattleManager.Instance.EntityTurn != null)
            {
                BattleManager.Instance.EntityTurn.PreviousAction?.Draw();

                //Show current turn debug text
                SpriteRenderer.Instance.DrawUIText(AssetManager.Instance.TTYDFont, $"Current turn: {BattleManager.Instance.EntityTurn.Name}", new Vector2(250, 10), Color.White, 0f, Vector2.Zero, 1.3f, .2f);
            }
        }

        private void RenderStatusInfo(IList<BattleEntity> allEntities)
        {
            //Ignore if we shouldn't show this UI
            if (BattleManager.Instance.ShouldShowPlayerTurnUI == true && UtilityGlobals.IListIsNullOrEmpty(allEntities) == false)
            {
                for (int i = 0; i < allEntities.Count; i++)
                {
                    BattleEntity entity = allEntities[i];

                    Vector2 statusIconPos = new Vector2(entity.Position.X + 10, entity.Position.Y - 40);
                    StatusEffect[] statuses = entity.EntityProperties.GetStatuses();
                    int index = 0;

                    for (int j = 0; j < statuses.Length; j++)
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
                }
            }
        }

        private void RenderUI()
        {
            BattleUIManager.Instance.Draw();

            //Draw Test Dialogue Bubble
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
