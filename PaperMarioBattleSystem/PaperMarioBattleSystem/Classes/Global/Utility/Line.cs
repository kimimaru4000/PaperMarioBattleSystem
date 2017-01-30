using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Describes a 2D-line, which consists of two points.
    /// </summary>
    public struct Line : IEquatable<Line>
    {
        Point P1;
        Point P2;

        public Line(Point p1, Point p2)
        {
            P1 = p1;
            P2 = p2;
        }

        /// <summary>
        /// Determines of this Line intersects another Line.
        /// </summary>
        /// <param name="other">The Line to test intersection with.</param>
        /// <remarks>Code obtained from here: http://gamedev.stackexchange.com/a/26022</remarks>
        /// <returns>true if this Line intersects the other, otherwise false.</returns>
        public bool Intersects(Line other)
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
             * If both numerators are 0, then the two lines are coincident (exacty the same)
             * 
             * Check:
             * 0<=Ua<= 1
             * 0<=Ub<=1
             * 
             * If so, the lines intersect. To find point of intersection:
             * x=x1+Ua(x2-x1)
             * y=y1+Ua(y2-y1)
            */

            const float closeToZero = 0.00001f;

            Point a = P1;
            Point b = P2;
            Point c = other.P1;
            Point d = other.P2;

            float denominator = ((d.Y - c.Y) * (b.X - a.X)) - ((d.X - c.X) * (b.Y - a.Y));
            float numerator1 = ((d.X - c.X) * (a.Y - c.Y)) - ((d.Y - c.Y) * (a.X - c.X));
            float numerator2 = ((b.X - a.X) * (a.Y - c.Y)) - ((b.Y - a.Y) * (a.X - c.X));

            //Check for parallel - check for a close to 0 value since these are floats
            if (Math.Abs(denominator) <= closeToZero)
            {
                //Parallel; check if they are coincident
                //NOTE: This will still return an intersection if the lines are coincident but don't overlap
                return (Math.Abs(numerator1) <= closeToZero && Math.Abs(numerator2) <= closeToZero);
            }

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return ((r >= 0 && r <= 1) && (s >= 0 && s <= 1));
        }

        /// <summary>
        /// Determines if two Lines intersect each other.
        /// </summary>
        /// <param name="line1">The first Line to test intersection with.</param>
        /// <param name="line2">The second Line to test intersection with.</param>
        /// <returns>true if the Lines intersect each other, otherwise false.</returns>
        public static bool Intersects(Line line1, Line line2)
        {
            return line1.Intersects(line2);
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
    }
}
