using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Attack FX Badges.
    /// </summary>
    public abstract class AttackFXBadge : Badge
    {
        /// <summary>
        /// The Attack FX sound to play.
        /// </summary>
        protected SoundManager.Sound SoundToPlay = SoundManager.Sound.AttackFXB;

        protected override void OnEquip()
        {
            AttackFXManager manager = EntityEquipped.EntityProperties.GetAdditionalProperty<AttackFXManager>(Enumerations.AdditionalProperty.AttackFXSounds);

            //If the AttackFXManager is null, create it
            if (manager == null)
            {
                manager = new AttackFXManager();
                manager.Initialize(EntityEquipped);

                //Add it as a property to the BattleEntity
                EntityEquipped.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.AttackFXSounds, manager);
            }

            //Add the sound
            manager.AddSound(SoundToPlay);
        }

        protected override void OnUnequip()
        {
            AttackFXManager manager = EntityEquipped.EntityProperties.GetAdditionalProperty<AttackFXManager>(Enumerations.AdditionalProperty.AttackFXSounds);

            if (manager != null)
            {
                //Remove the sound
                manager.RemoveSound(SoundToPlay);

                //If there are no more sounds to play, clean up the AttackFXManager and remove the property from the BattleEntity
                if (manager.SoundCount == 0)
                {
                    manager.CleanUp();

                    EntityEquipped.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.AttackFXSounds);
                }

                manager = null;
            }
        }
    }
}
