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
    /// Kooper's Shell Toss action.
    /// </summary>
    public sealed class ShellTossAction : MoveAction
    {
        public ShellTossAction(BattleEntity user) : base(user)
        {
            Name = "Shell Toss";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(216, 845, 22, 22)),
                "Shoot yourself at an enemy.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, Enumerations.EntitySelectionType.First, true,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Hovering }, User.GetOpposingEntityType(), EntityTypes.Neutral);

            DamageInfo = new DamageData(1, Elements.Normal, false, ContactTypes.SideDirect, ContactProperties.Protected, null, DamageEffects.RemovesSegment);

            SetMoveSequence(new ShellTossSequence(this));
            actionCommand = new HammerCommand(MoveSequence, 4, 500d);
        }
    }
}
