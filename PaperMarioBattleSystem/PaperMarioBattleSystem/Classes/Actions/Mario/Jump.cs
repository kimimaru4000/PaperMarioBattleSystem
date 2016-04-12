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
            BaseLength = 1800f;

            Command = new JumpCommand(this);

            ActionSequence.SetEntity(User);
            ActionSequence.AddSequence(new MoveAmount(new Vector2(25f, 0), 500f), new Wait(500f),
                new MoveAmount(new Vector2(0f, -50f), 400f), new MoveAmount(new Vector2(0f, 50), 400f));
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
