using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles all audio playback
    /// <para>This is a Singleton</para>
    /// </summary>
    public class SoundManager : IDisposable
    {
        #region Singleton Fields

        public static SoundManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SoundManager();
                }

                return instance;
            }
        }

        private static SoundManager instance = null;

        #endregion

        private SoundManager()
        {

        }

        public void Dispose()
        {


            instance = null;
        }
    }
}
