using System;
using System.Collections.Generic;
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
    public class AssetManager : IDisposable
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

        private ContentManager Content { get; set; } = null;

        public SpriteFont Font = null;

        /// <summary>
        /// The font used in Paper Mario (pmdialog2).
        /// <para>Credit: Retriever II from MFGG</para>
        /// </summary>
        public SpriteFont PMFont = null;

        /// <summary>
        /// The font used in Paper Mario The Thousand Year Door (Hey Gorgeous).
        /// <para>Credit: http://www.kevinandamanda.com/fonts/freescrapbookfonts/hey-gorgeous/ </para>
        /// </summary>
        public SpriteFont TTYDFont = null;

        private AssetManager()
        {
            
        }

        public void Initialize(ContentManager content)
        {
            Content = content;
            Content.RootDirectory = ContentGlobals.ContentRoot;

            Font = LoadAsset<SpriteFont>("Fonts/Font");
            PMFont = LoadAsset<SpriteFont>("Fonts/PM Font");
            TTYDFont = LoadAsset<SpriteFont>("Fonts/TTYD Font");
        }

        public void Dispose()
        {
            Content.Unload();

            instance = null;
        }

        /// <summary>
        /// Load an asset of a particular type
        /// </summary>
        /// <typeparam name="T">The type of content to load</typeparam>
        /// <param name="assetPath">The path to load the asset from</param>
        /// <returns>The asset if it was successfully found. Returns the same instance if the same asset was loaded previously</returns>
        public T LoadAsset<T>(string assetPath)
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
