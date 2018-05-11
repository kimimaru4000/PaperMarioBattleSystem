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
    /// Yoshi's Gulp action.
    /// Yoshi eats one enemy and spits it out at the enemy behind it. This move is the only one that can hurt Iron Clefts.
    /// </summary>
    public sealed class GulpAction : MoveAction
    {
        public GulpAction()
        {
            Name = "Gulp";

            MoveInfo = new MoveActionData(new CroppedTexture2D(AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png"), new Rectangle(874, 14, 22, 22)),
                string.Empty, Enumerations.MoveResourceTypes.FP, 4, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.First, true,
                new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded, Enumerations.HeightStates.Hovering }, User.GetOpposingEntityType());

            DamageInfo = new DamageData(4, Enumerations.Elements.Gulp, true, Enumerations.ContactTypes.SideDirect, Enumerations.ContactProperties.WeaponDirect, null,
                Enumerations.DamageEffects.None);

            GulpSequence gulpSequence = new GulpSequence(this);
            SetMoveSequence(gulpSequence);
            actionCommand = new GulpCommand(MoveSequence, gulpSequence.WalkDuration / 2f, 500f, 1f, Microsoft.Xna.Framework.Input.Keys.R);
        }
    }
}
