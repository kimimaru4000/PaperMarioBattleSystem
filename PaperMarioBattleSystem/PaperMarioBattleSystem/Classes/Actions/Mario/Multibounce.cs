using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class Multibounce : Jump
    {
        public Multibounce()
        {
            Name = "Multibounce";

            MoveInfo.Description = "Lets you do a Multibounce. Uses 2 FP. Jumps on all enemies in a row if action command is timed right.";
            MoveInfo.ResourceCost = 2;
            MoveInfo.SelectionType = TargetSelectionMenu.EntitySelectionType.All;

            SetMoveSequence(new MultibounceSequence(this));
        }
    }
}
