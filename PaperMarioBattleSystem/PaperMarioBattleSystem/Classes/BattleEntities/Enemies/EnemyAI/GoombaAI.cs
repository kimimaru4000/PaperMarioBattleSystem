using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goomba enemy AI.
    /// </summary>
    public class GoombaAI : EnemyAIBehavior
    {
        public GoombaAI(BattleEnemy enemy) : base(enemy)
        {

        }

        public override void PerformAction()
        {
            Enemy.StartAction(new Jump(), false, BattleManager.Instance.GetFrontPlayer().GetTrueTarget());
        }
    }
}
