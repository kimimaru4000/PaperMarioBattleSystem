using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The action for D-Down Jump.
    /// </summary>
    public sealed class DDownJumpAction : Jump
    {
        public DDownJumpAction()
        {
            Name = "D-Down Jump";

            //Set other data
            MoveInfo.Icon.SetRect(new Rectangle(939, 194, 24, 23));
            MoveInfo.Description = "Pierces enemy defense with a jump.";

            MoveInfo.ResourceCost = 2;

            DamageInfo.Piercing = true;
        }
    }
}
