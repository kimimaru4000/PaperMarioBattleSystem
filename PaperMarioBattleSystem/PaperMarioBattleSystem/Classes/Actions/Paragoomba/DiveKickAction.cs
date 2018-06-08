using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaperMarioBattleSystem.Extensions;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A Paragoomba's Dive Kick attack.
    /// </summary>
    public class DiveKickAction : MoveAction
    {
        public DiveKickAction(BattleEntity user) : base(user)
        {
            Name = "Dive Kick";

            MoveInfo = new MoveActionData(null, "Dive Kick a foe.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, Enumerations.EntitySelectionType.Single, true, null, User.GetOpposingEntityType());

            DamageInfo = new DamageData(1, Elements.Normal, false, ContactTypes.SideDirect, ContactProperties.None, null, DamageEffects.None);

            SetMoveSequence(new DiveKickSequence(this));

            //It's an Enemy move, so there's no Action Command
            //However one can be added if the player were to have a Paragoomba partner
            actionCommand = null;
        }
    }
}
