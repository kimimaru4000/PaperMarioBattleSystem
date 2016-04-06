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
    public class SpriteRenderer : IDisposable
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

        /// <summary>
        /// The SpriteBatch used for drawing non-UI elements
        /// </summary>
        private SpriteBatch spriteBatch { get; set; } = null;

        /// <summary>
        /// The SpriteBatch used for drawing UI elements. UI is always drawn on top of non-UI elements
        /// </summary>
        private SpriteBatch uiBatch { get; set; } = null;
        private GraphicsDeviceManager graphicsDeviceManager { get; set; } = null;

        public Vector2 WindowSize => new Vector2(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);
        public Vector2 WindowCenter => WindowSize.HalveInt();

        private SpriteRenderer()
        {
            
        }

        public void Dispose()
        {
            spriteBatch.Dispose();
            uiBatch.Dispose();
            graphicsDeviceManager.Dispose();

            instance = null;
        }

        /// <summary>
        /// Initializes the SpriteRenderer
        /// </summary>
        /// <param name="graphics">The GraphicsDeviceManager used for rendering</param>
        public void Initialize(GraphicsDeviceManager graphics)
        {
            graphicsDeviceManager = graphics;
            spriteBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
            uiBatch = new SpriteBatch(graphicsDeviceManager.GraphicsDevice);
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
        }

        public void BeginDrawing()
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, null, null, null, null, /*null*/Camera.Instance.CalculateTransformation());
            uiBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend);
        }

        public void EndDrawing()
        {
            spriteBatch.End();
            uiBatch.End();
        }

        public void Draw(Texture2D texture, Vector2 position, Color color, bool flip, float layer, bool uibatch = false)
        {
            Draw(texture, position, null, color, flip, layer, uibatch);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, bool flip, float layer, bool uibatch = false)
        {
            Draw(texture, position, sourceRect, color, Vector2.Zero, flip, layer, uibatch);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, Vector2 origin, bool flip, float layer, bool uibatch = false)
        {
            Draw(texture, position, sourceRect, color, 0f, origin, 1f, flip, layer, uibatch);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, bool flip, float layer, bool uibatch = false)
        {
            Draw(texture, position, sourceRect, color, rotation, origin, new Vector2(scale, scale), flip, layer, uibatch);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, Vector2 scale, bool flip, float layer, bool uibatch = false)
        {
            SpriteEffects se = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 realOrigin = sourceRect.HasValue ? sourceRect.Value.GetOrigin(origin.X, origin.Y) : origin;
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
