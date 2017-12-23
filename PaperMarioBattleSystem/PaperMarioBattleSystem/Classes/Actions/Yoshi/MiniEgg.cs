using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Yoshi's Mini-Egg move.
    /// </summary>
    public sealed class MiniEgg : MoveAction
    {
        public MiniEgg()
        {
            Name = "Mini-Egg";
            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 46, 22, 22)),
                "Throws Yoshi eggs at all enemies and shrinks them.", Enumerations.MoveResourceTypes.FP,
                3, Enumerations.CostDisplayTypes.Shown, Enumerations.MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.All,
                false, null, User.GetOpposingEntityType());
            DamageInfo = new DamageData(1, Enumerations.Elements.Normal, false, Enumerations.ContactTypes.None, Enumerations.ContactProperties.Ranged,
                new StatusChanceHolder[] { new StatusChanceHolder(100d, new TinyStatus(3)) }, Enumerations.DamageEffects.None);

            SetMoveSequence(new MiniEggSequence(this));
            actionCommand = new TimedLightCommand(MoveSequence, 4000d, 3, 500d, 1d, TimedLightCommand.LightDistributions.LastLightAtEnd);
        }
    }
}
