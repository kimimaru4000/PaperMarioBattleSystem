using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class PowerBounceAction : JumpAction
    {
        public PowerBounceAction(BattleEntity user) : base(user)
        {
            Name = "Power Bounce";
            MoveInfo.Icon.SetRect(new Rectangle(939, 136, 24, 21));
            MoveInfo.Description = "Bounce multiple times on an enemy";
            MoveInfo.ResourceCost = 3;
            
            SetMoveSequence(new PowerBounceSequence(this));
        }
    }
}
