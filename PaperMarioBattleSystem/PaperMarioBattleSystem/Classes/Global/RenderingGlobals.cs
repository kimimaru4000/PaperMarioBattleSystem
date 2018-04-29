using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with rendering
    /// </summary>
    public static class RenderingGlobals
    {
        public const int BaseResolutionWidth = 800;
        public const int BaseResolutionHeight = 600;

        private static readonly Vector2 Resolution = new Vector2(BaseResolutionWidth, BaseResolutionHeight);
        private static readonly Vector2 ResolutionHalf = Resolution / 2;

        public static ref readonly Vector2 BaseResolutionVec2 => ref Resolution;
        public static ref readonly Vector2 BaseResolutionHalved => ref ResolutionHalf;

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

        /// <summary>
        /// Gets the current global offset of the Sleep shader's shift.
        /// </summary>
        /// <remarks>We divide by a negative value to make it wave in the opposite direction.</remarks>
        public static float SleepShaderShiftOffset => (float)(Time.ActiveMilliseconds / -184f);
    }
}
