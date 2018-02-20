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
    /// Watt's Power Shock move.
    /// </summary>
    public sealed class PowerShockAction : MoveAction
    {
        private double CommandTime = 5000d;
        private Keys ButtonToPress = Keys.Z;

        public PowerShockAction()
        {
            Name = "Power Shock";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 14, 22, 22)),
                "If it works, paralyze an enemy\nwith an electric shock.", MoveResourceTypes.FP, 2, CostDisplayTypes.Shown, MoveAffectionTypes.Other,
                TargetSelectionMenu.EntitySelectionType.Single, false, null, User.GetOpposingEntityType());

            DamageInfo = new DamageData(0, Elements.Electric, false, ContactTypes.SideDirect, ContactProperties.None,
                new StatusChanceHolder[] { new StatusChanceHolder(100d, new ParalyzedStatus(3)) }, DamageEffects.None);

            SetMoveSequence(new PowerShockSequence(this));
            actionCommand = new MashButtonCommand(MoveSequence, 100d, 8d, CommandTime, ButtonToPress);
        }
    }
}
