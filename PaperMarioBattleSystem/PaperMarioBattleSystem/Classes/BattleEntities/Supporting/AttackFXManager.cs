using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Manages playing sounds associated with the Attack FX badges when a BattleEntity deals damage.
    /// <para>More of the same Attack FX Badge will increase the chances of the sound associated with it playing.</para>
    /// </summary>
    public sealed class AttackFXManager : ICleanup
    {
        private BattleEntity Entity = null;
        private readonly List<SoundManager.Sound> PotentialSounds = new List<SoundManager.Sound>();

        private bool Initialized = false;

        public int SoundCount => PotentialSounds.Count;

        public AttackFXManager()
        {
           
        }

        /// <summary>
        /// Initializes the AttackFXManager.
        /// </summary>
        public void Initialize(BattleEntity entity)
        {
            if (Initialized == true) return;

            Entity = entity;

            Entity.DealtDamageEvent -= OnEntityDealtDamage;
            Entity.DealtDamageEvent += OnEntityDealtDamage;

            Initialized = true;
        }

        /// <summary>
        /// Cleans up the AttackFXManager.
        /// </summary>
        public void CleanUp()
        {
            Entity.DealtDamageEvent -= OnEntityDealtDamage;

            Entity = null;
            PotentialSounds.Clear();

            Initialized = false;
        }

        /// <summary>
        /// Adds a sound to the list of sounds the AttackFXManager may pick to play.
        /// </summary>
        /// <param name="sound">The sound.</param>
        public void AddSound(SoundManager.Sound sound)
        {
            PotentialSounds.Add(sound);
        }

        /// <summary>
        /// Removes a sound from the list of sounds the AttackFXManager may pick to play.
        /// </summary>
        /// <param name="sound">The sound.</param>
        public void RemoveSound(SoundManager.Sound sound)
        {
            PotentialSounds.Remove(sound);
        }

        private void OnEntityDealtDamage(in InteractionHolder damageInfo)
        {
            //Attack FX badges don't take effect if damaging with Payback
            if (damageInfo.IsPaybackDamage == true || damageInfo.ContactType == Enumerations.ContactTypes.None)
            {
                return;
            }

            //We can't play any sounds if none are available (this shouldn't happen)
            if (PotentialSounds.Count == 0)
            {
                Debug.LogError($"{nameof(AttackFXManager)} on {Entity.Name} has no sounds it can play. If no sounds are available, the instance should be cleaned up and removed!");
                return;
            }

            //Choose a random sound from the list to play
            int index = RandomGlobals.Randomizer.Next(0, SoundCount);

            //Play the sound at the index
            SoundManager.Instance.PlaySound(PotentialSounds[index]);
        }
    }
}
