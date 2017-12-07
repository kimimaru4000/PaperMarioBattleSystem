using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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

        public static int Lerp(int value1, int value2, float amount) => value1 + (int)((value2 - value1) * amount);
        public static double Lerp(double value1, double value2, float amount) => value1 + ((value2 - value1) * amount);

        public static double LerpPrecise(double value1, double value2, float amount) => ((1 - amount) * value1) + (value2 * amount);
        public static int LerpPrecise(int value1, int value2, float amount) => (int)(((1 - amount) * value1) + (value2 * amount));

        /// <summary>
        /// Swaps two references of the same Type.
        /// </summary>
        /// <typeparam name="T">The Type of the objects to swap.</typeparam>
        /// <param name="obj1">The first object to swap.</param>
        /// <param name="obj2">The second object to swap.</param>
        public static void Swap<T>(ref T obj1, ref T obj2)
        {
            T temp = obj1;
            obj1 = obj2;
            obj2 = temp;
        }

        /// <summary>
        /// Gets the tangent angle between two Vector2s in radians. This value is between -π and π. 
        /// </summary>
        /// <param name="vec1">The first vector2.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the tangent angle between the two vectors, in radians.</returns>
        public static double TangentAngle(Vector2 vec1, Vector2 vec2) => Math.Atan2(vec2.Y - vec1.Y, vec2.X - vec1.X);

        /// <summary>
        /// Gets the cosign angle between two Vector2s in radians.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the cosign angle between the two vectors, in radians.</returns>
        public static double CosignAngle(Vector2 vec1, Vector2 vec2)
        {
            //a · b = (a.X * b.X) + (a.Y * b.Y) = ||a|| * ||b|| * cos(θ)
            double dotProduct = Vector2.Dot(vec1, vec2);
            double mag1 = vec1.Length();
            double mag2 = vec2.Length();

            double magMult = mag1 * mag2;

            double div = dotProduct / magMult;

            double angleRadians = Math.Acos(div);
            
            return angleRadians;
        }

        /// <summary>
        /// Gets the cosign angle between two Vector2s in degrees.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the cosign angle between the two vectors, in degrees.</returns>
        public static double CosignAngleDegrees(Vector2 vec1, Vector2 vec2) => ToDegrees(CosignAngle(vec1, vec2));

        /// <summary>
        /// Obtains the 2D cross product result of two Vector2s.
        /// </summary>
        /// <param name="vector1">The first vector.</param>
        /// <param name="vector2">The second vector.</param>
        /// <returns>A float representing the 2D cross product result between the two Vectors.</returns>
        public static double Cross(Vector2 vector1, Vector2 vector2)
        {
            //a x b = ((a.y * b.z) - (a.z * b.y), (a.z * b.x) - (a.x * b.z), (a.x * b.y) - (a.y * b.x))
            //The Z component is the only one that remains since we're dealing with Vector2s
            return (vector1.X * vector2.Y) - (vector1.Y * vector2.X);
        }

        /// <summary>
        /// Gets the sine angle between two Vector2s in radians.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A double representing the sine angle between the two vectors, in radians.</returns>
        public static double SineAngle(Vector2 vec1, Vector2 vec2)
        {
            //||a x b|| = ||a|| * ||b|| * sin(θ)
            double crossMag = Cross(vec1, vec2);

            double mag1 = vec1.Length();
            double mag2 = vec2.Length();

            double magMult = mag1 * mag2;

            double div = crossMag / magMult;

            double angleRadians = Math.Asin(div);

            return angleRadians;
        }

        /// <summary>
        /// Finds a point around a circle at a particular angle.
        /// </summary>
        /// <param name="center">The center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="angle">The angle of the point.</param>
        /// <param name="angleInDegrees">Whether the angle passed in is in degrees or not.</param>
        /// <returns>A Vector2 with the X and Y components at the location around the circle.</returns>
        public static Vector2 GetPointAroundCircle(Vector2 center, double radius, double angle, bool angleInDegrees)
        {
            //If the angle is in degrees, convert it to radians
            if (angleInDegrees == true)
            {
                angle = ToRadians(angle);
            }

            float x = (float)(Math.Cos(angle) * radius) + center.X;
            float y = (float)(Math.Sin(angle) * radius) + center.Y;

            return new Vector2(x, y);
        }

        /// <summary>
        /// Tells if two circles intersect.
        /// </summary>
        /// <param name="circle1">The point of the first circle.</param>
        /// <param name="radius1">The radius of the first circle.</param>
        /// <param name="circle2">The point of the second circle.</param>
        /// <param name="radius2">The radius of the second circle.</param>
        /// <returns>true if the sum of the radii squared is less than or equal to the distance between the circles squared.</returns>
        public static bool CircleCircleIntersection(Vector2 circle1, float radius1, Vector2 circle2, float radius2)
        {
            float radiusSquared = (float)Math.Pow(radius1 + radius2, 2);
            float distance = Vector2.DistanceSquared(circle1, circle2);

            return (distance <= radiusSquared);
        }

        //NOTE: (Leaving this here for now)
        //TTYD checks rand(100) < enemy susceptibility for a given value, such as chance of being inflicted with Dizzy
        //Clock Out and Showstopper are a bit different:
        //Clock Out has a 1x multiplier if the bar is filled at all and a 1.27x multiplier if the bar is full
        //Showstopper has a .5x multiplier that's increased by .1x for each successful button set, totaling a 1x multiplier
        //These multipliers are multiplied by the random value to increase or decrease the chances of the condition evaluating to true
        /// <summary>
        /// Tests a random condition with two values.
        /// This is commonly used when calculating a total percentage of something happening.
        /// For example, this is used when testing whether a move will inflict a Status Effect on a BattleEntity.
        /// <para>Two values are multiplied by each other then divided by <see cref="GeneralGlobals.RandomConditionVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(double value1, double value2)
        {
            double value = GeneralGlobals.GenerateRandomDouble();

            double percentageResult = ((value1 * value2) / (double)GeneralGlobals.RandomConditionVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with one value.
        /// </summary>
        /// <param name="value">The value to test, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(double value)
        {
            return TestRandomCondition(value, (double)GeneralGlobals.RandomConditionVal);
        }

        /// <summary>
        /// Tests a random condition with two values. An int overload.
        /// This is commonly used when calculating a total percentage of something happening.
        /// For example, this is used when testing whether a move will inflict a Status Effect on a BattleEntity.
        /// <para>Two values are multiplied by each other then divided by <see cref="GeneralGlobals.RandomConditionVal"/>.
        /// A random value is then rolled; if it's less than the result, it returns true. This works for any non-negative values.</para>
        /// </summary>
        /// <param name="value1">The first value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <param name="value2">The second value to test with, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(int value1, int value2)
        {
            int value = GeneralGlobals.GenerateRandomInt();

            int percentageResult = ((value1 * value2) / GeneralGlobals.RandomConditionVal);

            return (value < percentageResult);
        }

        /// <summary>
        /// Tests a random condition with one value. An int overload.
        /// </summary>
        /// <param name="value">The value to test, representing a percentage with a number from 0 to 100+.</param>
        /// <returns>true if the RNG value is less than a calculated percentage result, otherwise false.</returns>
        public static bool TestRandomCondition(int value)
        {
            return TestRandomCondition(value, GeneralGlobals.RandomConditionVal);
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

        #region Flag Check Utilities

        /* Adding flags: flag1 |= flag2            ; 10 | 01 = 11
         * Checking flags: (flag1 & flag2) != 0    ; 11 & 10 = 10
         * Removing flags: (flag1 & (~flag2))      ; 1111 & (~0010) = 1111 & 1101 = 1101
         * */

        /// <summary>
        /// Tells whether a set of DamageEffects has any of the flags in another DamageEffects set.
        /// </summary>
        /// <param name="damageEffects">The DamageEffects value.</param>
        /// <param name="damageEffectFlags">The flags to test.</param>
        /// <returns>true if any of the flags in damageEffectFlags are in damageEffects, otherwise false.</returns>
        public static bool DamageEffectHasFlag(Enumerations.DamageEffects damageEffects, Enumerations.DamageEffects damageEffectFlags)
        {
            Enumerations.DamageEffects flags = (damageEffects & damageEffectFlags);

            return (flags != 0);
        }

        /// <summary>
        /// Tells whether a set of DefensiveActionTypes has any of the flags in another DefensiveActionTypes set.
        /// </summary>
        /// <param name="defensiveOverrides">The DefensiveActionTypes value.</param>
        /// <param name="defensiveOverrideFlags">The flags to test.</param>
        /// <returns>true if any of the flags in defensiveOverrides are in defensiveOverrideFlags, otherwise false.</returns>
        public static bool DefensiveActionTypesHasFlag(Enumerations.DefensiveActionTypes defensiveOverrides,
            Enumerations.DefensiveActionTypes defensiveOverrideFlags)
        {
            Enumerations.DefensiveActionTypes flags = (defensiveOverrides & defensiveOverrideFlags);

            return (flags != 0);
        }

        /// <summary>
        /// Tells whether a set of MoveAffectionTypes has any of the flags in another MoveAffectionTypes set.
        /// </summary>
        /// <param name="moveAffectionTypes">The MoveAffectionTypes value.</param>
        /// <param name="moveAffectionTypesFlags">The flags to test.</param>
        /// <returns>true if any of the flags in moveAffectionTypes are in moveAffectionTypesFlags, otherwise false.</returns>
        public static bool MoveAffectionTypesHasFlag(Enumerations.MoveAffectionTypes moveAffectionTypes,
            Enumerations.MoveAffectionTypes moveAffectionTypesFlags)
        {
            Enumerations.MoveAffectionTypes flags = (moveAffectionTypes & moveAffectionTypesFlags);

            return (flags != 0);
        }

        public static bool ItemTypesHasFlag(Item.ItemTypes itemTypes, Item.ItemTypes itemTypesFlags)
        {
            Item.ItemTypes flags = (itemTypes & itemTypesFlags);

            return (flags != 0);
        }

        #endregion

        /// <summary>
        /// Initializes a jagged array with default values.
        /// <para>This should be used on null jagged arrays to easily initialize them.</para>
        /// </summary>
        /// <typeparam name="T">The type of the jagged array.</typeparam>
        /// <param name="jaggedArray">The jagged array of type T to initialize.</param>
        /// <param name="columns">The number of columns (first bracket) in the jagged array.</param>
        /// <param name="rows">The number of rows (second bracket) in the jagged array.</param>
        public static void InitializeJaggedArray<T>(ref T[][] jaggedArray, int columns, int rows)
        {
            jaggedArray = new T[columns][];
            for (int i = 0; i < jaggedArray.Length; i++)
            {
                jaggedArray[i] = new T[rows];
            }
        }

        /// <summary>
        /// Clears a jagged array by nulling out its outer arrays.
        /// </summary>
        /// <typeparam name="T">The type of the jagged array.</typeparam>
        /// <param name="jaggedArray">The jagged array of type T to clear.</param>
        public static void ClearJaggedArray<T>(ref T[][] jaggedArray)
        {
            if (jaggedArray != null)
            {
                for (int i = 0; i < jaggedArray.Length; i++)
                {
                    jaggedArray[i] = null;
                }
            }
        }

        #region Line Intersection

        /// <summary>
        /// A class for checking Line intersection.
        /// </summary>
        public static class LineIntersection
        {
            private const double CloseToZero = .00001d;

            /// <summary>
            /// Checks the overlap between two Lines. If no overlap exists, returns null.
            /// </summary>
            /// <param name="line1">The first Line to test overlap with.</param>
            /// <param name="line2">The second Line to test overlap with.</param>
            /// <remarks>Code obtained from here: http://stackoverflow.com/q/22456517 and modified to handle division by 0.</remarks>
            /// <returns>A Line containing the overlap points between the two lines. null if no overlap exists.</returns>
            public static Line? GetLineOverlap(Line line1, Line line2)
            {
                bool undefinedSlope = false;
                double xDiff = (line1.P2.X - line1.P1.X);
                if (xDiff == 0f) undefinedSlope = true;

                double slope = 0d;
                if (undefinedSlope == false) slope = (line1.P2.Y - line1.P1.Y) / (line1.P2.X - line1.P1.X);

                bool isHorizontal = (undefinedSlope == true) ? false : AlmostZero(slope);
                bool isDescending = slope < 0 && !isHorizontal;
                double invertY = isDescending || isHorizontal ? -1 : 1;

                Point min1 = new Point(Math.Min(line1.P1.X, line1.P2.X), (int)Math.Min(line1.P1.Y * invertY, line1.P2.Y * invertY));
                Point max1 = new Point(Math.Max(line1.P1.X, line1.P2.X), (int)Math.Max(line1.P1.Y * invertY, line1.P2.Y * invertY));

                Point min2 = new Point(Math.Min(line2.P1.X, line2.P2.X), (int)Math.Min(line2.P1.Y * invertY, line2.P2.Y * invertY));
                Point max2 = new Point(Math.Max(line2.P1.X, line2.P2.X), (int)Math.Max(line2.P1.Y * invertY, line2.P2.Y * invertY));

                Point minIntersection;
                if (isDescending)
                    minIntersection = new Point(Math.Max(min1.X, min2.X), (int)Math.Min(min1.Y * invertY, min2.Y * invertY));
                else
                    minIntersection = new Point(Math.Max(min1.X, min2.X), (int)Math.Max(min1.Y * invertY, min2.Y * invertY));

                Point maxIntersection;
                if (isDescending)
                    maxIntersection = new Point(Math.Min(max1.X, max2.X), (int)Math.Max(max1.Y * invertY, max2.Y * invertY));
                else
                    maxIntersection = new Point(Math.Min(max1.X, max2.X), (int)Math.Min(max1.Y * invertY, max2.Y * invertY));

                bool intersect = minIntersection.X <= maxIntersection.X &&
                                 (!isDescending && minIntersection.Y <= maxIntersection.Y ||
                                   isDescending && minIntersection.Y >= maxIntersection.Y);

                if (!intersect)
                    return null;

                return new Line(minIntersection, maxIntersection);
            }

            /// <summary>
            /// Determines if two Lines intersect.
            /// </summary>
            /// <param name="line1">The first Line to test intersection with.</param>
            /// <param name="line2">The second Line to test intersection with.</param>
            /// <remarks>Code obtained from here: http://gamedev.stackexchange.com/a/26022 </remarks>
            /// <returns>true if the Lines intersect each other, otherwise false.</returns>
            public static bool Intersects(Line line1, Line line2)
            {
                //Find if a Line intersects:
                /*Formula:
                 *Pa = P1+Ua(P2-P1)
                 *Pb = P3+Ub(P4-P3)
                 * 0 for U = start, 1 = end
                 * Pa=Pb
                 * P1+Ua(P2-P1)=P3+Ub(P4-P3)
                 * X-Y Terms:
                 * x1+Ua(x2-x1)=x3+Ub(x4-x3)
                 * y1+Ua(y2-y1)=y3+Ub(y4-y3)
                 * 
                 * Solve for U:
                 * Ua=((x4-x3)(y1-y3)-(y4-y3)(x1-x3))/((y4-y3)(x2-x1)-(x4-x3)(y2-y1))
                 * Ub=((x2-x1)(y1-y3)-(y2-y1)(x1-x3))/((y4-y3)(x2-x1)-(x4-x3)(y2-y1))
                 * 
                 * Solve denominator first: if 0, then the lines are parallel and don't intersect
                 * If both numerators are 0, then the two lines are coincident (lie on top of each other, but may or may not overlap)
                 * 
                 * Check:
                 * 0<=Ua<= 1
                 * 0<=Ub<=1
                 * 
                 * If so, the lines intersect. To find point of intersection:
                 * x=x1+Ua(x2-x1)
                 * y=y1+Ua(y2-y1)
                */

                Point a = line1.P1;
                Point b = line1.P2;
                Point c = line2.P1;
                Point d = line2.P2;

                //numerator1 = Ua, numerator2 = Ub
                float denominator = ((d.Y - c.Y) * (b.X - a.X)) - ((d.X - c.X) * (b.Y - a.Y));
                float numerator1 = ((d.X - c.X) * (a.Y - c.Y)) - ((d.Y - c.Y) * (a.X - c.X));
                float numerator2 = ((b.X - a.X) * (a.Y - c.Y)) - ((b.Y - a.Y) * (a.X - c.X));

                //Check for parallel - check for a close to 0 value since these are floats
                if (Math.Abs(denominator) <= CloseToZero)
                {
                    //Parallel; check if they are coincident

                    //Check if they're coincident - if so, make sure they don't overlap
                    if (Math.Abs(numerator1) <= CloseToZero && Math.Abs(numerator2) <= CloseToZero)
                    {
                        //Check for an overlap
                        Line? overlap = GetLineOverlap(line1, line2);

                        return (overlap != null);
                    }
                    //Not coincident, no intersection
                    else
                    {
                        return false;
                    }
                }

                float r = numerator1 / denominator;
                float s = numerator2 / denominator;

                return ((r >= 0 && r <= 1) && (s >= 0 && s <= 1));
            }

            private static bool AlmostEqualTo(double value1, double value2)
            {
                return Math.Abs(value1 - value2) <= CloseToZero;
            }

            private static bool AlmostZero(double value)
            {
                return Math.Abs(value) <= CloseToZero;
            }
        }

        #endregion
    }
}
