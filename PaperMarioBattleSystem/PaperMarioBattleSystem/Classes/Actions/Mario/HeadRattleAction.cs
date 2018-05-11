using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's Head Rattle action.
    /// It's identical to Hammer except it can confuse enemies and has a slightly longer Action Command.
    /// </summary>
    public sealed class HeadRattleAction : HammerAction
    {
        public HeadRattleAction()
        {
            Name = "Head Rattle";
            MoveInfo.ResourceCost = 2;
            MoveInfo.Description = "Wear this to use Head Rattle. " +
                           "2 FP are required to use this attack, which can confuse enemies if executed superbly. " +
                           "Wearing two or more of these badges requires more FP for the move, but enemies stay confused longer.";

            DamageInfo.Statuses = new StatusChanceHolder[] { new StatusChanceHolder(100d, new ConfusedStatus(2)) };

            SetMoveSequence(new HammerSequence(this, 0));
            actionCommand = new HammerCommand(MoveSequence, 4, 1000d);
        }
    }
}
