using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Refresh action. Eldstar restores 5 HP and FP to Mario at the cost of 1 SP.
    /// </summary>
    public sealed class Refresh : SpecialMoveAction
    {
        public Refresh()
        {
            Name = "Refresh";

            SPCost = 100;

            MoveInfo = new MoveActionData(null, "Recover HP & FP by 5. Also cures poisoning and Shrink.",
                Enumerations.MoveResourceTypes.SP, 100, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Self,
                TargetSelectionMenu.EntitySelectionType.First, false, null);
            SetMoveSequence(new RefreshSequence(this));
        }
    }
}
