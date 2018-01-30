using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles all rendering
    /// <para>This is a Singleton</para>
    /// </summary>
    public class SpriteRenderer : ICleanup
    {
        #region Singleton Fields

        public static SpriteRenderer Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SpriteRenderer();
                }

                return instance;
            }
        }

        private static SpriteRenderer instance = null;

        #endregion

        public delegate void OnWindowSizeChanged(Vector2 newWindowSize);
        /// <summary>
        /// An event invoked when the window size has changed.
        /// </summary>
        public event OnWindowSizeChanged WindowSizeChangedEvent = null;

        /// <summary>
        /// The SpriteBatch used for drawing non-UI elements
        /// </summary>
        public SpriteBatch spriteBatch { get; private set; } = null;

        /// <summary>
        /// The SpriteBatch used for drawing UI elements. UI is always drawn on top of non-UI elements
        /// </summary>
        public SpriteBatch uiBatch { get; private set; } = null;
        public GraphicsDeviceManager graphicsDeviceManager { get; private set; } = null;

        public Vector2 WindowSize => new Vector2(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);
        public Vector2 WindowCenter => WindowSize.HalveInt();

        private bool Initialized = false;

        /// <summary>
        /// The main RenderTarget used to render the screen to.
        /// </summary>
        private RenderTarget2D MainRenderTarget = null;

        /// <summary>
        /// Whether the RenderTarget was set or not.
        /// If true, no screen size changes will be applied until the next frame.
        /// </summary>
        private bool SetRenderTarget = false;

        private SpriteRenderer()
        {
            
        }

        public void CleanUp()
        {
            spriteBatch.Dispose();
            uiBatch.Dispose();
            graphicsDeviceManager.Dispose();

            Initialized = false;

            WindowSizeChangedEvent = null;

            instance = null;
        }

        /// <summary>
        /// Initializes the SpriteRenderer
        /// </summary>
        /// <param name="graphics">The GraphicsDeviceManager used for rendering</param>
        public void Initialize(GraphicsDeviceManager graphics)
        {
            if (Initialized == true) return;
            
            graphicsDeviceManager = graphics;
            spriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
            uiBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);

            MainRenderTarget = new RenderTarget2D(graphicsDeviceManager.GraphicsDevice, graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);

            Initialized = true;
        }

        /// <summary>
        /// Adjusts the size of the game window
        /// </summary>
        /// <param name="newWindowSize">The new size of the window</param>
        public void AdjustWindowSize(Vector2 newWindowSize)
        {
            graphicsDeviceManager.PreferredBackBufferWidth = (int)newWindowSize.X;
            graphicsDeviceManager.PreferredBackBufferHeight = (int)newWindowSize.Y;

            graphicsDeviceManager.ApplyChanges();
            
            //Replace the main RenderTarget if we can
            if (SetRenderTarget == false)
            {
                ReplaceMainRenderTarget(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);
            }

            //Invoke the window size changed event
            WindowSizeChangedEvent?.Invoke(newWindowSize);
        }

        /// <summary>
        /// Replaces the main RenderTarget with a new one of a different size.
        /// Don't call this until the RenderTarget is cleared from the GraphicsDevice.
        /// </summary>
        /// <param name="newWidth">The new width of the RenderTarget.</param>
        /// <param name="newHeight">The new height of the RenderTarget.</param>
        private void ReplaceMainRenderTarget(int newWidth, int newHeight)
        {
            MainRenderTarget?.Dispose();
            MainRenderTarget = null;

            MainRenderTarget = new RenderTarget2D(graphicsDeviceManager.GraphicsDevice, newWidth, newHeight);
        }

        /* Idea for refactoring rendering to easily support shaders:
         * 1. Store all info in new struct (spritebatch, texture, position, sourcerect, color, etc.)
         * 2. Add to a dictionary which has the shader as the key (null is a valid key) and a list of structs as the value
         * 3. Handle all rendering at once with the data in the dictionary
         * 4. Clear dictionary after finishing rendering
         * 
         * The idea is to batch together everything that is rendered with a particular shader.
         * 
         * The main concerns are:
         *  -If one object wants to change the values of a shader for itself, the new values will be applied to all using that shader
         *  -Potentially too much memory usage (will need to be tested)
         *  -It's unsure how shaders will even be used yet
         */

        /// <summary>
        /// Sets up initial drawing to the main RenderTarget.
        /// </summary>
        public void SetupDrawing()
        {
            if (SetRenderTarget == true)
            {
                Debug.LogError($"Cannot set up drawing since it hasn't concluded!");
                return;
            }

            Vector2 windowSize = WindowSize;
            int width = (int)windowSize.X;
            int height = (int)windowSize.Y;

            //If our main RenderTarget isn't large enough, change its size
            if (MainRenderTarget.Width != width || MainRenderTarget.Height != height)
            {
                ReplaceMainRenderTarget(width, height);
            }

            //Set the RenderTarget to the graphics device
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(MainRenderTarget);

            //Clear the contents
            graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            //Mark that we set the RenderTarget
            SetRenderTarget = true;
        }

        /// <summary>
        /// Ends drawing for the frame, rendering the contents of the main RenderTarget to the screen.
        /// </summary>
        public void ConcludeDrawing()
        {
            if (SetRenderTarget == false)
            {
                Debug.LogError($"Cannot conclude drawing since it hasn't started!");
                return;
            }

            //Clear RenderTarget
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);

            //Clear screen with color
            graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            //Start a new batch just to draw the RenderTarget
            BeginBatch(spriteBatch, null, null, null, null);

            //Draw the render target
            Draw(MainRenderTarget, new Rectangle(0, 0, MainRenderTarget.Width, MainRenderTarget.Height), null, Color.White, 0f, Vector2.Zero, false, false, 1f);

            //End the batch
            EndBatch(spriteBatch);

            //Mark that we unset the render target
            SetRenderTarget = false;
        }

        public void BeginBatch(SpriteBatch batch, BlendState blendState, SamplerState samplerState, Effect effect, Matrix? transformMatrix)
        {
            batch.Begin(SpriteSortMode.FrontToBack, blendState, samplerState, null, null, effect, transformMatrix);
        }

        public void EndBatch(SpriteBatch batch)
        {
            batch.End();
        }

        #region Generalized Draw Methods

        public void DrawBatch(SpriteBatch batch, Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            SpriteEffects se = SpriteEffects.None;
            if (flipX == true) se |= SpriteEffects.FlipHorizontally;
            if (flipY == true) se |= SpriteEffects.FlipVertically;

            //NOTE: absOrigin was added as a simple fix to not having absolute origins with sourceRects
            //Come up with something better later

            Vector2 realOrigin = (sourceRect.HasValue && absOrigin == false) ? sourceRect.Value.GetOrigin(origin.X, origin.Y) : origin;

            batch.Draw(texture, position, sourceRect, color, rotation, realOrigin, scale, se, layer);
        }

        public void DrawBatch(SpriteBatch batch, Texture2D texture, Rectangle destinationRect, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            SpriteEffects se = SpriteEffects.None;
            if (flipX == true) se |= SpriteEffects.FlipHorizontally;
            if (flipY == true) se |= SpriteEffects.FlipVertically;

            Vector2 realOrigin = (sourceRect.HasValue && absOrigin == false) ? sourceRect.Value.GetOrigin(origin.X, origin.Y) : origin;

            batch.Draw(texture, destinationRect, sourceRect, color, rotation, realOrigin, se, layer);
        }

        public void DrawBatchText(SpriteBatch batch, SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            Vector2 realOrigin = spriteFont.GetOrigin(text, origin.X, origin.Y);
            batch.DrawString(spriteFont, text, position, color, rotation, realOrigin, scale, SpriteEffects.None, layer);
        }

        #endregion

        #region Sprite Draw Methods

        public void Draw(Texture2D texture, Vector2 position, Color color, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            Draw(texture, position, null, color, flipX, flipY, layer, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            Draw(texture, position, sourceRect, color, Vector2.Zero, flipX, flipY, layer, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, Vector2 origin, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            Draw(texture, position, sourceRect, color, 0f, origin, 1f, flipX, flipY, layer, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            Draw(texture, position, sourceRect, color, rotation, origin, new Vector2(scale, scale), flipX, flipY, layer, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawBatch(spriteBatch, texture, position, sourceRect, color, rotation, origin, scale, flipX, flipY, layer, absOrigin);
        }

        public void Draw(Texture2D texture, Rectangle destinationRect, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawBatch(spriteBatch, texture, destinationRect, sourceRect, color, rotation, origin, flipX, flipY, layer, absOrigin);
        }

        #endregion

        #region UI Draw Methods

        public void DrawUI(Texture2D texture, Vector2 position, Color color, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawUI(texture, position, null, color, flipX, flipY, layer, absOrigin);
        }

        public void DrawUI(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawUI(texture, position, sourceRect, color, Vector2.Zero, flipX, flipY, layer, absOrigin);
        }

        public void DrawUI(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, Vector2 origin, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawUI(texture, position, sourceRect, color, 0f, origin, 1f, flipX, flipY, layer, absOrigin);
        }

        public void DrawUI(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawUI(texture, position, sourceRect, color, rotation, origin, new Vector2(scale, scale), flipX, flipY, layer, absOrigin);
        }

        public void DrawUI(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawBatch(uiBatch, texture, position, sourceRect, color, rotation, origin, scale, flipX, flipY, layer, absOrigin);
        }

        public void DrawUI(Texture2D texture, Rectangle destinationRect, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, bool flipX, bool flipY, float layer, bool absOrigin = false)
        {
            DrawBatch(uiBatch, texture, destinationRect, sourceRect, color, rotation, origin, flipX, flipY, layer, absOrigin);
        }

        #endregion

        #region Text Draw Methods

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color, float layer)
        {
            DrawText(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, layer);
        }

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            DrawBatchText(spriteBatch, spriteFont, text, position, color, rotation, origin, scale, layer);
        }

        public void DrawUIText(SpriteFont spriteFont, string text, Vector2 position, Color color, float layer)
        {
            DrawUIText(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, layer);
        }

        public void DrawUIText(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            DrawBatchText(uiBatch, spriteFont, text, position, color, rotation, origin, scale, layer);
        }

        #endregion
    }
}
