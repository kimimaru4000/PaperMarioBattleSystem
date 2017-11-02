using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The FP Drain Badge - Reduces Mario's Attack by 1 and recovers 1 FP after damaging one or more enemies.
    /// </summary>
    public sealed class FPDrainBadge : Badge
    {
        private const int AttackReduction = 1;

        /// <summary>
        /// Tells whether the heal is queued after the BattleEntity damages an enemy.
        /// This prevents it from healing multiple times if the BattleEntity deals damage again.
        /// </summary>
        private bool QueuedHeal = false;

        public FPDrainBadge()
        {
            Name = "HP Drain";
            Description = "Drop Mario's Attack power by 1 but regain 1 FP per attack.";

            BPCost = 1;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.FPDrain;
            AffectedType = BadgeGlobals.AffectedTypes.Self;
        }

        protected override void OnEquip()
        {
            EntityEquipped.DealtDamageEvent -= OnDamagedEntity;
            EntityEquipped.DealtDamageEvent += OnDamagedEntity;

            EntityEquipped.TurnEndEvent -= OnEntityTurnEnd;
            EntityEquipped.TurnEndEvent += OnEntityTurnEnd;

            EntityEquipped.LowerAttack(AttackReduction);

            QueuedHeal = false;
        }

        protected override void OnUnequip()
        {
            EntityEquipped.DealtDamageEvent -= OnDamagedEntity;

            EntityEquipped.TurnEndEvent -= OnEntityTurnEnd;

            EntityEquipped.RaiseAttack(AttackReduction);

            QueuedHeal = false;
        }

        private void OnDamagedEntity(InteractionHolder damageInfo)
        {
            //FP Drain doesn't take effect if damaging with Payback or if the damage dealt is 0
            if (damageInfo.IsPaybackDamage == true || damageInfo.TotalDamage == 0 || QueuedHeal == true)
            {
                return;
            }

            //Queue a Battle Event to heal FP after your turn is over
            BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.HealFP,
                new BattleManager.BattleState[] { BattleManager.BattleState.TurnEnd },
                new HealFPBattleEvent(EntityEquipped, 1));

            //Mark that the heal is queued
            QueuedHeal = true;
        }

        private void OnEntityTurnEnd()
        {
            //Reset that the heal was queued when the entity's turn ends
            QueuedHeal = false;
        }
    }
}
