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

            MoveInfo = new MoveActionData(null, 0, "Recover HP & FP by 5. Also cures poisoning and Shrink.",
                TargetSelectionMenu.EntitySelectionType.First, Enumerations.EntityTypes.Player, null);
            SetMoveSequence(new RefreshSequence(this));
        }

        public override void OnMenuSelected()
        {
            //Refresh only targets the user
            BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, User);
        }
    }
}
