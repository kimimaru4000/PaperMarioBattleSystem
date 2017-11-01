using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The HP Drain Badge - Reduces Mario's Attack by 1 and recovers 1 HP after damaging one or more enemies.
    /// </summary>
    public class HPDrainBadge : Badge
    {
        private const int AttackReduction = 1;

        public HPDrainBadge()
        {
            Name = "HP Drain";
            Description = "Drops Mario's Attack power by 1 but regain 1 HP per attack.";

            BPCost = 1;
            PriceValue = 50;
            
            BadgeType = BadgeGlobals.BadgeTypes.HPDrain;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.DealtDamageEvent -= OnDamagedEntity;
            EntityEquipped.DealtDamageEvent += OnDamagedEntity;

            EntityEquipped.LowerAttack(AttackReduction);
        }

        protected override void OnUnequip()
        {
            EntityEquipped.DealtDamageEvent -= OnDamagedEntity;

            EntityEquipped.RaiseAttack(AttackReduction);
        }

        private void OnDamagedEntity(InteractionHolder damageInfo)
        {
            //NOTE: Prepare a BattleEvent to restore the amount of HP on turn end
            //Reset the amount when the BattleEntity is done with its turn
        }
    }
}
