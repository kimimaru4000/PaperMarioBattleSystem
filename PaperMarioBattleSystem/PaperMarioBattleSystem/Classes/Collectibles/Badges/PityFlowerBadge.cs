using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Pity Flower Badge - Has a 30% chance of recovering 1 FP after taking damage.
    /// </summary>
    public sealed class PityFlowerBadge : Badge
    {
        /// <summary>
        /// The chance of the equipped BattleEntity recovering FP when taking damage.
        /// </summary>
        private const double FPRecoverChance = 30d;

        /// <summary>
        /// The amount of FP to recover.
        /// </summary>
        private const int FPRecoverAmount = 1;

        public PityFlowerBadge()
        {
            Name = "Pity Flower";
            Description = "When Mario takes damage, occasionally recover 1 FP.";

            BPCost = 3;
            PriceValue = 100;

            BadgeType = BadgeGlobals.BadgeTypes.PityFlower;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.DamageTakenEvent -= HealFP;
            EntityEquipped.DamageTakenEvent += HealFP;
        }

        protected override void OnUnequip()
        {
            EntityEquipped.DamageTakenEvent -= HealFP;
        }

        private void HealFP(in InteractionHolder damageInfo)
        {
            //The entity doesn't recover FP if dead (TEST THIS) or if the damage dealt was 0 or less
            if (EntityEquipped.IsDead == false && damageInfo.TotalDamage > 0)
            {
                //Test for recovering FP
                if (UtilityGlobals.TestRandomCondition(FPRecoverChance) == true)
                {
                    //Recover FP
                    EntityEquipped.HealFP(FPRecoverAmount);
                }
            }
        }
    }
}
