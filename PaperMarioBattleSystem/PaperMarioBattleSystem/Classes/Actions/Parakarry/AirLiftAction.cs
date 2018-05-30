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
    /// Parakarry's Air Lift move.
    /// </summary>
    public sealed class AirLiftAction : MoveAction
    {
        private double CommandTime = 3500d;
        private Keys ButtonToPress = Keys.Z;

        public AirLiftAction(BattleEntity user) : base(user)
        {
            Name = "Air Lift";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 46, 22, 22)),
                "If it works, will carry an enemy\n away from the battle.", MoveResourceTypes.FP, 3, CostDisplayTypes.Shown, MoveAffectionTypes.Other,
                TargetSelectionMenu.EntitySelectionType.Single, false, null, EntityTypes.Enemy);

            DamageInfo = new DamageData(0, Elements.Normal, false, ContactTypes.Latch, ContactProperties.None, null, true, false, 
                DefensiveActionTypes.None, DamageEffects.None);

            SetMoveSequence(new AirLiftSequence(this));
            actionCommand = new AirLiftCommand(MoveSequence, 15d, 100d, 8d, .15d, CommandTime, ButtonToPress);
        }
    }
}
