using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles lighting for Dark battles.
    /// <para>This is a Singleton.</para>
    /// </summary>
    public sealed class LightingManager : ICleanup
    {
        #region Singleton Fields

        public static LightingManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LightingManager();
                }

                return instance;
            }
        }

        public static bool HasInstance => (instance != null);

        private static LightingManager instance = null;

        #endregion

        /// <summary>
        /// The darkness object for Dark battles.
        /// </summary>
        private BattleDarknessObj BattleDarkness = null;

        /// <summary>
        /// The lightmap generated using information from the darkness object.
        /// This lightmap is blended with the main RenderTarget to create the lighting.
        /// </summary>
        private RenderTarget2D LightMap = null;

        /// <summary>
        /// A RenderTarget holding a copy of the screen data blended with the lightmap.
        /// It's then copied back to the main RenderTarget.
        /// </summary>
        private RenderTarget2D ScreenCopy = null;

        /// <summary>
        /// The shader used for lighting.
        /// </summary>
        private Effect LightingShader = null;

        /// <summary>
        /// The texture representing the light.
        /// </summary>
        private Texture2D LightMask = null;

        /// <summary>
        /// Whether the LightingManager is initialized or not.
        /// </summary>
        private bool Initialized = false;

        private LightingManager()
        {

        }

        /// <summary>
        /// Initializes the LightingManager.
        /// </summary>
        /// <param name="battleDarkness">The battle darkness object.</param>
        public void Initialize(BattleDarknessObj battleDarkness)
        {
            if (Initialized == true)
            {
                Debug.LogError($"The {nameof(LightingManager)} has already been initialized! To reset it, clean it up first");
                return;
            }

            BattleDarkness = battleDarkness;
            LightMap = new RenderTarget2D(SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice, RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight);
            ScreenCopy = new RenderTarget2D(SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice, RenderingGlobals.BaseResolutionWidth, RenderingGlobals.BaseResolutionHeight);

            LightingShader = AssetManager.Instance.LoadAsset<Effect>($"{ContentGlobals.ShaderRoot}Lighting");
            LightMask = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.ShaderTextureRoot}LightMask.png");

            Initialized = true;
        }

        public void CleanUp()
        {
            LightMap?.Dispose();
            LightMap = null;

            ScreenCopy?.Dispose();
            ScreenCopy = null;

            LightingShader = null;
            LightMask = null;

            BattleDarkness = null;

            Initialized = false;

            instance = null;
        }

        /// <summary>
        /// Creates a lightmap based on the current light sources in battle.
        /// </summary>
        public void CreateLightMap()
        {
            if (Initialized == false)
            {
                Debug.LogError($"Cannot create lightmap in until initializing the {nameof(LightingManager)}!");
                return;
            }

            //Set the RenderTarget to the LightMap and clear the screen with some transparency
            //We clear to white since it'll be blended in with the main RenderTarget later
            SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice.SetRenderTarget(LightMap);
            SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice.Clear(Color.White * .4f);

            Vector2 circleSize = new Vector2(LightMask.Width, LightMask.Height);

            //Use Additive blending to allow for overlaps with the lighting
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, SpriteSortMode.Texture, BlendState.Additive, null, null, null, null, Camera.Instance.Transform);

            //Go through all light sources and generate a lightmap using their positions and diameters
            //Do this by drawing the light mask texture where the lit BattleEntities are at a scale on par with their diameters
            for (int i = 0; i < BattleDarkness.LightSources.Count; i++)
            {
                Vector2 entPos = BattleDarkness.LightSources[i].Entity.Position;
                double diameter = BattleDarkness.LightSources[i].LightRadius * 2d;

                //Divide the diameter by the texture size of the light mask so that it's the same size regardless of the light mask used
                Vector2 scale = new Vector2((float)(diameter / circleSize.X), (float)(diameter / circleSize.Y));

                SpriteRenderer.Instance.Draw(LightMask, entPos, null, Color.White, 0f, circleSize.Halve(), scale, false, false, 1f);
            }

            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);

            //Set the texture in the lighting shader to our generated lightmap
            LightingShader.Parameters["lightTex"].SetValue(LightMap);
        }

        /// <summary>
        /// Blends the generated lightmap with the current state of the screen and renders it to the screen.
        /// </summary>
        public void RenderLightMap()
        {
            if (Initialized == false)
            {
                Debug.LogError($"Cannot render the lightmap until initializing the {nameof(LightingManager)}!");
                return;
            }

            //Copy the current state of the main RenderTarget to our copy, applying the lighting shader
            SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice.SetRenderTarget(ScreenCopy);

            //Use AlphaBlend to blend the lightmap with the final output
            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, LightingShader, null);
            SpriteRenderer.Instance.Draw(SpriteRenderer.Instance.FinalRenderTarget, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, false, false, 1f);
            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);

            //Copy the RenderTarget, now with the lightmap applied, back to the main RenderTarget
            SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice.SetRenderTarget(SpriteRenderer.Instance.FinalRenderTarget);
            SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            SpriteRenderer.Instance.BeginBatch(SpriteRenderer.Instance.spriteBatch, SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, null);
            SpriteRenderer.Instance.Draw(ScreenCopy, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1f, false, false, 1f);
            SpriteRenderer.Instance.EndBatch(SpriteRenderer.Instance.spriteBatch);
        }
    }
}
