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
            EntityEquipped.DealtDamageEvent -= OnDealtDamage;
            EntityEquipped.DealtDamageEvent += OnDealtDamage;
        }

        protected override void OnUnequip()
        {
            EntityEquipped.DealtDamageEvent -= OnDealtDamage;
        }

        private void OnDealtDamage(InteractionHolder damageInfo)
        {
            //Attack FX badges don't take effect if damaging with Payback
            if (damageInfo.IsPaybackDamage == true || damageInfo.ContactType == Enumerations.ContactTypes.None)
            {
                return;
            }
            
            SoundManager.Instance.PlaySound(SoundToPlay);
        }
    }
}
