using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Vivian's Veil action.
    /// </summary>
    public sealed class Veil : MoveAction
    {
        public Veil()
        {
            Name = "Veil";

            MoveInfo = new MoveActionData(null, "Hide Mario.", MoveResourceTypes.FP, 1, CostDisplayTypes.Shown,
                MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.Single, true, null);

            SetMoveSequence(new VeilSequence(this));

            actionCommand = null;//new MultiButtonCommand();
        }

        public override void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, BattleManager.Instance.GetEntityAllies(User, MoveProperties.HeightsAffected));
        }
    }
}
