using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Describes a 2D-line, which consists of two points.
    /// </summary>
    public struct Line : IEquatable<Line>
    {
        public Point P1;
        public Point P2;

        public Line(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        public Line(int x1, int y1, int x2, int y2) : this (new Point(x1, y1), new Point(x2, y2))
        {

        }

        /// <summary>
        /// Determines if this Line intersects another Line.
        /// </summary>
        /// <param name="other">The Line to test intersection with.</param>
        /// <remarks>Code obtained from here: http://gamedev.stackexchange.com/a/26022 </remarks>
        /// <returns>true if this Line intersects the other, otherwise false.</returns>
        public bool Intersects(Line other)
        {
            return Intersects(this, other);
        }

        /// <summary>
        /// Gets the length of the Line.
        /// </summary>
        /// <returns>A double corresponding to the length of the Line.</returns>
        public double GetLength()
        {
            //Use the distance formula: d = sqrt((x2 - x1)^2 + (y2 - y1)^2)
            Vector2 diff = P2.ToVector2() - P1.ToVector2();

            double xSquared = Math.Pow(diff.X, 2);
            double ySquared = Math.Pow(diff.Y, 2);

            double length = Math.Sqrt(xSquared + ySquared);

            return length;
        }

        /// <summary>
        /// Gets the direction of the Line.
        /// </summary>
        /// <returns>A normalized Vector2 showing the direction the Line is pointing.</returns>
        public Vector2 GetDirection()
        {
            Vector2 direction = P2.ToVector2() - P1.ToVector2();
            direction.Normalize();

            return direction;
        }

        /// <summary>
        /// Gets the angle of the Line, in radians or degrees.
        /// </summary>
        /// <returns>A float of the angle the Line is pointing, in radians or degrees.</returns>
        public float GetLineAngle(bool inDegrees)
        {
            Vector2 point1 = P1.ToVector2();
            Vector2 point2 = P2.ToVector2();

            double radians = Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            if (inDegrees == false) return (float)radians;

            double degrees = UtilityGlobals.ToDegrees(radians);

            return (float)degrees;
        }

        /// <summary>
        /// Gets the center of the Line.
        /// </summary>
        /// <returns>Gets the center point of the line</returns>
        public Vector2 GetCenter()
        {
            Vector2 diff = ((P2.ToVector2() - P1.ToVector2()) / 2f);

            return new Vector2(P1.X + (int)diff.X, P1.Y + (int)diff.Y);
        }

        /// <summary>
        /// Gets the center origin of the Line.
        /// </summary>
        /// <returns>A Vector2 with the center origin of the Line.</returns>
        public Vector2 GetCenterOrigin()
        {
            Vector2 diff = P2.ToVector2() - P1.ToVector2();
            diff.X = Math.Abs(diff.X);
            diff.Y = Math.Abs(diff.Y);

            return (diff / 2);
        }

        public static void UnitTestCoincident()
        {
            Line line1 = new Line(400, 80, 450, 80);
            Line line2 = new Line(200, 80, 399, 80);

            Debug.Log($"line 1: {line1} and line 2: {line2}");
            Debug.Log($"Do they intersect? {line1.Intersects(line2)}");
        }

        #region Comparison and Operator Overloading

        public bool Equals(Line other)
        {
            return (P1.Equals(other.P1) && P2.Equals(other.P2));
        }

        public override bool Equals(object obj)
        {
            return (obj is Line) && this.Equals((Line)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 18;
                hash = (hash * 24) + P1.GetHashCode();
                hash = (hash * 24) + P2.GetHashCode();
                return hash;
            }
        }

        public override string ToString()
        {
            return $"{P1.ToString()} {P2.ToString()}";
        }

        public static Line operator +(Line a, Line b)
        {
            return new Line(a.P1 + b.P1, a.P2 + b.P2);
        }

        public static Line operator -(Line a, Line b)
        {
            return new Line(a.P1 - b.P1, a.P2 - b.P2);
        }

        public static Line operator *(Line a, Line b)
        {
            return new Line(a.P1 * b.P1, a.P2 * b.P2);
        }

        public static Line operator /(Line a, Line b)
        {
            return new Line(a.P1 / b.P1, a.P2 / b.P2);
        }

        public static bool operator ==(Line a, Line b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Line a, Line b)
        {
            return (a.Equals(b) == false);
        }

        #endregion

        #region Static Methods

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

        #endregion
    }
}
