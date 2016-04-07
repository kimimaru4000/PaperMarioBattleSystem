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
            //Cap success of normal Jump to 1 since it can only bounce once
            successRate = HelperGlobals.Clamp(successRate, 0, 1);

            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].LoseHP(BaseDamage + successRate);
            }
        }

        public override void OnMenuSelected()
        {
            base.OnMenuSelected();
        }
    }
}
