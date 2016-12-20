using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Star Power granted by Crystal Stars that can be used by Mario.
    /// This is the TTYD Star Power. This Star Power is gained from your battle performance, as determined by the Audience.
    /// </summary>
    public sealed class CrystalStarPower : StarPowerBase
    {
        /// <summary>
        /// Calculates the amount of Star Power the Audience gives Mario after either he or his Partner use Appeal.
        /// </summary>
        /// <param name="activeAudienceMembers">The number of active Audience members.</param>
        /// <param name="superAppealCount">The number of Super Appeal or Super Appeal P Badges equipped, depending on who's using Appeal.</param>
        /// <returns>The total amount of Star Power gained from using Appeal.</returns>
        public float CalculateAppealStarPower(int activeAudienceMembers, int superAppealCount)
        {
            int appealValue = 25 * (superAppealCount + 1);
            int audienceOverFour = activeAudienceMembers / 4;

            //Cap the amount of SPU you can gain to be your max SPU
            float totalSPUGained = UtilityGlobals.Clamp(appealValue + audienceOverFour, 0f, MaxSPU);

            return totalSPUGained;
        }

        /// <summary>
        /// Calculates the amount of Star Power the Audience give Mario after either he or his Partner attacks.
        /// </summary>
        /// <param name="audienceValue">The total value of the Audience.</param>
        /// <param name="commandRankValue">The CommandRank value earned during the attack. This should also factor whether a Stylish move was performed or not.</param>
        /// <param name="dangerStatus">The Danger status value.</param>
        /// <param name="BINGOStatus">The value of the current BINGO! status. 0 for Poison Shrooms, 2 for Mushrooms, Flowers, and Stars, and 3 for Shine Sprites.</param>
        /// <returns>The total amount of Star Power gained from the attack.</returns>
        public float CalculateStarPowerFromAudience(int audienceValue, float commandRankValue, float dangerStatus, float BINGOStatus)
        {
            float audienceSquared = (float)Math.Sqrt((double)audienceValue);

            float value = audienceSquared * commandRankValue * dangerStatus * BINGOStatus;

            //Cap the amount of SPU you can gain to be your max SPU
            float totalSPUGained = UtilityGlobals.Clamp((float)Math.Floor(value), 0f, MaxSPU);

            return totalSPUGained;
        }
    }
}
