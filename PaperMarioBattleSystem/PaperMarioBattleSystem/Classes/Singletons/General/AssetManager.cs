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

        private AssetManager()
        {
            
        }

        public void Initialize(ContentManager content)
        {
            Content = content;
            Content.RootDirectory = ContentGlobals.ContentRoot;

            Font = LoadAsset<SpriteFont>("Fonts/Font");
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
            return Content.Load<T>(assetPath);
        }
    }
}
