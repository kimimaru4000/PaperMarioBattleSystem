using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Defend action.
    /// Mario or his Partner assumes the Guard stance and has their Defense boosted by 1 until their next turn.
    /// </summary>
    public sealed class DefendAction : MoveAction
    {
        public DefendAction(BattleEntity user) : base(user)
        {
            Name = "Defend";

            Texture2D battleTex = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");
            MoveInfo = new MoveActionData(new CroppedTexture2D(battleTex, new Rectangle(580, 815, 32, 32)),
                "Defend this turn.", Enumerations.MoveResourceTypes.FP, 0, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.None, TargetSelectionMenu.EntitySelectionType.Single, false, null);

            SetMoveSequence(new DefendSequence(this, 1));
            actionCommand = null;
        }
    }
}
