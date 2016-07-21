using System;
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

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);

            //false for variable timestep, true for fixed
            IsFixedTimeStep = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferMultiSampling = true;
            graphics.PreferredBackBufferWidth = RenderingGlobals.WindowWidth;
            graphics.PreferredBackBufferHeight = RenderingGlobals.WindowHeight;

            //FOR TESTING
            InitializeInventory();

            AssetManager.Instance.Initialize(Content);
            SpriteRenderer.Instance.Initialize(graphics);
            BattleManager.Instance.Initialize(new BattleMario(new Stats(1, 50, 5, 1, 0)), new Goombario(), new List<BattleEnemy>() { new Goomba(), new SpikedGoomba() });

            base.Initialize();
        }

        /// <summary>
        /// FOR TESTING
        /// </summary>
        private void InitializeInventory()
        {
            Inventory.Instance.AddBadge(new PowerPlusBadge());
            Inventory.Instance.AddBadge(new SpikeShieldBadge());
            Inventory.Instance.AddBadge(new FireShieldBadge());
            Inventory.Instance.AddBadge(new IcePowerBadge());
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
            AssetManager.Instance.Dispose();
            SoundManager.Instance.Dispose();
            SpriteRenderer.Instance.Dispose();
            BattleUIManager.Instance.Dispose();
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
            Input.UpdateInputState();
            
            base.Update(gameTime);

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
            GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteRenderer.Instance.BeginDrawing();
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
            SpriteRenderer.Instance.EndDrawing();

            base.Draw(gameTime);
        }
    }
}
