using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global utility functions
    /// </summary>
    public static class UtilityGlobals
    {
        public static int Clamp(int value, int min, int max) => (value < min) ? min : (value > max) ? max : value;
        public static float Clamp(float value, float min, float max) => (value < min) ? min : (value > max) ? max : value;
        public static double Clamp(double value, double min, double max) => (value < min) ? min : (value > max) ? max : value;
        public static uint Clamp(uint value, uint min, uint max) => (value < min) ? min : (value > max) ? max : value;

        public static int Wrap(int value, int min, int max) => (value < min) ? max : (value > max) ? min : value;
        public static float Wrap(float value, float min, float max) => (value < min) ? max : (value > max) ? min : value;
        public static double Wrap(double value, double min, double max) => (value < min) ? max : (value > max) ? min : value;

        public static T Min<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) < 0) ? val1 : (val2.CompareTo(val1) < 0) ? val2 : val1;
        public static T Max<T>(T val1, T val2) where T : IComparable => (val1.CompareTo(val2) > 0) ? val1 : (val2.CompareTo(val1) > 0) ? val2 : val1;

        public static float ToDegrees(float radians) => Microsoft.Xna.Framework.MathHelper.ToDegrees(radians);
        public static float ToRadians(float degrees) => Microsoft.Xna.Framework.MathHelper.ToRadians(degrees);

        public static double ToDegrees(double radians) => (radians * (180d / Math.PI));
        public static double ToRadians(double degrees) => (degrees * (Math.PI / 180d));

        /// <summary>
        /// Tests a random condition.
        /// </summary>
        /// <param name="minValue">The minimum possible value</param>
        /// <param name="maxValue">The maximum possible value</param>
        /// <param name="valueTested">The value to test against</param>
        /// <param name="checkEquals">If true, will also check if the randomized value matches the value tested</param>
        /// <returns>true if the condition succeeded, false otherwise</returns>
        //NOTE: (Leaving this here for now)
        //TTYD checks rand(100) < enemy susceptibility for a given value, such as chance of being inflicted with Dizzy
        //Clock Out and Showstopper are a bit different:
        //Clock Out has a 1x multiplier if the bar is filled at all and a 1.27x multiplier if the bar is full
        //Showstopper has a .5x multiplier that's increased by .1x for each successful button set, totaling a 1x multiplier
        //These multipliers are multiplied by the random value to increase or decrease the chances of the condition evaluating to true

        public static bool TestRandomCondition(int minValue, int maxValue, int valueTested, bool checkEquals)
        {
            int value = GeneralGlobals.Randomizer.Next(minValue, maxValue);

            return (checkEquals == false) ? (value < valueTested) : (value <= valueTested);
        }

        /// <summary>
        /// Chooses a random index in a list of percentages
        /// </summary>
        /// <param name="percentages">The container of percentages, each with positive values, with the sum adding up to 1</param>
        /// <returns>The index in the container of percentages that was chosen</returns>
        public static int ChoosePercentage(IList<double> percentages)
        {
            double randomVal = GeneralGlobals.Randomizer.NextDouble();
            double value = 0d;

            for (int i = 0; i < percentages.Count; i++)
            {
                value += percentages[i];
                if (value > randomVal)
                {
                    return i;
                }
            }

            //Return the last one if it goes through
            return percentages.Count - 1;
        }

        public static T[] GetEnumValues<T>()
        {
            return (T[])Enum.GetValues(typeof(T));
        }
    }
}
