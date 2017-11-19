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
    /// Huff N. Puff's Wind Breath action.
    /// It deals 2-7 damage to Mario depending on how well the player performed the Action Command to reduce the damage it deals.
    /// </summary>
    public sealed class WindBreathAction : MoveAction
    {
        public override bool CommandEnabled => (HasActionCommand == true && DisableActionCommand == false);

        public WindBreathAction(int minDamage, int maxDamage)
        {
            Name = "Wind Breath";

            MoveInfo = new MoveActionData(null, "Blow wind at Mario to deal damage.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.Single, true, null, User.GetOpposingEntityType());

            DamageInfo = new DamageData(0, Elements.Normal, false, ContactTypes.None, null, false, false,
                DefensiveActionTypes.Guard | DefensiveActionTypes.Superguard, DamageEffects.None);

            SetMoveSequence(new WindBreathSequence(this));
            actionCommand = new MashButtonRangeCommand(MoveSequence, 100d, 10d, 7000d, Keys.Z, .3d, maxDamage, minDamage);
        }
    }
}
