using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The base class for the two different types of Star Power found in the Paper Mario games (Star Spirits, Crystal Stars).
    /// </summary>
    public abstract class StarPowerBase
    {
        //Each usable Star Power circle/bar is represented as 100.
        //So in PM, a full bar of 1 Star Power is 100 and in TTYD one filled circle is 100

        /// <summary>
        /// The current amount of Star Power units Mario has.
        /// <para>100 is equivalent to one bar/circle of Star Power in PM/TTYD.</para>
        /// </summary>
        public float SPU { get; protected set; } = 0;

        /// <summary>
        /// The max Star Power units Mario has.
        /// </summary>
        public float MaxSPU { get; protected set; } = 700f;

        /// <summary>
        /// The type of Star Power this is.
        /// </summary>
        public StarPowerGlobals.StarPowerTypes StarPowerType { get; protected set; } = StarPowerGlobals.StarPowerTypes.StarSpirit;

        protected StarPowerBase()
        {
            
        }

        /// <summary>
        /// Tells if Mario has enough Star Power to use a special move.
        /// </summary>
        /// <param name="spuCost">The cost of the Star Power, in Star Power units.</param>
        /// <returns>true if the current SPU is greater than or equal to the cost, otherwise false.</returns>
        public bool CanUseStarPower(float spuCost) => (SPU >= spuCost);

        /// <summary>
        /// Increases the current number of Star Power units Mario has.
        /// </summary>
        /// <param name="spuGained">The number of Star Power units to add. If negative, this value will be changed to positive.</param>
        public void GainStarPower(float spuGained)
        {
            if (spuGained <= 0)
            {
                Debug.LogError($"{nameof(spuGained)} is less than or equal to 0, which should never happen. Changing to positive");
                spuGained = -spuGained;
            }

            SPU = UtilityGlobals.Clamp(SPU + spuGained, 0f, MaxSPU);

            Debug.Log($"Gained {spuGained} SPU for {StarPowerType} Star Power! Total SPU: {SPU}");
        }

        /// <summary>
        /// Decreases the current number of Star Power units Mario has.
        /// </summary>
        /// <param name="spuLost">The number of Star Power units to subtract. If negative, this value will be changed to positive.</param>
        public void LoseStarPower(float spuLost)
        {
            if (spuLost <= 0)
            {
                Debug.LogError($"{nameof(spuLost)} is less than or equal to 0, which should never happen. Changing to negative");
                spuLost = -spuLost;
            }

            SPU = UtilityGlobals.Clamp(SPU - spuLost, 0f, MaxSPU);

            Debug.Log($"Lost {spuLost} SPU for {StarPowerType} Star Power! Total SPU: {SPU}");
        }
    }
}
