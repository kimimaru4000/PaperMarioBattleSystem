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
            MoveInfo.Description = "Headbonk an enemy.";

            InteractionParamHolder damageInfo = (InteractionParamHolder)DamageInfo;
            damageInfo.Damage = 1;
            DamageInfo = damageInfo;
        }
    }
}
