﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PaperMarioBattleSystem.StarPowerGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Special Moves, which use up Star Power.
    /// </summary>
    public class SpecialMoveAction : MoveAction
    {
        /// <summary>
        /// The type of Star Power this Special Move uses.
        /// </summary>
        public StarPowerTypes SPType { get; protected set; } = StarPowerTypes.StarSpirit;

        /// <summary>
        /// The amount of Star Power the Special Move costs.
        /// </summary>
        public float SPCost { get; protected set; } = 0f;

        public override bool CostsSP => (SPCost > 0f);

        protected SpecialMoveAction()
        {

        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, HealingData healingData, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, healingData)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, HealingData healingData, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, actionCommand, healingData)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, InteractionParamHolder damageInfo, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, damageInfo)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public SpecialMoveAction(string name, MoveActionData moveProperties, Sequence moveSequence, ActionCommand actionCommand, InteractionParamHolder damageInfo, StarPowerTypes spType, float spCost) : base(name, moveProperties, moveSequence, actionCommand, damageInfo)
        {
            SPType = spType;
            SPCost = spCost;
        }

        public override void Initialize()
        {
            float starPower = 0f;

            //Check if the entity has enough SP to perform this Special Move
            //Only Mario has Star Power to perform Special Moves
            MarioStats marioStats = User.BattleStats as MarioStats;

            if (marioStats != null)
            {
                starPower = marioStats.GetStarPowerFromType(SPType).SPU;
            }

            //If the entity has insufficient SP to use this move, disable it
            if (SPCost > starPower)
            {
                Disabled = true;
                DisabledString = "Not enough Star Power.";
                return;
            }

            //Check base aspects to determine whether the move should be disabled or not
            base.Initialize();
        }
    }
}