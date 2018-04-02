using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Helps manage content
    /// <para>This is a Singleton</para>
    /// </summary>
    public class AssetManager : ICleanup
    {
        #region Singleton Fields

        public static AssetManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AssetManager();
                }

                return instance;
            }
        }

        private static AssetManager instance = null;

        #endregion

        /* Credit for the Hey Gorgeous font:
         * http://www.kevinandamanda.com/fonts/freescrapbookfonts/hey-gorgeous/
         */

        private ContentManager Content = null;

        public SpriteFont Font { get; private set; } = null;

        /// <summary>
        /// The font used in Paper Mario (pmdialog2).
        /// <para>Credit: Retriever II from MFGG</para>
        /// </summary>
        public SpriteFont PMFont { get; private set; } = null;

        /// <summary>
        /// The font used in Paper Mario The Thousand Year Door (PopJoyStd-B).
        /// <para>Credit: https://en.fontke.com/font/10390387/ (Copyright 2003 - Fontworks Japan, Inc.)</para>
        /// </summary>
        public SpriteFont TTYDFont { get; private set; } = null;

        /// <summary>
        /// The Charge shader used for Charged BattleEntities.
        /// </summary>
        public Effect ChargeShader { get; private set; } = null;

        /// <summary>
        /// The Charge texture used for the Charge shader.
        /// </summary>
        public Texture2D ChargeShaderTex { get; private set; } = null;

        /// <summary>
        /// Holds loaded raw Texture2Ds. These are disposed on cleanup.
        /// </summary>
        private Dictionary<string, Texture2D> RawTextures = null;

        /// <summary>
        /// Holds loaded raw SoundEffects. These are disposed on cleanup.
        /// </summary>
        private Dictionary<string, SoundEffect> RawSounds = null;

        private AssetManager()
        {
            
        }

        public void Initialize(ContentManager content)
        {
            Content = content;
            Content.RootDirectory = ContentGlobals.ContentRoot;

            RawTextures = new Dictionary<string, Texture2D>();
            RawSounds = new Dictionary<string, SoundEffect>();

            Font = LoadAsset<SpriteFont>("Fonts/Font");
            PMFont = LoadAsset<SpriteFont>("Fonts/PM Font");
            TTYDFont = LoadAsset<SpriteFont>("Fonts/Real TTYD Font");

            ChargeShader = LoadAsset<Effect>($"{ContentGlobals.ShaderRoot}Charge");
            ChargeShaderTex = LoadRawTexture2D($"{ContentGlobals.ShaderTextureRoot}ChargeShaderTex.png");
        }

        public void CleanUp()
        {
            //Unload all content
            Content.Unload();
            Content = null;

            //Dispose each raw texture
            foreach (KeyValuePair<string, Texture2D> texPair in RawTextures)
            {
                texPair.Value.Dispose();
            }

            //Dispose each raw sound
            foreach (KeyValuePair<string, SoundEffect> soundPair in RawSounds)
            {
                soundPair.Value.Dispose();
            }

            //Clear the dictionaries
            RawTextures.Clear();
            RawSounds.Clear();

            instance = null;
        }

        /// <summary>
        /// Loads a raw Texture2D. They're cached for quick fetching.
        /// </summary>
        /// <param name="texturePath">The path to load the Texture2D from.
        /// The content root directory is appended to the start of this.</param>
        /// <returns>A Texture2D found at <paramref name="texturePath"/>, otherwise null.</returns>
        public Texture2D LoadRawTexture2D(in string texturePath)
        {
            Texture2D tex = null;

            //Insert content at the start
            string realTexPath = texturePath.Insert(0, Content.RootDirectory + "/");

            //Return the cached texture if we have it
            if (RawTextures.ContainsKey(realTexPath) == true)
            {
                tex = RawTextures[realTexPath];
            }
            else
            {
                //Load the raw texture
                try
                {
                    using (FileStream fileStream = new FileStream(realTexPath, FileMode.Open))
                    {
                        tex = Texture2D.FromStream(SpriteRenderer.Instance.graphicsDeviceManager.GraphicsDevice, fileStream);

                        //Cache the texture for faster loading next time
                        if (tex != null)
                        {
                            RawTextures.Add(realTexPath, tex);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading raw {nameof(Texture2D)} {realTexPath}: {e.Message}");
                }
            }

            return tex;
        }

        public SoundEffect LoadRawSound(in string soundPath)
        {
            SoundEffect sound = null;
            
            //Insert content at the start
            string realSoundPath = soundPath.Insert(0, Content.RootDirectory + "/");

            //Return the cached sound if we have it
            if (RawSounds.ContainsKey(realSoundPath) == true)
            {
                sound = RawSounds[realSoundPath];
            }
            else
            {
                //Load the raw sound
                try
                {
                    using (FileStream fileStream = new FileStream(realSoundPath, FileMode.Open))
                    {
                        sound = SoundEffect.FromStream(fileStream);

                        //Cache the sound for faster loading next time
                        if (sound != null)
                        {
                            RawSounds.Add(realSoundPath, sound);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading raw {nameof(SoundEffect)} {realSoundPath}: {e.Message}");
                }
            }

            return sound;
        }

        /// <summary>
        /// Load an asset of a particular type
        /// </summary>
        /// <typeparam name="T">The type of content to load</typeparam>
        /// <param name="assetPath">The path to load the asset from</param>
        /// <returns>The asset if it was successfully found. Returns the same instance if the same asset was loaded previously</returns>
        public T LoadAsset<T>(in string assetPath)
        {
            //NOTE: I opted for this rather than not handling the exception to make the content workflow less of a hassle
            //I find that missing assets are very easy to spot, so just look at the logs if you notice an asset missing
            try
            {
                return Content.Load<T>(assetPath);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error loading asset {assetPath}: {exception.Message}");
                return default(T);
            }
        }
    }
}
