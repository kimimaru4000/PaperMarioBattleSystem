using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(33, 961, 24, 24)),
                "Recover HP & FP by 5. Also cures poisoning and Shrink.",
                Enumerations.MoveResourceTypes.SSSP, 100, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Self,
                TargetSelectionMenu.EntitySelectionType.First, false, null);
            SetMoveSequence(new RefreshSequence(this));
        }
    }
}
