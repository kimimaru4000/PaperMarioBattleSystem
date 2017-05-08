using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Charged Status Effect.
    /// The entity stores power for the next attack.
    /// <para>This StatusEffect does not end over time. It ends when the entity afflicted uses a move that can expend the charge.</para>
    /// </summary>
    public sealed class ChargedStatus : StatusEffect
    {
        /// <summary>
        /// The amount of total damage stored in the charge.
        /// This value gets updated to reflect the total charge stored from all charges.
        /// </summary>
        private int TotalChargeDamage = 0;

        public ChargedStatus(int chargeDamage)
        {
            StatusType = Enumerations.StatusTypes.Charged;
            Alignment = StatusAlignments.Positive;
            
            Duration = StatusGlobals.InfiniteDuration;

            AfflictedMessage = "Charged! Attack Power is\nnow boosted!";

            TotalChargeDamage = chargeDamage;
        }

        public override void Refresh(StatusEffect newStatus)
        {
            ChargedStatus charged = (ChargedStatus)newStatus;

            TotalChargeDamage += charged.TotalChargeDamage;

            //Charge damage is capped at the max damage that can be dealt
            TotalChargeDamage = UtilityGlobals.Clamp(TotalChargeDamage, BattleGlobals.MinDamage, BattleGlobals.MaxDamage);

            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.ChargedDamage, TotalChargeDamage);
        }

        protected override void OnAfflict()
        {
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.ChargedDamage, TotalChargeDamage);
        }

        protected override void OnEnd()
        {
            TotalChargeDamage = 0;
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.ChargedDamage);
        }

        protected override void OnPhaseCycleStart()
        {
            
        }

        protected override void OnSuspend()
        {
            EntityAfflicted.EntityProperties.RemoveAdditionalProperty(Enumerations.AdditionalProperty.ChargedDamage);
        }

        protected override void OnResume()
        {
            //Add the property back with the previous charged damage
            EntityAfflicted.EntityProperties.AddAdditionalProperty(Enumerations.AdditionalProperty.ChargedDamage, TotalChargeDamage);
        }

        public override StatusEffect Copy()
        {
            return new ChargedStatus(TotalChargeDamage);
        }
    }
}
