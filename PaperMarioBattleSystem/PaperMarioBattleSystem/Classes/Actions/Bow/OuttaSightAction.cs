using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Bow's Outta Sight action.
    /// </summary>
    public sealed class OuttaSightAction : MoveAction
    {
        public OuttaSightAction()
        {
            Name = "Outta Sight";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 14, 22, 22)),
                "Make Mario transparent so he\ncan avoid enemy attacks.", MoveResourceTypes.FP, 2, CostDisplayTypes.Shown,
                MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.Single, true, null);

            SetMoveSequence(new OuttaSightSequence(this));

            //Outta Sight does not have an Action Command
            actionCommand = null;
        }
    }
}
