using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for enemy AI behavior.
    /// </summary>
    public abstract class EnemyAIBehavior
    {
        /// <summary>
        /// The enemy choosing the action to perform.
        /// </summary>
        public BattleEnemy Enemy { get; private set; } = null;

        protected EnemyAIBehavior()
        {

        }

        protected EnemyAIBehavior(BattleEnemy enemy)
        {
            Enemy = enemy;
        }

        /// <summary>
        /// Tells the enemy to perform an action on its turn.
        /// </summary>
        public abstract void PerformAction();
    }
}
