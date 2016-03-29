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
        /// The SpriteBatch used for drawing
        /// </summary>
        private SpriteBatch spriteBatch { get; set; } = null;
        private SpriteBatch uiBatch { get; set; } = null;
        private GraphicsDeviceManager graphicsDeviceManager { get; set; } = null;

        public Vector2 GameWindowSize => new Vector2(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);

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
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, /*null*/Camera.Instance.CalculateTransformation());
            uiBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
        }

        public void EndDrawing()
        {
            spriteBatch.End();
            uiBatch.End();
        }

        public void Draw(Texture2D texture, Vector2 position, Color color, bool flip, float layer)
        {
            Draw(texture, position, null, color, flip, layer);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, bool flip, float layer)
        {
            Draw(texture, position, sourceRect, color, Vector2.Zero, flip, layer);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, Vector2 origin, bool flip, float layer)
        {
            Draw(texture, position, sourceRect, color, 0f, origin, 1f, flip, layer);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color, float rotation, Vector2 origin, float scale, bool flip, float layer)
        {
            SpriteEffects se = flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, position, sourceRect, color, rotation, origin, scale, se, layer);
        }

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color, float layer)
        {
            DrawText(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, layer);
        }

        public void DrawText(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            uiBatch.DrawString(spriteFont, text, position, color, rotation, origin, scale, SpriteEffects.None, layer);
        }
    }
}
