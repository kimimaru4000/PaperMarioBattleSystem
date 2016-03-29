using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles all time
    /// </summary>
    public static class Time
    {
        private static TimeSpan TotalTime = default(TimeSpan);
        private static double ActiveElapsedTime = 0d;

        /// <summary>
        /// Whether in-game time is enabled or not. If set to false, ActiveMilliseconds won't be updated
        /// </summary>
        public static bool TimeEnabled { get; private set; } = true;

        /// <summary>
        /// The total amount of time, in milliseconds, since the game booted up
        /// </summary>
        public static double TotalMilliseconds => TotalTime.TotalMilliseconds;

        /// <summary>
        /// The total amount of unpaused or unfrozen time, in milliseconds, since the game booted up
        /// </summary>
        public static double ActiveMilliseconds => ActiveElapsedTime;

        /// <summary>
        /// Enables time
        /// </summary>
        public static void EnableTime()
        {
            TimeEnabled = true;
        }

        /// <summary>
        /// Disables time
        /// </summary>
        public static void DisableTime()
        {
            TimeEnabled = false;
        }

        /// <summary>
        /// Updates the game time
        /// </summary>
        /// <param name="gameTime">Provides a snapshop of timing values</param>
        public static void UpdateTime(GameTime gameTime)
        {
            TotalTime = gameTime.TotalGameTime;

            if (TimeEnabled == true)
                ActiveElapsedTime += gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }
}
