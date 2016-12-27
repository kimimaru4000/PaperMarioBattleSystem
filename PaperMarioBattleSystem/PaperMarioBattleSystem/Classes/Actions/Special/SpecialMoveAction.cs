using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for Special Moves, which use up Star Power.
    /// </summary>
    public abstract class SpecialMoveAction : MoveAction
    {
        /// <summary>
        /// The type of Star Power this Special Move uses.
        /// </summary>
        public StarPowerGlobals.StarPowerTypes SPType { get; protected set; } = StarPowerGlobals.StarPowerTypes.StarSpirit;

        /// <summary>
        /// The amount of Star Power the Special Move costs.
        /// </summary>
        public float SPCost { get; protected set; } = 0f;

        public override bool CostsSP => (SPCost > 0f);

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
