using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Jump action
    /// </summary>
    public sealed class Jump : BattleAction
    {
        public Jump()
        {
            Name = "Jump";
            Description = "Jump and stomp on an enemy.";
            BaseDamage = 1;

            Command = new JumpCommand(this);
        }

        protected override void OnActionCompleted(int successRate, BattleEntity[] targets)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].LoseHP(BaseDamage + successRate);
            }
        }

        public override void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection((targets) =>
            {
                BattleUIManager.Instance.ClearMenuStack();
                BattleManager.Instance.EntityTurn.StartAction(this, targets);
            }, SelectionType, BattleManager.Instance.GetAliveEnemies());
        }
    }
}
