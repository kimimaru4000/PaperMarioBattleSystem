using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Bow's Outta Sight action.
    /// </summary>
    public sealed class OuttaSight : MoveAction
    {
        public OuttaSight()
        {
            Name = "Outta Sight";

            MoveInfo = new MoveActionData(null, "Make Mario transparent so he\ncan avoid enemy attacks.", MoveResourceTypes.FP, 2, CostDisplayTypes.Shown,
                MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.Single, true, null);

            SetMoveSequence(new OuttaSightSequence(this));

            //Outta Sight does not have an Action Command
            actionCommand = null;
        }

        public override void OnMenuSelected()
        {
            BattleUIManager.Instance.StartTargetSelection(ActionStart, MoveProperties.SelectionType, BattleManager.Instance.GetEntityAllies(User, MoveProperties.HeightsAffected));
        }
    }
}
