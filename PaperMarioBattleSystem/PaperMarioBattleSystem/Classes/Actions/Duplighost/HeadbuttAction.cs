using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Duplighost's Headbutt move.
    /// </summary>
    public sealed class HeadbuttAction : MoveAction
    {
        public HeadbuttAction(BattleEntity user) : base(user)
        {
            Name = "Headbutt";

            MoveInfo = new MoveActionData(null, "Headbutt into the enemy.", MoveResourceTypes.FP, 0f, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.Single, true, null, User.GetOpposingEntityType());

            DamageInfo = new DamageData(4, Elements.Normal, false, ContactTypes.SideDirect, ContactProperties.None, null, false, true,
                DefensiveActionTypes.None, DamageEffects.None);

            SetMoveSequence(new HeadbuttSequence(this));
            actionCommand = null;
        }
    }
}
