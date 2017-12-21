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
    public sealed class Gulp : MoveAction
    {
        public Gulp()
        {
            Name = "Gulp";

            MoveInfo = new MoveActionData(null, string.Empty, Enumerations.MoveResourceTypes.FP, 4, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.First, true,
                new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded, Enumerations.HeightStates.Hovering }, User.GetOpposingEntityType());

            DamageInfo = new DamageData(4, Enumerations.Elements.Normal, true, Enumerations.ContactTypes.SideDirect, Enumerations.ContactProperties.WeaponDirect, null,
                Enumerations.DamageEffects.None);

            GulpSequence gulpSequence = new GulpSequence(this);
            SetMoveSequence(gulpSequence);
            actionCommand = new GulpCommand(MoveSequence, gulpSequence.WalkDuration / 2f, 500f, 1f);
        }
    }
}
