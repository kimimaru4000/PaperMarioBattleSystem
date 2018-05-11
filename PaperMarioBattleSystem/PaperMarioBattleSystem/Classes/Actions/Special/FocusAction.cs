using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Focus action. Mario regains some Star Spirit Star Power.
    /// <para>The amount regained can be increased with Deep Focus Badges.
    /// Partners can also use this move with the Group Focus Badge, but Deep Focus doesn't increase the amount they gain when using it.</para>
    /// </summary>
    public sealed class FocusAction : SpecialMoveAction
    {
        public FocusAction()
        {
            Name = "Focus";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(182, 812, 24, 24)),
                "Charge Star Energy.", Enumerations.MoveResourceTypes.SSSP, 0,
                Enumerations.CostDisplayTypes.Hidden, Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.First,
                false, null);
            SetMoveSequence(new FocusSequence(this));
        }
    }
}
