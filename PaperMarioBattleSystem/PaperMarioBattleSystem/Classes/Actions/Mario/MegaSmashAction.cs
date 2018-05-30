using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Mega Smash action.
    /// </summary>
    public sealed class MegaSmashAction : HammerAction
    {
        public MegaSmashAction(BattleEntity user) : base(user)
        {
            Name = "Mega Smash";

            MoveInfo.Icon.SetRect(new Rectangle(907, 73, 24, 24));
            MoveInfo.Description = "Hammers an enemy with a huge\namount of attack power.";
            MoveInfo.ResourceCost = 6;

            SetMoveSequence(new MegaSmashSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 1000d);
        }
    }
}
