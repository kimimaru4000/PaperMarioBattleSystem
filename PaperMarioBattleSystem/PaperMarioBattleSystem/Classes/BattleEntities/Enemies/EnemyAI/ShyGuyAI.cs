using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public sealed class ShyGuyAI : EnemyAIBehavior
    {
        public ShyGuyAI(BattleEnemy enemy) : base(enemy)
        {

        }

        public override void PerformAction()
        {
            Enemy.StartAction(new SwapMarioPartnerAction(Enemy), true, BattleManager.Instance.FrontPlayer);
        }
    }
}
