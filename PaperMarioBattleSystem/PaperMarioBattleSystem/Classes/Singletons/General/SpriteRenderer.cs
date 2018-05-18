using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Utilities;

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

        public delegate void ConcludedDrawing(in RenderTarget2D finalRenderTarget);
        /// <summary>
        /// An event invoked when drawing has concluded for the frame and the final RenderTarget is ready.
        /// <para>This is invoked right after obtaining the final RenderTarget but right before it is drawn to the backbuffer.</para>
        /// </summary>
        public event ConcludedDrawing ConcludedDrawingEvent = null;

        public delegate void FullyConcludedDrawing(in RenderTarget2D finalRenderTarget);
        /// <summary>
        /// An event invoked when drawing has fully concluded for the frame and the final RenderTarget has been drawn to the backbuffer.
        /// <para>Anything that needs to draw at this point must draw to the backbuffer.</para>
        /// </summary>
        public event FullyConcludedDrawing FullyConcludedDrawingEvent = null;

        /// <summary>
        /// The SpriteBatch used for drawing non-UI elements
        /// </summary>
        public SpriteBatch spriteBatch { get; private set; } = null;

        /// <summary>
        /// The SpriteBatch used for drawing UI elements. UI is always drawn on top of non-UI elements
        /// </summary>
        public SpriteBatch uiBatch { get; private set; } = null;
        public GraphicsDeviceManager graphicsDeviceManager { get; private set; } = null;

        public Vector2 WindowSize => new Vector2(graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferWidth, graphicsDeviceManager.GraphicsDevice.PresentationParameters.BackBufferHeight);
        public Vector2 WindowCenter => WindowSize.HalveInt();
        public Vector2 ResolutionScale => WindowSize / new Vector2(RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight);

        public bool IsFullscreen => graphicsDeviceManager.IsFullScreen;

        private bool Initialized = false;

        /// <summary>
        /// The number of post-processing shaders in effect.
        /// </summary>
        public int PostProcessingCount => PostProcessingEffects.Count;

        /// <summary>
        /// A list of post-processing shaders to apply. They'll be applied in order.
        /// </summary>
        private readonly List<Effect> PostProcessingEffects = new List<Effect>();

        /// <summary>
        /// The RenderTarget containing the final output.
        /// </summary>
        public RenderTarget2D FinalRenderTarget { get; private set; } = null;

        /// <summary>
        /// The main RenderTarget used to render the screen to.
        /// </summary>
        private RenderTarget2D MainRenderTarget = null;

        /// <summary>
        /// The alternative RenderTarget used to aid in post-processing effects.
        /// </summary>
        private RenderTarget2D PPRenderTarget = null;

        /// <summary>
        /// Whether drawing for the frame started or not.
        /// </summary>
        public bool StartedDrawing { get; private set; } = false;

        private SpriteRenderer()
        {
            
        }

        public void CleanUp()
        {
            spriteBatch.Dispose();
            uiBatch.Dispose();
            graphicsDeviceManager.Dispose();

            RemoveAllPostProcessingEffects();

            Initialized = false;

            WindowSizeChangedEvent = null;
            ConcludedDrawingEvent = null;
            FullyConcludedDrawingEvent = null;

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

            //Set the RenderTargets to the base resolution
            //From there, we can upscale or downscale it
            int width = RenderingGlobals.BaseResolutionWidth;
            int height = RenderingGlobals.BaseResolutionHeight;

            MainRenderTarget = new RenderTarget2D(graphicsDeviceManager.GraphicsDevice, width, height);
            PPRenderTarget = new RenderTarget2D(graphicsDeviceManager.GraphicsDevice, width, height);

            FinalRenderTarget = MainRenderTarget;

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

        /// <summary>
        /// Sets the game to be windowed or full screen.
        /// </summary>
        /// <param name="fullScreen">Whether to set full screen or not.</param>
        public void SetFullscreen(bool fullScreen)
        {
            graphicsDeviceManager.IsFullScreen = fullScreen;
            graphicsDeviceManager.ApplyChanges();
        }

        /// <summary>
        /// Sets up initial drawing to the main RenderTarget.
        /// </summary>
        public void SetupDrawing()
        {
            if (StartedDrawing == true)
            {
                Debug.LogError($"Cannot set up drawing since it hasn't concluded!");
                return;
            }

            //Set the RenderTarget to the graphics device
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(MainRenderTarget);

            //Clear the contents
            graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            FinalRenderTarget = MainRenderTarget;

            //Mark that we started drawing
            StartedDrawing = true;
        }

        /// <summary>
        /// Ends drawing for the frame, rendering the contents of the main RenderTarget to the screen.
        /// </summary>
        public void ConcludeDrawing()
        {
            if (StartedDrawing == false)
            {
                Debug.LogError($"Cannot conclude drawing since it hasn't started!");
                return;
            }

            //Handle rendering multiple post-processing effects with two RenderTargets
            RenderTarget2D renderToTarget = PPRenderTarget;
            RenderTarget2D renderTarget = MainRenderTarget;

            //Render all post-processing effects
            for (int i = 0; i < PostProcessingCount; i++)
            {
                graphicsDeviceManager.GraphicsDevice.SetRenderTarget(renderToTarget);
                graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

                spriteBatch.Begin(SpriteSortMode.Texture, null, null, null, null, PostProcessingEffects[i], null);

                spriteBatch.Draw(renderTarget, new Rectangle(0, 0, renderTarget.Width, renderTarget.Height), null, Color.White);

                spriteBatch.End();

                //Swap RenderTargets; the one that was just rendered to has the updated data
                UtilityGlobals.Swap(ref renderToTarget, ref renderTarget);
            }

            //Set the final render target to the one with the updated data
            FinalRenderTarget = renderTarget;

            //Invoke the event saying we finished drawing and pass in the final RenderTarget
            //Do this before drawing to the backbuffer so other RenderTargets can do as they please without clearing it
            ConcludedDrawingEvent?.Invoke(FinalRenderTarget);

            //Perform one final draw to the backbuffer
            graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);
            graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            //Scale the final RenderTarget by the resolution
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, null);
            spriteBatch.Draw(FinalRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, ResolutionScale, SpriteEffects.None, 1f);
            spriteBatch.End();

            //Mark that we stopped drawing
            StartedDrawing = false;

            //Invoke the event saying we completely finished rendering
            FullyConcludedDrawingEvent?.Invoke(FinalRenderTarget);
        }

        /// <summary>
        /// Adds a post-processing effect.
        /// </summary>
        /// <param name="effect">The post-processing effect to add.</param>
        /// <param name="index">The index to insert the effect at. If less than 0, it will add it to the end of the list.</param>
        public void AddPostProcessingEffect(Effect effect, int index = -1)
        {
            if (index < 0)
            {
                PostProcessingEffects.Add(effect);
            }
            else
            {
                PostProcessingEffects.Insert(index, effect);
            }
        }

        /// <summary>
        /// Removes a post-processing effect.
        /// </summary>
        /// <param name="effectToRemove">The post-processing effect to remove.</param>
        public void RemovePostProcessingEffect(Effect effectToRemove)
        {
            PostProcessingEffects.Remove(effectToRemove);
        }

        /// <summary>
        /// Removes a post-processing effect at a particular index.
        /// </summary>
        /// <param name="index">The index to remove the post-processing effect at.</param>
        public void RemovePostProcessingEffect(int index)
        {
            PostProcessingEffects.RemoveAt(index);
        }

        /// <summary>
        /// Removes all post-processing effects.
        /// </summary>
        public void RemoveAllPostProcessingEffects()
        {
            PostProcessingEffects.Clear();
        }

        public void BeginBatch(SpriteBatch batch, SpriteSortMode spriteSortMode, BlendState blendState, SamplerState samplerState, 
            DepthStencilState depthStencilState, RasterizerState rasterizerState, Effect effect, Matrix? transformMatrix)
        {
            batch.Begin(spriteSortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }

        public void BeginBatch(SpriteBatch batch, BlendState blendState, SamplerState samplerState, Effect effect, Matrix? transformMatrix)
        {
            BeginBatch(batch, SpriteSortMode.FrontToBack, blendState, samplerState, null, null, effect, transformMatrix);
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

        public void DrawBatchText(SpriteBatch batch, SpriteFont spriteFont, StringBuilder stringBuilder, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            batch.DrawString(spriteFont, stringBuilder, position, color, rotation, origin, scale, SpriteEffects.None, layer);
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

        public void DrawText(SpriteFont spriteFont, StringBuilder stringBuilder, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            DrawBatchText(spriteBatch, spriteFont, stringBuilder, position, color, rotation, origin, scale, layer);
        }

        public void DrawUIText(SpriteFont spriteFont, string text, Vector2 position, Color color, float layer)
        {
            DrawUIText(spriteFont, text, position, color, 0f, Vector2.Zero, 1f, layer);
        }

        public void DrawUIText(SpriteFont spriteFont, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            DrawBatchText(uiBatch, spriteFont, text, position, color, rotation, origin, scale, layer);
        }

        public void DrawUIText(SpriteFont spriteFont, StringBuilder stringBuilder, Vector2 position, Color color, float rotation, Vector2 origin, float scale, float layer)
        {
            DrawBatchText(uiBatch, spriteFont, stringBuilder, position, color, rotation, origin, scale, layer);
        }

        #endregion
    }
}
