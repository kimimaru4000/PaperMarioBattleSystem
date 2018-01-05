using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A class containing useful interpolation methods.
    /// </summary>
    public static class Interpolation
    {
        /// <summary>
        /// The types of interpolation.
        /// </summary>
        public enum InterpolationTypes
        {
            Linear,
            QuadIn,
            QuadOut,
            QuadInOut,
            CubicIn,
            CubicOut,
            CubicInOut,
            ExponentialIn,
            ExponentialOut,
            ExponentialInOut,
            SineIn,
            SineOut,
            SineInOut
        }

        /// <summary>
        /// A delegate for an interpolation method.
        /// This should adjust the time value based on the interpolation, which will be used in the interpolation method.
        /// </summary>
        /// <param name="time">The time, from 0 to 1.</param>
        /// <returns>A double with the adjusted time value.</returns>
        public delegate double InterpolationMethod(double time);

        #region Internal Time Calculations

        //These private methods will calculate the time, while the public methods will return values.
        //They are based off Robert Penner's easing equations in Actionscript found here: http://gizma.com/easing/
        //Some methods sourced from: https://github.com/acron0/Easings/blob/master/Easings.cs and https://joshondesign.com/2013/03/01/improvedEasingEquations with modifications

        private static double LinearTime(double time)
        {
            return time;
        }

        private static double EaseInQuadTime(double time)
        {
            return Math.Pow(time, 2);
        }

        private static double EaseOutQuadTime(double time)
        {
            return (1 - EaseInQuadTime(1 - time));
        }

        private static double EaseInOutQuadTime(double time)
        {
            if (time < .5d) return (EaseInQuadTime(time * 2d) / 2d);
            return (1 - (EaseInQuadTime((1 - time) * 2d) / 2d));
        }

        private static double EaseInCubicTime(double time)
        {
            return Math.Pow(time, 3);
        }

        private static double EaseOutCubicTime(double time)
        {
            return (1 - EaseInCubicTime(1 - time));
        }

        private static double EaseInOutCubicTime(double time)
        {
            if (time < .5d) return (EaseInCubicTime(time * 2d) / 2d);
            return (1 - (EaseInCubicTime((1 - time) * 2d) / 2d));
        }

        private static double EaseInExponentialTime(double time)
        {
            //Exponential gets close to the starting value, but not to it
            if (time == 0d) return time;
            return Math.Pow(2, 10 * (time - 1));
        }

        private static double EaseOutExponentialTime(double time)
        {
            //Exponential gets close to the final value, but not to it
            if (time == 1d) return time;
            return -Math.Pow(2, -10 * time) + 1;
        }

        private static double EaseInOutExponentialTime(double time)
        {
            //Exponential gets close to the starting and final values, but not to them
            if (time == 0d || time == 1d) return time;

            if (time < .5d) return (.5d * Math.Pow(2, (20 * time) - 10));
            return (.5d * -Math.Pow(2, (-20 * time) + 10)) + 1;
        }

        private static double EaseInSineTime(double time)
        {
            return Math.Sin((time - 1) * UtilityGlobals.HalfPI) + 1;
        }

        private static double EaseOutSineTime(double time)
        {
            return Math.Sin(time * UtilityGlobals.HalfPI);
        }

        private static double EaseInOutSineTime(double time)
        {
            return (.5d * (1 - Math.Cos(time * Math.PI)));
        }

        #endregion

        /// <summary>
        /// Gets the appropriate interpolation method based on the InterpolationType passed in.
        /// </summary>
        /// <param name="interpolationType">The InterpolationTypes to get.</param>
        /// <returns>The interpolation method associated with the InterpolationType, otherwise null.</returns>
        private static InterpolationMethod GetInterpolationFromType(InterpolationTypes interpolationType)
        {
            switch (interpolationType)
            {
                case InterpolationTypes.Linear: return LinearTime;
                case InterpolationTypes.QuadIn: return EaseInQuadTime;
                case InterpolationTypes.QuadOut: return EaseOutQuadTime;
                case InterpolationTypes.QuadInOut: return EaseInOutQuadTime;
                case InterpolationTypes.CubicIn: return EaseInCubicTime;
                case InterpolationTypes.CubicOut: return EaseOutCubicTime;
                case InterpolationTypes.CubicInOut: return EaseInOutCubicTime;
                case InterpolationTypes.ExponentialIn: return EaseInExponentialTime;
                case InterpolationTypes.ExponentialOut: return EaseOutExponentialTime;
                case InterpolationTypes.ExponentialInOut: return EaseInOutExponentialTime;
                case InterpolationTypes.SineIn: return EaseInSineTime;
                case InterpolationTypes.SineOut: return EaseOutSineTime;
                case InterpolationTypes.SineInOut: return EaseInOutSineTime;
                default: return null;
            }
        }

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A double in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static double Interpolate(double startVal, double endVal, double time, InterpolationTypes interpolationType)
        {
            return CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation with an interpolation method.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolateMethod">The interpolation method.</param>
        /// <returns>A double in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation method.</returns>
        public static double CustomInterpolate(double startVal, double endVal, double time, InterpolationMethod interpolateMethod)
        {
            //Return the starting value if the interpolation method is null
            if (interpolateMethod == null) return startVal;

            double diff = endVal - startVal;
            double interpolateVal = interpolateMethod(time);
            return startVal + (interpolateVal * diff);
        }

        #region Overloads

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>An int in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static int Interpolate(int startVal, int endVal, double time, InterpolationTypes interpolationType)
        {
            return (int)CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting value.</param>
        /// <param name="endVal">The ending value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A float in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static float Interpolate(float startVal, float endVal, double time, InterpolationTypes interpolationType)
        {
            return (float)CustomInterpolate(startVal, endVal, time, GetInterpolationFromType(interpolationType));
        }

        /// <summary>
        /// Performs interpolation based on the InterpolationType.
        /// </summary>
        /// <param name="startVal">The starting Vector2 value.</param>
        /// <param name="endVal">The ending Vector2 value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="interpolationType">The type of interpolation.</param>
        /// <returns>A Vector2 in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the interpolation type.</returns>
        public static Vector2 Interpolate(Vector2 startVal, Vector2 endVal, double time, InterpolationTypes interpolationType)
        {
            float x = Interpolate(startVal.X, endVal.X, time, interpolationType);
            float y = Interpolate(startVal.Y, endVal.Y, time, interpolationType);

            return new Vector2(x, y);
        }

        /// <summary>
        /// Performs interpolation between Vector2s based on the InterpolationTypes.
        /// </summary>
        /// <param name="startVal">The starting Vector2 value.</param>
        /// <param name="endVal">The ending Vector2 value.</param>
        /// <param name="time">The time, between 0 and 1.</param>
        /// <param name="xInterpolationType">The type of interpolation for the X value.</param>
        /// <param name="yInterpolationType">The type of interpolation for the Y value.</param>
        /// <returns>A Vector2 in the range of <paramref name="startVal"/> and <paramref name="endVal"/> based on the X and Y interpolation types.</returns>
        public static Vector2 Interpolate(Vector2 startVal, Vector2 endVal, double time, InterpolationTypes xInterpolationType, InterpolationTypes yInterpolationType)
        {
            float x = Interpolate(startVal.X, endVal.X, time, xInterpolationType);
            float y = Interpolate(startVal.Y, endVal.Y, time, yInterpolationType);

            return new Vector2(x, y);
        }

        #endregion
    }
}
