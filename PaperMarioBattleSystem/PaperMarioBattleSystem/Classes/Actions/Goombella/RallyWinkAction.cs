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
    /// Goombella's Rally Wink action.
    /// </summary>
    public class RallyWinkAction : MoveAction
    {
        public RallyWinkAction(BattleEntity user) : base(user)
        {
            Name = "Rally Wink";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 75, 22, 22)),
                "Wink at Mario to give him the\ncourage for an extra attack.", MoveResourceTypes.FP, 4, CostDisplayTypes.Shown, MoveAffectionTypes.Ally,
                Enumerations.EntitySelectionType.Single, false, null, null);

            //Rally Wink has damage data for a failed Action Command
            //It's used to show the move is ineffective
            DamageInfo = new DamageData(0, Elements.Normal, false, ContactTypes.None, ContactProperties.Ranged, null, true, false,
                DefensiveActionTypes.Guard | DefensiveActionTypes.Superguard, DamageEffects.None);

            SetMoveSequence(new RallyWinkSequence(this));
            actionCommand = new RallyWinkCommand(MoveSequence, new Keys[] { Keys.Z, Keys.X }, 100d, 4000d, 1000d, 10d, .2d, new Vector2(200f, 1f),
                new ActionCommandGlobals.BarRangeData(66f, 101f, 1, ActionCommand.CommandRank.Nice, Color.AliceBlue));
        }
    }
}
