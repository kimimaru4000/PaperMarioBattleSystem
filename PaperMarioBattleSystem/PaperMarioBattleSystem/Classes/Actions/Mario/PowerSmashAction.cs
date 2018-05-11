using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Power Smash action.
    /// </summary>
    public sealed class PowerSmashAction : HammerAction
    {
        public PowerSmashAction(int numEquipped)
        {
            Name = "Power Smash";

            MoveInfo.Description = "Hammer a single enemy using\nlots of Attack power.";

            //Each additional Power Smash costs 2 more FP
            MoveInfo.ResourceCost = 2 * numEquipped;

            //Each additional Power Smash adds 1 more damage
            SetMoveSequence(new HammerSequence(this, 2 + (numEquipped - 1)));
            actionCommand = new HammerCommand(MoveSequence, 4, 800d);
        }
    }
}
