using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PaperMarioBattleSystem.Extensions;

namespace PaperMarioBattleSystem
{
    public sealed class MultibounceAction : JumpAction
    {
        public MultibounceAction(BattleEntity user) : base(user)
        {
            Name = "Multibounce";

            MoveInfo.Icon.SetRect(new Rectangle(939, 167, 24, 22));
            MoveInfo.Description = "Lets you do a Multibounce. Uses 2 FP. Jumps on all enemies in a row if action command is timed right.";
            MoveInfo.ResourceCost = 2;
            MoveInfo.SelectionType = Enumerations.EntitySelectionType.All;
            MoveInfo.OtherEntTypes = new Enumerations.EntityTypes[] { User.GetOpposingEntityType() };

            SetMoveSequence(new MultibounceSequence(this));
        }
    }
}
