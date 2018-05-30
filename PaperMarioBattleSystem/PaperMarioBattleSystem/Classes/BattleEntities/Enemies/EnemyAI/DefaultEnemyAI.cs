using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The default Enemy AI behavior; do nothing.
    /// </summary>
    public sealed class DefaultEnemyAI : EnemyAIBehavior
    {
        public DefaultEnemyAI(BattleEnemy enemy) : base(enemy)
        {

        }

        public override void PerformAction()
        {
            Enemy.StartAction(new NoAction(Enemy), true, null);
        }
    }
}
