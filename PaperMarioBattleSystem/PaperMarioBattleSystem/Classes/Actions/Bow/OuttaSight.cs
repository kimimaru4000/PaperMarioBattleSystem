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

            MoveInfo = new MoveActionData(null, "Hide Mario.", MoveResourceTypes.FP, 2, CostDisplayTypes.Shown,
                MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.First, true,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Hovering });

            SetMoveSequence(new OuttaSightSequence(this));

            //Outta Sight does not have an Action Command
            actionCommand = null;
        }
    }
}
