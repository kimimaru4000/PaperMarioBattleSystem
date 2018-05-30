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
    /// The Power Lift action. Move the cursor on a 3x3 grid to hit the red and blue arrows to boost Attack and Defense.
    /// </summary>
    public sealed class PowerLiftAction : SpecialMoveAction
    {
        private const double ActionCommandTime = 15000d;
        
        public PowerLiftAction(BattleEntity user) : base(user)
        {
            Name = "Power Lift";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(158, 987, 40, 37)),
                "Briefly increases your party's\nAttack and Defense power.", Enumerations.MoveResourceTypes.SSSP,
                300, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Self | Enumerations.MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.All, false, null);

            SetMoveSequence(new PowerLiftSequence(this));
            actionCommand = new PowerLiftCommand(MoveSequence, ActionCommandTime);
        }
    }
}
