using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with StarPower.
    /// </summary>
    public static class StarPowerGlobals
    {
        #region Enums

        /// <summary>
        /// The types of Star Power.
        /// <para>PM has the Star Spirits, and TTYD has the Crystal Stars.</para>
        /// </summary>
        public enum StarPowerTypes
        {
            None, StarSpirit, CrystalStar
        }

        #endregion

        #region Command Rank Data

        /// <summary>
        /// The table of Crystal Star Power modifiers based on the highest CommandRank earned.
        /// </summary>
        private static readonly Dictionary<ActionCommand.CommandRank, float> CommandRankModifierTable = new Dictionary<ActionCommand.CommandRank, float>()
        {
            { ActionCommand.CommandRank.None, 0.00f },
            { ActionCommand.CommandRank.NiceM2, 0.50f },
            { ActionCommand.CommandRank.NiceM1, 0.75f },
            { ActionCommand.CommandRank.Nice, 1.00f },
            { ActionCommand.CommandRank.Good, 1.25f },
            { ActionCommand.CommandRank.Great, 1.50f },
            { ActionCommand.CommandRank.Wonderful, 1.75f },
            { ActionCommand.CommandRank.Excellent, 2.00f }
        };

        /// <summary>
        /// The table of Crystal Star Power modifiers based on whether any Stylish moves were performed or not for a particular CommandRank.
        /// </summary>
        private static readonly Dictionary<ActionCommand.CommandRank, float> StylishModifierTable = new Dictionary<ActionCommand.CommandRank, float>()
        {
            { ActionCommand.CommandRank.None, 1.00f },
            { ActionCommand.CommandRank.NiceM2, 3.00f },
            { ActionCommand.CommandRank.NiceM1, 3.50f },
            { ActionCommand.CommandRank.Nice, 4.00f },
            { ActionCommand.CommandRank.Good, 4.50f },
            { ActionCommand.CommandRank.Great, 5.00f },
            { ActionCommand.CommandRank.Wonderful, 5.50f },
            { ActionCommand.CommandRank.Excellent, 6.00f }
        };

        /// <summary>
        /// Gets the total CommandRank value based on how well Mario or his Partner performed an Action Command.
        /// This is factored in when calculating the amount of Crystal Star Star Power gained from an attack.
        /// </summary>
        /// <param name="highestRank">The highest CommandRank earned while performing the Action Command.</param>
        /// <param name="performedStylish">Whether any Stylish moves were performed or not.</param>
        /// <returns>A float of the CommandRank value.</returns>
        public static float GetCommandRankValue(ActionCommand.CommandRank highestRank, bool performedStylish)
        {
            if (performedStylish == true) return StylishModifierTable[highestRank];
            else return CommandRankModifierTable[highestRank];
        }

        #endregion

        #region Danger Status Values

        public const float NormalMod = 1f;

        public const float MarioDangerMod = 2f;
        public const float MarioPerilMod = 3f;

        public const float PartnerDangerMod = 1.5f;
        public const float PartnerPerilMod = 2f;

        /// <summary>
        /// Gets Mario's Danger status value based on his current HealthState.
        /// </summary>
        /// <param name="entity">A BattleEntity representing Mario.</param>
        /// <returns>A float of Mario's Danger status value.</returns>
        private static float GetMarioDangerStatusValue(BattleEntity entity)
        {
            Enumerations.HealthStates? marioHealthState = entity?.HealthState;

            switch (marioHealthState)
            {
                case Enumerations.HealthStates.Danger:
                    return MarioDangerMod;
                case Enumerations.HealthStates.Peril:
                case Enumerations.HealthStates.Dead:
                    return MarioPerilMod;
                case Enumerations.HealthStates.Normal:
                default:
                    return NormalMod;
            }
        }

        /// <summary>
        /// Gets a Partner's Danger status value based on its current HealthState.
        /// </summary>
        /// <param name="entity">A BattleEntity representing Mario's Partner.</param>
        /// <returns>A float of the Partner's Danger status value.</returns>
        private static float GetPartnerDangerStatusValue(BattleEntity entity)
        {
            Enumerations.HealthStates? partnerHealthState = entity?.HealthState;

            switch (partnerHealthState)
            {
                case Enumerations.HealthStates.Danger:
                    return PartnerDangerMod;
                case Enumerations.HealthStates.Peril:
                case Enumerations.HealthStates.Dead:
                    return PartnerPerilMod;
                case Enumerations.HealthStates.Normal:
                default:
                    return NormalMod;
            }
        }

        /// <summary>
        /// Gets the total Danger status value for Mario and his Partner based on their HealthStates.
        /// This is factored in when calculating the amount of Crystal Star Star Power gained from an attack.
        /// </summary>
        /// <param name="mario">A BattleEntity representing Mario.</param>
        /// <param name="partner">A BattleEntity representing Mario's Partner.</param>
        /// <returns>A float of the Danger status value based on the HealthStates of both Mario and his Partner.</returns>
        public static float GetDangerStatusValue(BattleEntity mario, BattleEntity partner)
        {
            float marioDangerStatusValue = GetMarioDangerStatusValue(mario);
            float partnerDangerStatusValue = GetPartnerDangerStatusValue(partner);

            return (marioDangerStatusValue * partnerDangerStatusValue);
        }

        #endregion

        #region Constants

        /// <summary>
        /// The amount of Star Power Units (SPU) per usable Star Power (how much SPU each full bar/circle equates to).
        /// </summary>
        public const float SPUPerStarPower = 100f;

        /// <summary>
        /// The amount of Star Spirit Star Power the Focus move gives.
        /// </summary>
        public const float FocusSPUGain = SPUPerStarPower / 2f;

        /// <summary>
        /// The amount of additional Star Spirit Star Power each Deep Focus Badge gives to Focus.
        /// </summary>
        public const float DeepFocusSPUIncrease = SPUPerStarPower / 4f;

        /// <summary>
        /// The amount of Star Spirit Star Power Mario gains each turn.
        /// </summary>
        public const float StarSpiritSPUPerTurn = SPUPerStarPower / 8f;

        #endregion
    }
}
