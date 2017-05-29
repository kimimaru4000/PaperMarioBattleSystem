using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Peekaboo Badge - Allows Mario and his Partner to see enemy HP.
    /// </summary>
    public sealed class PeekabooBadge : Badge
    {
        public PeekabooBadge()
        {
            Name = "Peekaboo";
            Description = "Make enemy HP visible.";

            BPCost = 2;
            PriceValue = 50;

            BadgeType = BadgeGlobals.BadgeTypes.Peekaboo;
            AffectedType = BadgeGlobals.AffectedTypes.Both;
        }

        protected override void OnEquip()
        {
            BattleManager.Instance.EnemyAddedEvent -= OnEnemyAdded;
            BattleManager.Instance.EnemyAddedEvent += OnEnemyAdded;

            //For all current enemies, show their HP
            BattleEntity[] enemies = BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Enemy, null);
            for (int i = 0; i < enemies.Length; i++)
            {
                AddShowHPProperty((BattleEnemy)enemies[i]);
            }
        }

        protected override void OnUnequip()
        {
            BattleManager.Instance.EnemyAddedEvent -= OnEnemyAdded;

            //For all current enemies, remove showing their HP (unless tattled, which will handle itself)
            BattleEntity[] enemies = BattleManager.Instance.GetEntities(Enumerations.EntityTypes.Enemy, null);
            for (int i = 0; i < enemies.Length; i++)
            {
                RemoveShowHPProperty((BattleEnemy)enemies[i]);
            }
        }

        private void OnEnemyAdded(BattleEnemy enemy)
        {
            //Tell the enemy to show its HP. Note that we have an integer in case they have been tattled
            AddShowHPProperty(enemy);
        }

        private void AddShowHPProperty(BattleEnemy enemy)
        {
            enemy.AddShowHPProperty();
        }

        private void RemoveShowHPProperty(BattleEnemy enemy)
        {
            enemy.SubtractShowHPProperty();
        }
    }
}
