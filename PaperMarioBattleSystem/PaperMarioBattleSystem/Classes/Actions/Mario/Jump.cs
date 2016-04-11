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
    public class Jump : BattleAction
    {
        public Jump()
        {
            Name = "Jump";
            Description = "Jump and stomp on an enemy.";
            BaseDamage = 1;

            Command = new JumpCommand(this);
        }

        public override void OnCommandSuccess(int successRate)
        {
            DealDamage(BaseDamage);
        }

        public override void OnCommandFailed()
        {
            DealDamage(BaseDamage);

            EndSequence();
        }

        public override void OnMenuSelected()
        {
            base.OnMenuSelected();
        }
    }
}
