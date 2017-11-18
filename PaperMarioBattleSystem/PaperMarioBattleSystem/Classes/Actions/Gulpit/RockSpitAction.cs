using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Gulpit's Rock Spit move. It eats a usable BattleEntity (such as a Gulpit's Rock) and spits it out at an enemy.
    /// <para>The BattleEntity used in the attack dies and is removed from battle.</para>
    /// </summary>
    public sealed class RockSpitAction : MoveAction
    {
        public RockSpitAction(BattleEntity entityUsed)
        {
            Name = "Rock Spit";

            //Gulpits shouldn't use this move to begin with if there are no usable entities available, so this must be valid
            IUsableEntity usableEntity = (IUsableEntity)entityUsed;

            //NOTE: As an idea, this can perhaps add the charge of the usable entity to the damage; something to consider
            MoveInfo = new MoveActionData(null, "Spit a rock at an enemy.", MoveResourceTypes.FP, 0, CostDisplayTypes.Shown,
                MoveAffectionTypes.Enemy, TargetSelectionMenu.EntitySelectionType.Single, true, null);
            DamageInfo = new DamageData(usableEntity.UsableValue, Elements.Normal, false, ContactTypes.None, null, false, false, DefensiveActionTypes.None, DamageEffects.None);

            SetMoveSequence(new RockSpitSequence(this, entityUsed));
        }
    }
}
