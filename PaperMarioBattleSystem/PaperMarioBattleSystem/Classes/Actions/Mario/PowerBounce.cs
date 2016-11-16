using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class PowerBounce : Jump
    {
        public PowerBounce()
        {
            Name = "Power Bounce";
            MoveInfo.Description = "Bounce multiple times on an enemy";
            MoveInfo.FPCost = 3;
            
            SetMoveSequence(new PowerBounceSequence(this));
        }
    }
}
