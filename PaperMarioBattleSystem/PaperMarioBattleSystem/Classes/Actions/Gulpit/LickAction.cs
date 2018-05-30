using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Gulpit's Lick move. It performs this if no Gulpits' Rocks are available.
    /// </summary>
    public sealed class LickAction : MoveAction
    {
        public LickAction(BattleEntity user) : base(user)
        {
            Name = "Lick";
            MoveInfo = new MoveActionData(null, "Lick the enemy.", Enumerations.MoveResourceTypes.FP, 0, Enumerations.CostDisplayTypes.Shown,
                Enumerations.MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.Single, true,
                new Enumerations.HeightStates[] { Enumerations.HeightStates.Grounded }, User.GetOpposingEntityType());

            DamageInfo = new DamageData(0, Enumerations.Elements.Normal, false, Enumerations.ContactTypes.SideDirect, Enumerations.ContactProperties.None, null, false, true,
                Enumerations.DefensiveActionTypes.None, Enumerations.DamageEffects.None);

            SetMoveSequence(new LickSequence(this));
        }
    }
}
