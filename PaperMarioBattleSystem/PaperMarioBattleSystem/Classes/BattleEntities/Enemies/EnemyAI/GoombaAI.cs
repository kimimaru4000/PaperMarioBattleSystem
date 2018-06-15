using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Extensions;

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
            //Try to use an item; if so, return
            if (TryUseItem() == true) return;

            Enemy.StartAction(new JumpAction(Enemy), false, Enemy.BManager.FrontPlayer.GetTrueTarget());
        }
    }
}
