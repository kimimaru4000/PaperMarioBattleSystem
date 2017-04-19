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
        /// <summary>
        /// The number of milliseconds in a second.
        /// </summary>
        public const double MsPerS = 1000d;

        private static TimeSpan TotalTime = default(TimeSpan);
        private static TimeSpan ElapsedTime = default(TimeSpan);
        private static double ActiveElapsedTime = 0d;

        /// <summary>
        /// The frames per second the game runs at
        /// </summary>
        public static double FPS = 60d;

        /// <summary>
        /// Whether the game's frame rate is updated at the end of each frame or not
        /// </summary>
        public static bool UpdateFPS { get; private set; } = false;

        /// <summary>
        /// Whether in-game time is enabled or not. If set to false, ActiveMilliseconds won't be updated
        /// </summary>
        public static bool InGameTimeEnabled { get; private set; } = true;

        /// <summary>
        /// The total amount of time, in milliseconds, since the game booted up
        /// </summary>
        public static double TotalMilliseconds => TotalTime.TotalMilliseconds;

        /// <summary>
        /// The total amount of unpaused or unfrozen time, in milliseconds, since the game booted up
        /// </summary>
        public static double ActiveMilliseconds => ActiveElapsedTime;

        /// <summary>
        /// The amount of time since the previous frame
        /// </summary>
        public static double ElapsedMilliseconds => ElapsedTime.TotalMilliseconds;

        /// <summary>
        /// Determines if the game is running slowly or not
        /// </summary>
        public static bool RunningSlowly { get; private set; } = false;

        /// <summary>
        /// Enables or disables in-game time. If false, <see cref="ActiveMilliseconds"/> will not be updated.
        /// </summary>
        /// <param name="ingameTimeEnabled"></param>
        public static void ToggleInGameTime(bool ingameTimeEnabled)
        {
            InGameTimeEnabled = ingameTimeEnabled;
        }

        /// <summary>
        /// Enables or disables updating the game's framerate at the end of each frame
        /// </summary>
        /// <param name="enableUpdateFPS">true to enable updating the FPS, false to disable it</param>
        public static void ToggleFPSUpdate(bool enableUpdateFPS)
        {
            UpdateFPS = enableUpdateFPS;
        }

        /// <summary>
        /// Updates the game time
        /// </summary>
        /// <param name="gameTime">Provides a snapshop of timing values</param>
        public static void UpdateTime(GameTime gameTime)
        {
            TotalTime = gameTime.TotalGameTime;
            ElapsedTime = gameTime.ElapsedGameTime;
            RunningSlowly = gameTime.IsRunningSlowly;

            if (InGameTimeEnabled == true)
                ActiveElapsedTime += ElapsedTime.TotalMilliseconds;
        }
    }
}
