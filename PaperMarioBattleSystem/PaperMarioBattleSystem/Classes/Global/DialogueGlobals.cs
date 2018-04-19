using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using HtmlAgilityPack;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Global values relating to dialogue.
    /// </summary>
    public static class DialogueGlobals
    {
        #region Text Modifiers

        /// <summary>
        /// The string representing the color text modifier.
        /// </summary>
        public const string ColorMod = "color";

        /// <summary>
        /// The string representing the dynamic text modifier.
        /// </summary>
        public const string DynamicMod = "dynamic";

        /// <summary>
        /// The string representing the shake text modifier.
        /// </summary>
        public const string ShakeMod = "shake";

        /// <summary>
        /// The string representing the wave text modifier.
        /// </summary>
        public const string WaveMod = "wave";

        /// <summary>
        /// Tells if the string is the color text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsColorMod(in string text)
        {
            return (text == ColorMod);
        }

        /// <summary>
        /// Tells if the string is the dynamic text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsDynamicMod(in string text)
        {
            return (text == DynamicMod);
        }

        /// <summary>
        /// Tells if the string is the shake text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsShakeMod(in string text)
        {
            return (text == ShakeMod);
        }

        /// <summary>
        /// Tells if the string is the wave text modifier.
        /// </summary>
        /// <param name="text">The string to test.</param>
        /// <returns>true if so, otherwise false.</returns>
        public static bool IsWaveMod(in string text)
        {
            return (text == WaveMod);
        }

        #endregion

        #region Classes

        /// <summary>
        /// Represents Text Modifier data for a Dialogue Bubble.
        /// </summary>
        public class BubbleTextData
        {
            public int StartIndex = 0;
            public int EndIndex = 0;
            public Color color = Color.Black;
            public float DynamicSize = 1f;
            public bool Shake = false;
            public bool Wave = false;
        }

        #endregion
    }
}
