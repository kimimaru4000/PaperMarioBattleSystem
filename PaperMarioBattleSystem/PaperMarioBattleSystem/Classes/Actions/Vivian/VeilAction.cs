using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Vivian's Veil action.
    /// </summary>
    public sealed class VeilAction : MoveAction
    {
        private const int NumCommandButtons = 5;
        private const double ActionCommandTime = 5000d;
        private readonly Keys[] ValidCommandButtons = new Keys[] { Keys.Z, Keys.X };

        public VeilAction(BattleEntity user) : base(user)
        {
            Name = "Veil";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 14, 22, 22)),
                "Hide in the shadows with\nMario to avoid attacks.", MoveResourceTypes.FP, 1, CostDisplayTypes.Shown,
                MoveAffectionTypes.Ally, Enumerations.EntitySelectionType.Single, true, null);

            SetMoveSequence(new VeilSequence(this));
            actionCommand = new MultiButtonCommand(MoveSequence, NumCommandButtons, NumCommandButtons, ActionCommandTime, ValidCommandButtons);
        }
    }
}
