using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with rendering
    /// </summary>
    public static class RenderingGlobals
    {
        public const int WindowWidth = 800;
        public const int WindowHeight = 600;

        /// <summary>
        /// Gets the current global offset the Charge shader's texture should have.
        /// </summary>
        /// <returns></returns>
        public static float ChargeShaderTexOffset => (((float)Time.ActiveMilliseconds % 1000f) / 1000f);

        /// <summary>
        /// Gets the current global alpha value the Charge shader should have.
        /// </summary>
        public static float ChargeShaderAlphaVal
        {
            get
            {
                return (UtilityGlobals.PingPong(Time.ActiveMilliseconds / 1000f, .9f));
            }
        }
    }
}
