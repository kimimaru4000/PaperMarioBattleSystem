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
            
            //Invoke the window size changed event
            WindowSizeChangedEvent?.Invoke(newWindowSize);
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

        public void BeginDrawing(SpriteBatch batch, SamplerState samplerState, Effect effect, Matrix? transformMatrix)
        {
            batch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, samplerState, null, null, effect, transformMatrix);
        }

        public void EndDrawing(SpriteBatch batch)
        {
            batch.End();
        }

        public void Draw(Texture2D texture, Vector2 position, Color color, bool flipX, bool flipY, float layer, bool uibatch = false, bool absOrigin = false)
        {
            Draw(texture, position, null, color, flipX, flipY, layer, uibatch, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, bool flipX, bool flipY, float layer, bool uibatch = false, bool absOrigin = false)
        {
            Draw(texture, position, sourceRect, color, Vector2.Zero, flipX, flipY, layer, uibatch, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, Vector2 origin, bool flipX, bool flipY, float layer, bool uibatch = false, bool absOrigin = false)
        {
            Draw(texture, position, sourceRect, color, 0f, origin, 1f, flipX, flipY, layer, uibatch, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, bool flipX, bool flipY, float layer, bool uibatch = false, bool absOrigin = false)
        {
            Draw(texture, position, sourceRect, color, rotation, origin, new Vector2(scale, scale), flipX, flipY, layer, uibatch, absOrigin);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, bool flipX, bool flipY, float layer, bool uibatch = false, bool absOrigin = false)
        {
            SpriteEffects se = SpriteEffects.None;
            if (flipX == true) se |= SpriteEffects.FlipHorizontally;
            if (flipY == true) se |= SpriteEffects.FlipVertically;

            //NOTE: absOrigin was added as a simple fix to not having absolute origins with sourceRects
            //Come up with something better later

            Vector2 realOrigin = (sourceRect.HasValue && absOrigin == false) ? sourceRect.Value.GetOrigin(origin.X, origin.Y) : origin;
            if (uibatch == false)
                spriteBatch.Draw(texture, position, sourceRect, color, rotation, realOrigin, scale, se, layer);
            else uiBatch.Draw(texture, position, sourceRect, color, rotation, realOrigin, scale, se, layer);
        }

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color, float layer, bool uibatch = true)
        {
            DrawText(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, layer, uibatch);
        }

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer, bool uibatch = true)
        {
            Vector2 realOrigin = spriteFont.GetOrigin(text, origin.X, origin.Y);
            if (uibatch == true)
                uiBatch.DrawString(spriteFont, text, position, color, rotation, realOrigin, scale, SpriteEffects.None, layer);
            else spriteBatch.DrawString(spriteFont, text, position, color, rotation, realOrigin, scale, SpriteEffects.None, layer);
        }
    }
}
