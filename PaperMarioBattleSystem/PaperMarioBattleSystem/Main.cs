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

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            
            crashHandler = new CrashHandler();

            //false for variable timestep, true for fixed
            Time.FixedTimeStep = true;
            Time.VSyncEnabled = true;
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
            graphics.SynchronizeWithVerticalRetrace = Time.VSyncEnabled;

            graphics.PreferMultiSampling = true;
            
            AssetManager.Instance.Initialize(Content);
            SpriteRenderer.Instance.Initialize(graphics);

            SpriteRenderer.Instance.AdjustWindowSize(new Vector2(RenderingGlobals.WindowWidth, RenderingGlobals.WindowHeight));

            //FOR TESTING
            InitializeInventory();

            BattleManager.Instance.Initialize(new BattleGlobals.BattleProperties(BattleGlobals.BattleSettings.Normal),
                new BattleMario(new MarioStats(1, 50, 10, 0, 0, EquipmentGlobals.BootLevels.Normal, EquipmentGlobals.HammerLevels.Normal)),
                Inventory.Instance.partnerInventory.GetPartner(Enumerations.PartnerTypes.Goombario),
                new List<BattleEntity>() { new Duplighost() });

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
            
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, BlendState.AlphaBlend, null, null, Camera.Instance.CalculateTransformation());
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.uiBatch, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            Debug.DebugDraw();
        }

        /// <summary>
        /// The main Draw method
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values</param>
        private void MainDraw(GameTime gameTime)
        {
            BattleManager.Instance.Draw();
            BattleUIManager.Instance.Draw();
            BattleObjManager.Instance.Draw();
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
            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);
            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.uiBatch);

            //End drawing and render the RenderTarget's contents to the screen
            SpriteRenderer.Instance.ConcludeDrawing();

            base.Draw(gameTime);
        }
    }
}
