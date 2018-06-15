using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    public sealed class SkyGuyAI : EnemyAIBehavior
    {
        public SkyGuyAI(BattleEnemy enemy) : base(enemy)
        {

        }

        public override void PerformAction()
        {
            //Try to use an item; if so, return
            if (TryUseItem() == true) return;

            Enemy.StartAction(new WindBreathAction(Enemy, 1, 4), true, Enemy.BManager.FrontPlayer);
        }
    }
}
