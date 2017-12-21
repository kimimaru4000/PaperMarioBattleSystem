using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Bobbery's Bomb Squad move. Throw 3 Bobbery Bombs on the field, which will explode after a couple of turns to damage enemies.
    /// </summary>
    public sealed class BombSquadAction : MoveAction
    {
        /// <summary>
        /// The number of bombs to throw.
        /// </summary>
        private int BombCount = 3;

        /// <summary>
        /// The amount of damage each bomb deals.
        /// </summary>
        private int BombDamage = 3;

        public BombSquadAction(int bombCount, int bombDamage)
        {
            Name = "Bomb Squad";

            BombCount = bombCount;
            BombDamage = bombDamage;

            MoveInfo = new MoveActionData(null, "Throw three bombs that\nwill explode one turn later.", MoveResourceTypes.FP, 3,
                CostDisplayTypes.Shown, MoveAffectionTypes.Other, TargetSelectionMenu.EntitySelectionType.All, false,
                new HeightStates[] { HeightStates.Grounded, HeightStates.Hovering, HeightStates.Airborne }, User.GetOpposingEntityType());

            DamageInfo = new DamageData(BombDamage, Elements.Explosion, false, ContactTypes.None, ContactProperties.Ranged, null, DamageEffects.None);

            SetMoveSequence(new BombSquadSequence(this, BombCount));

            actionCommand = new BombSquadCommand(MoveSequence, BombCount);
        }
    }
}
