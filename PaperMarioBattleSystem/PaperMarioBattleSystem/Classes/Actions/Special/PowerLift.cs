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
    public sealed class PowerLift : SpecialMoveAction
    {
        private const double ActionCommandTime = 15000d;
        
        public PowerLift()
        {
            Name = "Power Lift";

            //NOTE: Commented out until we have a means to obtain Crystal Star Star Power (Audience)
            //SPType = StarPowerGlobals.StarPowerTypes.CrystalStar;

            SPCost = 300;
            MoveInfo = new MoveActionData(null, "Briefly increases your party's\nAttack and Defense power.", Enumerations.MoveResourceTypes.SP,
                300, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Ally, TargetSelectionMenu.EntitySelectionType.All, false, null);

            SetMoveSequence(new PowerLiftSequence(this));
            actionCommand = new PowerLiftCommand(MoveSequence, ActionCommandTime);
        }
    }
}
