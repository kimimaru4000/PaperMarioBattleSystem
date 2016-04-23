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
    public sealed class Bonk : Jump
    {
        public Bonk()
        {
            Name = "Bonk";
            Description = "Headbonk an enemy.";
            BaseDamage = 1;

            Command = new JumpCommand(this, 500f);
        }
    }
}
