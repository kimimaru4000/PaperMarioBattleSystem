using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Defend action.
    /// Mario or his Partner assumes the Guard stance and has their Defense boosted by 1 until their next turn.
    /// </summary>
    public sealed class Defend : MoveAction
    {
        public Defend()
        {
            Name = "Defend";

            MoveInfo = new MoveActionData(null, "Defend this turn.", Enumerations.MoveResourceTypes.FP, 0, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.Single, false, null);

            SetMoveSequence(new DefendSequence(this, BattleManager.Instance.EntityTurn, 1));
            actionCommand = null;
        }
    }
}
