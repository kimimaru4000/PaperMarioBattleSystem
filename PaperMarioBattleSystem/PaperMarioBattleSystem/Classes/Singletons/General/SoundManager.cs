using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

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

        #region Enumerations

        /// <summary>
        /// Enum for Sound IDs
        /// </summary>
        public enum Sound
        {
            SwitchPartner, EnemyDeath, PartnerAway
        }

        #endregion

        private static Dictionary<Sound, string> SoundMap = new Dictionary<Sound, string>()
        {
            { Sound.SwitchPartner, $"{ContentGlobals.SoundRoot}Switch Partner.wav" },
            { Sound.EnemyDeath, $"{ContentGlobals.SoundRoot}Enemy Death.wav" },
            { Sound.PartnerAway, $"{ContentGlobals.SoundRoot}Partner Away.wav" }
        };

        public float MusicVolume 
        {
            get
            {
                return musicVolume;
            }
            set
            {
                ChangeMusicVolume(value);
            }
        }

        public float SoundVolume
        {
            get
            {
                return musicVolume;
            }
            set
            {
                ChangeSoundVolume(value);
            }
        }

        private float musicVolume = .5f;
        private float soundVolume = .5f;

        /// <summary>
        /// The currently playing sounds
        /// </summary>
        private List<SoundHolder> Sounds = null;

        private const double ClearSoundTimer = 10000d;
        private double LastPlayedSound = 0d;

        private SoundManager()
        {
            Sounds = new List<SoundHolder>();
        }

        public void Dispose()
        {
            for (int i = 0; i < Sounds.Count; i++)
            {
                Sounds[i].Dispose();
            }

            Sounds.Clear();

            instance = null;
        }

        private void UpdateLastSoundTimer()
        {
            LastPlayedSound = Time.TotalMilliseconds + ClearSoundTimer;
        }

        private void ChangeMusicVolume(float mVolume)
        {
            musicVolume = mVolume;
        }

        private void ChangeSoundVolume(float sVolume)
        {
            soundVolume = sVolume;

            //Update the volume for all the sounds
            for (int i = 0; i < Sounds.Count; i++)
            {
                Sounds[i].UpdateVolume();
            }   
        }

        private SoundHolder NextAvailableSound()
        {
            //Check for available sounds
            for (int i = 0; i < Sounds.Count; i++)
            {
                if (Sounds[i].IsPlaying == false) return Sounds[i];
            }

            //If no sounds are available, create a new sound holder and add it to the list
            SoundHolder newHolder = new SoundHolder();
            Sounds.Add(newHolder);
            return newHolder;
        }

        public void PlaySound(Sound sound, float volume = 1f)
        {
            if (SoundMap.ContainsKey(sound) == false)
            {
                Debug.LogError($"Cannot play sound {sound} because it doesn't exist in {nameof(SoundMap)}!");
                return;
            }

            //Retrieve next sound holder, then assign the sound ID to the holder
            SoundHolder holder = NextAvailableSound();
            holder.SetSound(sound);
            holder.SetVolume(volume);

            //Play the sound
            holder.Play();

            UpdateLastSoundTimer();
        }

        public void Update()
        {
            //Clear all sounds after a set amount of time
            if (Sounds.Count > 0 && Time.TotalMilliseconds >= LastPlayedSound)
            {
                Sounds.Clear();
            }
        }

        private class SoundHolder : IDisposable
        {
            /// <summary>
            /// Tells if the sound instance is playing or not
            /// </summary>
            public bool IsPlaying => (SoundInstance != null && SoundInstance.State == SoundState.Playing);

            public float CurrentVolume { get; private set; } = 1f;

            /// <summary>
            /// The SoundEffectInstance of the sound to play
            /// </summary>
            private SoundEffectInstance SoundInstance = null;

            /// <summary>
            /// The ID of the Sound
            /// </summary>
            private Sound SoundID = 0;

            public SoundHolder()
            {
                
            }

            public void Dispose()
            {
                Stop(true);
                SoundInstance.Dispose();
                SoundInstance = null;
            }

            public void SetSound(Sound sound)
            {
                //If it's the same sound, then return
                if (SoundInstance != null && SoundID == sound) return;

                SoundID = sound;

                //Load the sound asset and create an instance
                SoundEffect newSound = AssetManager.Instance.LoadAsset<SoundEffect>(SoundMap[SoundID]);
                SoundInstance = newSound.CreateInstance();
            }

            public void Play()
            {
                SoundInstance.Play();
            }

            public void Stop(bool immediate = false)
            {
                SoundInstance.Stop(immediate);
            }

            public void SetVolume(float volume)
            {
                CurrentVolume = volume;
                UpdateVolume();
            }

            public void UpdateVolume()
            {
                SoundInstance.Volume = CurrentVolume * SoundManager.Instance.SoundVolume;
            }
        }
    }
}
