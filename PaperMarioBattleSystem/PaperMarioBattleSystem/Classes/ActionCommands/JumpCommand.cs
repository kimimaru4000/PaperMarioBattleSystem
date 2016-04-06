using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Press the jump button once before Mario almost hits the enemy
    /// </summary>
    public sealed class JumpCommand : ActionCommand
    {
        public JumpCommand(BattleAction battleAction) : base(battleAction)
        {
            
        }

        protected override void ReadInput()
        {
            
        }
    }
}
