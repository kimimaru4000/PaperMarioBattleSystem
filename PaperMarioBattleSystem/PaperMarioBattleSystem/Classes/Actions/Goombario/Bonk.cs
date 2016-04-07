using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Goombario's Bonk action
    /// </summary>
    public sealed class Bonk : BattleAction
    {
        public Bonk()
        {
            Name = "Bonk";
            Description = "Headbonk an enemy.";
            BaseDamage = 1;
            BaseLength = 1000000f;

            Command = new HammerCommand(this);
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
            base.OnMenuSelected();
        }
    }
}
