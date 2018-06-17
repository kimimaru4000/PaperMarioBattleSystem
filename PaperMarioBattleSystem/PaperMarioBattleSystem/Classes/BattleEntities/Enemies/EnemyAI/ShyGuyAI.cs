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
            //Try to use an item; if so, return
            if (TryUseItem() == true) return;

            Enemy.StartAction(new KissyKissyAction(Enemy, true, 5, 2, Enumerations.Elements.Normal, true, true, null), true, Enemy.BManager.FrontPlayer);
        }
    }
}
